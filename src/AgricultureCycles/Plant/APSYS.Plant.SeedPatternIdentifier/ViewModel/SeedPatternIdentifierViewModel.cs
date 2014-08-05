namespace APSYS.Plant.SeedPatternIdentifier.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Core.MVVM;
    using Domain;
    using Infrastructure.Communication.Domain.Serial;
    using Infrastructure.Communication.SerialPortControl;
    using NLog;
    using NLog.Targets;
    using NLog.Targets.Wrappers;
    using Service;
    using View;

    public class SeedPatternIdentifierViewModel : BaseViewModel<SeedPatternIdentifierView>
    {
        private readonly SerialPortService _serialPortService;
        private readonly SeedTubeDataService _seedTubeDataService;
        private readonly PlanterService _planterService;
        private Logger _logger;
        private DispatcherTimer _dispatcherTimerCalibration;

        public SeedPatternIdentifierViewModel(SerialPortService serialPortService, SerialPortControlView serialPortControlView, SeedTubeDataService seedTubeDataService, PlanterService planterService)
        {
            SerialPortControl = serialPortControlView;
            _serialPortService = serialPortService;
            _seedTubeDataService = seedTubeDataService;
            _planterService = planterService;
            _logger = LogManager.GetLogger("logDataRule");
            GlobalDiagnosticsContext.Set("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        }

        public SerialPortControlView SerialPortControl { get; set; }

        public ObservableCollection<SensorParameter> CalibrationParameters { get; set; }

        public Visibility CalibrationSettingsVisibility
        {
            get
            {
                if (_seedTubeDataService.SensorParameters == null || _seedTubeDataService.SensorParameters.Count < 1)
                {
                    return Visibility.Hidden;
                }

                return Visibility.Visible;
            }
        }

        public ICommand SaveCommand
        {
            get { return new RelayCommand(Save, CanSave); }
        }

        public ICommand PatternIdentifierCommand
        {
            get { return new RelayCommand(PatternIdentifier, CanPatternIdentifier); }
        }

        public override void Initialize()
        {
            CalibrationParameters = new ObservableCollection<SensorParameter>();
        }

        private bool CanPatternIdentifier()
        {
            return _serialPortService.IsOpen;
        }

        private void PatternIdentifier()
        {
            _serialPortService.IsCalibration = true;

            _dispatcherTimerCalibration = new DispatcherTimer();
            _dispatcherTimerCalibration.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimerCalibration.Tick += CalibrationTick;
            _dispatcherTimerCalibration.Start();

            _logger.Info("Timer de calibramento iniciado");
        }

        private void CalibrationTick(object sender, EventArgs e)
        {
            _dispatcherTimerCalibration.Stop();
            _logger.Info("Timer de calibramento finalizado");

            _serialPortService.IsCalibration = false;

            string tst = string.Empty;
            string dataLog;
            while (_serialPortService.CalibrationDataEnqueue.TryDequeue(out dataLog))
            {
                _logger.Info(dataLog);
                tst += dataLog;
            }

            var seedTubeDataText = _seedTubeDataService.SplitPureTextToSeedTube(tst);
            var seedTubeDataReadings = _seedTubeDataService.MountSeedTubeDataReadings(seedTubeDataText);

            IEnumerable<IGrouping<int, SeedTubeData>> allSeedsGroupBySensorNumber = seedTubeDataReadings.SelectMany(c => c.SeedTubeDataReadings).GroupBy(a => a.SensorNumber).ToList();

            foreach (var seedTubeDatas in allSeedsGroupBySensorNumber)
            {
                int maxValuePerSensor = seedTubeDatas.Max(a => a.SensorValue);

                SensorParameter sp = new SensorParameter()
                {
                    SensorMaxValue = (int)(maxValuePerSensor * 1.1),
                    SensorNumber = seedTubeDatas.Key
                };

                _seedTubeDataService.AddSensorParameter(sp);
            }

            CalibrationParameters.Clear();
            foreach (var sensorParameter in _seedTubeDataService.SensorParameters)
            {
                CalibrationParameters.Add(sensorParameter);
            }

            RaisePropertyChanged("CalibrationSettingsVisibility");
        }

        private bool CanSave()
        {
            /* if (_serialPortService == null || _serialPortService.DataEnqueue.Count < 1)
             {
                 return false;
             }*/

            return true;
        }

        private void Save()
        {
            var names = LogManager.Configuration.FileNamesToWatch;

            var fileName = GetLogFileName("logData");
            var directory = Directory.GetParent(fileName);

            var lastOrDefault = directory.EnumerateFiles().LastOrDefault();
            if (lastOrDefault != null)
            {
                try
                {
                    StreamReader str = new StreamReader(lastOrDefault.FullName);
                    var planter = _planterService.Verify(str.ReadToEnd());
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.Error(e.Message);
                }
            }

            /*_serialPortService.Close();
            string message = string.Empty;
            while (_serialPortService.DataEnqueue.TryDequeue(out message))
            {
                _logger.Info(message);
            }*/
        }

        private string GetLogFileName(string targetName)
        {
            string fileName = null;

            if (LogManager.Configuration != null && LogManager.Configuration.ConfiguredNamedTargets.Count != 0)
            {
                Target target = LogManager.Configuration.FindTargetByName(targetName);
                if (target == null)
                {
                    throw new Exception("Could not find target named: " + targetName);
                }

                FileTarget fileTarget = null;
                WrapperTargetBase wrapperTarget = target as WrapperTargetBase;

                // Unwrap the target if necessary.
                if (wrapperTarget == null)
                {
                    fileTarget = target as FileTarget;
                }
                else
                {
                    fileTarget = wrapperTarget.WrappedTarget as FileTarget;
                }

                if (fileTarget == null)
                {
                    throw new Exception("Could not get a FileTarget from " + target.GetType());
                }

                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                fileName = fileTarget.FileName.Render(logEventInfo);
            }
            else
            {
                throw new Exception("LogManager contains no Configuration or there are no named targets");
            }

            /*  if (!File.Exists(fileName))
              {
                  throw new Exception("File " + fileName + " does not exist");
              }*/

            return fileName;
        }
    }
}