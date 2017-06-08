using QAirMonitor.Abstract.Business;
using QAirMonitor.Abstract.Persist;
using QAirMonitor.Domain.Models;
using QAirMonitor.Persist.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

namespace QAirMonitor.UWP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Fields
        private readonly IReadAllRepository<ReadingModel> _readAllRepo;
        private readonly IWriteRepository<ReadingModel> _writeRepo;
        private readonly ITempHumiditySensor<ReadingModel> _sensor;

        private readonly TimeSpan DataCollectionInterval = TimeSpan.FromMinutes(5);

        private ThreadPoolTimer _timer;

        private ObservableCollection<ReadingModel> _readings;
        private double _rangeSize;
        private double _rangeMin;
        private double _rangeMax;
        private Size _zoomFactor;
        private Point _scrollOffset;
        private bool _autoScroll = true;
        private bool _maintainScope;
        #endregion

        #region Constructors
        public MainPageViewModel()
        {
            var repo = new HistoricalReadingRepository();
            _readAllRepo = repo;
            _writeRepo = repo;

            _sensor = new TestTempHumiditySensor();
        }
        #endregion

        #region Properties
        public ObservableCollection<ReadingModel> Readings
        {
            get { return _readings; }
            set { Set(ref _readings, value); }
        }

        public double RangeMin
        {
            get { return _rangeMin; }
            set
            {
                Set(ref _rangeMin, value);
                UpdateGraphView();
            }
        }

        public double RangeMax
        {
            get { return _rangeMax; }
            set
            {
                Set(ref _rangeMax, value);
                UpdateGraphView();
            }
        }

        public Size ZoomFactor
        {
            get { return _zoomFactor; }
            set { Set(ref _zoomFactor, value); }
        }

        public Point ScrollOffset
        {
            get { return _scrollOffset; }
            set { Set(ref _scrollOffset, value); }
        }

        public double RangeSize
        {
            get { return _rangeSize; }
            set { Set(ref _rangeSize, value); }
        }

        public bool AutoScroll
        {
            get { return _autoScroll; }
            set { Set(ref _autoScroll, value); }
        }

        public bool MaintainScope
        {
            get { return _maintainScope; }
            set { Set(ref _maintainScope, value); }
        }
        #endregion

        #region Methods
        private static DateTime Round(DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        private static DateTime Floor(DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks / span.Ticks);
            return new DateTime(ticks * span.Ticks);
        }

        private static DateTime Ceiling(DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        private void SetRangeSize()
        {
            var dateTimes = Readings.Select(r => r.ReadingDateTime).ToList();

            if (dateTimes.Count <= 1)
            {
                RangeSize = 1;
                return;
            }

            var min = dateTimes.Min();
            var max = dateTimes.Max();

            var diff = max - min;

            RangeSize = diff.TotalHours;
        }

        private void UpdateGraphView()
        {
            var rangeSize = (RangeMax - RangeMin) / RangeSize;
            var offset = (RangeMin / RangeSize) / rangeSize;

            ZoomFactor = new Size(1 / rangeSize, 1);
            ScrollOffset = new Point(-offset, 0);
        }

        private void SetDefaultSelectedRange()
        {
            RangeMax = RangeSize;
            RangeMin = Math.Max(0, RangeSize - 6);

            if (RangeMin != 0)
                MaintainScope = true;
        }

        private void ScrollRangeToLatest()
        {
            if (!AutoScroll)
            {
                UpdateGraphView();
                return;
            }

            var originalRangeMax = RangeMax;
            RangeMax = RangeSize;

            if (!MaintainScope)
            {
                UpdateGraphView();
                return;
            }

            var range = originalRangeMax - RangeMin;
            RangeMin = RangeMax - range;
        }

        private async Task LoadReadings()
        {
            Readings = new ObservableCollection<ReadingModel>(await _readAllRepo.GetAllAsync());

            SetRangeSize();
            SetDefaultSelectedRange();
        }

        private async Task StartDataCollection()
        {
            if (Readings.Count == 0)
            {
                var reading = new ReadingModel
                {
                    Temperature = 0,
                    Humidity = 0,
                    ReadingDateTime = Floor(DateTime.Now, DataCollectionInterval)
                };

                await _writeRepo.WriteAsync(reading);
                Readings.Insert(0, reading);
            }

            var preferredStartTime = Ceiling(DateTime.Now, DataCollectionInterval);
            var delay = preferredStartTime - DateTime.Now;

            await Task.Delay((int)delay.TotalMilliseconds);

            CollectSensorData();
            _timer = ThreadPoolTimer.CreatePeriodicTimer(CollectSensorData,
                DataCollectionInterval);
        }

        private void StopDataCollection()
        {
            _timer.Cancel();
            _timer = null;
        }

        private async void CollectSensorData(ThreadPoolTimer timer = null)
        {
            var reading = _sensor.GetReading();

            await _writeRepo.WriteAsync(reading);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { Readings.Insert(0, reading); });

            SetRangeSize();
            ScrollRangeToLatest();
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await LoadReadings();
            await StartDataCollection();
            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            StopDataCollection();
            return base.OnNavigatingFromAsync(args);
        }

        public async Task AddSampleReadings()
        {
            var rand = new Random();

            var lastTime = Floor(DateTime.Now, DataCollectionInterval);

            var totalEntries = (int)((24 * 60) / DataCollectionInterval.TotalMinutes);
            for (int i = 0; i < totalEntries; i++)
            {
                var reading = new ReadingModel
                {
                    Temperature = rand.Next(-2, 5),
                    Humidity = rand.Next(50, 85),
                    ReadingDateTime = DateTime.Now.AddMinutes((totalEntries - i) * -5)
                };
                await _writeRepo.WriteAsync(reading);
                Readings.Insert(0, reading);
                SetRangeSize();
                ScrollRangeToLatest();
                await Task.Delay(1000);
            }
        }
        #endregion
    }

    public class TestTempHumiditySensor : ITempHumiditySensor<ReadingModel>
    {
        private readonly Random _rand = new Random();

        public ReadingModel GetReading()
        {
            return new ReadingModel
            {
                Temperature = _rand.Next(-200, 500) / 100.0,
                Humidity = _rand.Next(5000, 8500) / 100.0,
                ReadingDateTime = DateTime.Now
            };
        }
    }
}
