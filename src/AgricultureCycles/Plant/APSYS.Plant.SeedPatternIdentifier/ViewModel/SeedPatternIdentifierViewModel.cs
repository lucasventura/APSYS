namespace APSYS.Plant.SeedPatternIdentifier.ViewModel
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Core.MVVM;
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
        private Logger _logger;
        private string _serialPort;
        private int _baudRate;
        private DispatcherTimer _dispatcherTimerCalibration;

        public SeedPatternIdentifierViewModel(SerialPortService serialPortService, SerialPortControlView serialPortControlView)
        {
            SerialPortControl = serialPortControlView;
            _serialPortService = serialPortService;
            _logger = LogManager.GetLogger("logDataRule");
            GlobalDiagnosticsContext.Set("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        }

        public SerialPortControlView SerialPortControl { get; set; }

        public ICommand SaveCommand
        {
            get { return new RelayCommand(Save, CanSave); }
        }

        public ICommand PatternIdentifierCommand
        {
            get { return new RelayCommand(PatternIdentifier, CanPatternIdentifier); }
        }

        private bool CanPatternIdentifier()
        {
            return _serialPortService.IsOpen && !_serialPortService.IsCalibration;
        }

        private void PatternIdentifier()
        {
            _serialPortService.IsCalibration = true;

            _dispatcherTimerCalibration = new DispatcherTimer();
            _dispatcherTimerCalibration.Interval = TimeSpan.FromSeconds(10);
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

            var seedTubeDataText = SeedTubeDataService.SplitPureTextToSeedTube(tst);
            var seedTubeDataReadings = SeedTubeDataService.MountSeedTubeDataReadings(seedTubeDataText);

            foreach (var seedTubeDataReading in seedTubeDataReadings)
            {
                var allSeedsGroupBySensorNumber = seedTubeDataReading.SeedTubeDataReadings.GroupBy(a => a.SensorNumber);

                foreach (var stdr in allSeedsGroupBySensorNumber)
                {
                    var maxValuePerSensor = stdr.Max(a => a.SensorValue);

                    SensorParameter sp = new SensorParameter()
                    {
                        SensorMaxValue = (int)(maxValuePerSensor * 1.1),
                        SensorNumber = stdr.Key
                    };

                    SeedTubeDataService.AddSensorParameter(sp);
                }
            }
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
                    var planter = PlanterService.Verify(str.ReadToEnd());
                }
                catch (Exception e)
                {
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