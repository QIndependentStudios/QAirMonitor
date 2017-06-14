using QAirMonitor.Abstract.Business;
using QAirMonitor.Abstract.Persist;
using QAirMonitor.Domain.Models;
using QAirMonitor.Persist.Repositories;
using QAirMonitor.UWP.Pages;
using QAirMonitor.UWP.Shared.Services;
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
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

namespace QAirMonitor.UWP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Fields
        private readonly IReadAllRepository<ReadingModel> _readAllRepo;
        private readonly IWriteRepository<ReadingModel> _writeRepo;

        private readonly SettingsService _settings;

        private ObservableCollection<ReadingModel> _readings;
        private double _rangeSize;
        private double _rangeMin;
        private double _rangeMax;
        private Size _zoomFactor;
        private Point _scrollOffset;
        private bool _autoScroll = true;
        private bool _maintainScope;
        private string _temperature;
        private string _humidity;
        private string _startup;
        private string _lastReading;
        private int _selectedPivotIndex;
        #endregion

        #region Constructors
        public MainPageViewModel()
        {
            var repo = new HistoricalReadingRepository();
            _readAllRepo = repo;
            _writeRepo = repo;

            _settings = SettingsService.Instance;
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

        public int SelectedPivotIndex
        {
            get { return _selectedPivotIndex; }
            set
            {
                Set(ref _selectedPivotIndex, value);
                RaisePropertyChanged(nameof(IsGraphVisible));
            }
        }

        public bool IsGraphVisible => SelectedPivotIndex == 0;

        public int LowerTempRangeThreshold => _settings.LowerTempRangeThreshold;

        public int UpperTempRangeThreshold => _settings.UpperTempRangeThreshold;

        public int LowerHumidityRangeThreshold => _settings.LowerHumidityRangeThreshold;

        public int UpperHumidityRangeThreshold => _settings.UpperHumidityRangeThreshold;
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

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Startup = $"Started: {App.Startup:M/d/yyyy h:mm:ss tt}";
            await LoadReadings();

            if (App.SensorDataCollector != null)
            {
                App.SensorDataCollector.ReadingReceived += SensorDataCollector_ReadingReceived;
                await UpdateDisplay(App.SensorDataCollector.LastReading, App.SensorDataCollector.LastReadingAttempts);
            }

            await base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            if (App.SensorDataCollector != null)
                App.SensorDataCollector.ReadingReceived -= SensorDataCollector_ReadingReceived;

            return base.OnNavigatingFromAsync(args);
        }

        private async Task UpdateDisplay(ReadingModel reading, int attempts)
        {
            if (reading == null)
            {
                Temperature = "Unknown";
                Humidity = "Unknown";
                LastReading = $"Read on {DateTime.Now:M/d/yyyy h:mm:ss tt} with {attempts} attempt(s).";
                return;
            }

            Temperature = $"{reading.Temperature:0.0}°C";
            Humidity = $"{reading.Humidity:0.0}%";
            LastReading = $"Read on {reading.ReadingDateTime:M/d/yyyy h:mm:ss tt} with {attempts} attempt(s).";

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    Readings.Insert(0, reading);

                    SetRangeSize();
                    ScrollRangeToLatest();
                });
        }
        #endregion

        #region Event Handlers
        private async void SensorDataCollector_ReadingReceived(object sender, ISensorReadingReceivedEventArgs<ReadingModel> e)
        {
            await UpdateDisplay(e.NewReading, e.Attempts);
        }

        public async void Reset()
        {
            await LoadReadings();
            AutoScroll = true;
        }

        public void ViewLogs()
        {
            NavigationService.Navigate(typeof(LogPage));
        }

        public void Settings()
        {
            NavigationService.Navigate(typeof(SettingsPage));
        }

        public async Task AddSampleReadings()
        {
            var rand = new Random();

            var lastTime = DateTime.Now.Floor(App.DataCollectionInterval);

            var totalEntries = (int)((24 * 60) / App.DataCollectionInterval.TotalMinutes);
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
