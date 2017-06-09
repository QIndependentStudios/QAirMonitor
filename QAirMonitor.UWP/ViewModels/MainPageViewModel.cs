using QAirMonitor.Abstract.Business;
using QAirMonitor.Abstract.Persist;
using QAirMonitor.Business.SensorDataCollection;
using QAirMonitor.Domain.Models;
using QAirMonitor.Hardware.UWP.Sensors;
using QAirMonitor.Persist.Repositories;
using QAirMonitor.UWP.Timers;
using QAirMonitor.UWP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

namespace QAirMonitor.UWP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Fields
        private readonly IReadAllRepository<ReadingModel> _readAllRepo;
        private readonly IWriteRepository<ReadingModel> _writeRepo;
        private readonly ISensorDataCollector<ReadingModel> _sensorDataCollector;

        private readonly TimeSpan DataCollectionInterval = TimeSpan.FromMinutes(5);

        private ObservableCollection<ReadingModel> _readings;
        private double _rangeSize;
        private double _rangeMin;
        private double _rangeMax;
        private Size _zoomFactor;
        private Point _scrollOffset;
        private bool _autoScroll = true;
        private bool _maintainScope;
        private string _temperature = "n/a";
        private string _humidity = "n/a";
        private string _startup;
        private string _lastReading;
        #endregion

        #region Constructors
        public MainPageViewModel()
        {
            var repo = new HistoricalReadingRepository();
            _readAllRepo = repo;
            _writeRepo = repo;

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT")
                _sensorDataCollector = new TempHumidityDataCollector(new SnappedIntervalTimer(DataCollectionInterval),
                    new DhtTempHumiditySensor());
            else
                _sensorDataCollector = new TempHumidityDataCollector(new SnappedIntervalTimer(DataCollectionInterval),
                    new VirtualTempHumiditySensor());

            _sensorDataCollector.ReadingReceived += SensorDataCollector_ReadingReceived;
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

        public string Temperature
        {
            get { return _temperature; }
            set { Set(ref _temperature, value); }
        }

        public string Humidity
        {
            get { return _humidity; }
            set { Set(ref _humidity, value); }
        }

        public string Startup
        {
            get { return _startup; }
            set { Set(ref _startup, value); }
        }

        public string LastReading
        {
            get { return _lastReading; }
            set { Set(ref _lastReading, value); }
        }
        #endregion

        #region Methods
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
            Startup = $"Started: {DateTime.Now:M/d/yyyy h:mm:ss tt}";

            if (Readings.Count == 0)
            {
                var reading = new ReadingModel
                {
                    Temperature = 0,
                    Humidity = 0,
                    ReadingDateTime = DateTime.Now.Floor(DataCollectionInterval)
                };

                await _writeRepo.WriteAsync(reading);
                Readings.Insert(0, reading);
            }

            _sensorDataCollector.Start();
        }

        private void StopDataCollection()
        {
            _sensorDataCollector.Stop();
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
        #endregion

        #region Event Handlers
        private async void SensorDataCollector_ReadingReceived(object sender, ISensorReadingReceivedEventArgs<ReadingModel> e)
        {
            if (e.NewReading == null)
            {
                Temperature = "Error";
                Humidity = "Error";
                LastReading = $"Read on {DateTime.Now:M/d/yyyy h:mm:ss tt} with {e.Attempts} attempt(s).";
                return;
            }

            Temperature = $"{e.NewReading.Temperature:0.00}°C";
            Humidity = $"{e.NewReading.Humidity:0.00}%";
            LastReading = $"Read on {e.NewReading.ReadingDateTime:M/d/yyyy h:mm:ss tt} with {e.Attempts} attempt(s).";

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { Readings.Insert(0, e.NewReading); });

            SetRangeSize();
            ScrollRangeToLatest();
        }

        public async void Reset()
        {
            await LoadReadings();
            AutoScroll = true;
        }

        public async Task AddSampleReadings()
        {
            var rand = new Random();

            var lastTime = DateTime.Now.Floor(DataCollectionInterval);

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
}
