namespace APSYS.Plant.SeedPatternIdentifier.ViewModel
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Core.MVVM;
    using Domain;
    using Infrastructure.Communication.Domain.Serial;
    using Infrastructure.Communication.Domain.Socket;
    using Infrastructure.Communication.SerialPortControl;
    using Infrastructure.Communication.SocketControl;
    using NLog;
    using NLog.Targets;
    using NLog.Targets.Wrappers;
    using Service;
    using View;

    public class SeedPatternIdentifierViewModel : BaseViewModel<SeedPatternIdentifierView>
    {
        private readonly SerialPortService _serialPortService;
        private readonly UdpListenerService _udpListenerService;
        private readonly SeedTubeDataService _seedTubeDataService;
        private readonly PlanterService _planterService;
        private Logger _logger;
        private DispatcherTimer _dispatcherTimerCalibration;
        private bool _isLogEnable;
        private Logger _loggerData;

        public SeedPatternIdentifierViewModel(SerialPortService serialPortService, UdpListenerService udpListenerService, SerialPortControlView serialPortControlView, SeedTubeDataService seedTubeDataService, PlanterService planterService, SocketControlView socketControlView)
        {
            SerialPortControl = serialPortControlView;
            SocketControl = socketControlView;
            _serialPortService = serialPortService;
            _udpListenerService = udpListenerService;
            _udpListenerService.DataReceived += NewSocketData;
            _serialPortService.DataReceived += NewSerialData;
            _seedTubeDataService = seedTubeDataService;
            _planterService = planterService;
            _logger = LogManager.GetLogger("logDataRule");
            GlobalDiagnosticsContext.Set("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        }
        
        public SerialPortControlView SerialPortControl { get; set; }

        public SocketControlView SocketControl { get; set; }

        public ObservableCollection<SensorParameter> CalibrationParameters { get; set; }

        public bool IsCalibration { get; set; }

        public ConcurrentQueue<string> CalibrationDataEnqueue { get; set; }

        public bool IsLogEnable
        {
            get
            {
                return _isLogEnable;
            }

            set
            {
                _isLogEnable = value;

                _serialPortService.IsDataEnqueueEnable = _isLogEnable;

                RaisePropertyChanged("IsLogEnable");
            }
        }

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
            get { return new RelayCommand(Save, CanCleanQueue); }
        }

        public ICommand PatternIdentifierCommand
        {
            get { return new RelayCommand(PatternIdentifier, CanPatternIdentifier); }
        }

        public ICommand CleanQueueCommand
        {
            get { return new RelayCommand(CleanQueue, CanCleanQueue); }
        }

        public ICommand LastLogAnalyzeCommand
        {
            get { return new RelayCommand(LastLogAnalyze); }
        }

        public override void Initialize()
        {
            CalibrationParameters = new ObservableCollection<SensorParameter>();
            CalibrationDataEnqueue = new ConcurrentQueue<string>();
        }

        private bool CanCleanQueue()
        {
            return _serialPortService.DataEnqueue != null && _serialPortService.DataEnqueue.Count > 0;
        }

        private void CleanQueue()
        {
            ConcurrentQueue<string> newQueue = new ConcurrentQueue<string>();
            _serialPortService.DataEnqueue = newQueue;
        }

        private void LastLogAnalyze()
        {
            var fileName = GetLogFileName("logData");
            var directory = Directory.GetParent(fileName);

            var lastOrDefault = directory.EnumerateFiles().LastOrDefault();
            if (lastOrDefault != null)
            {
                try
                {
                    StreamReader str = new StreamReader(lastOrDefault.FullName);
                    var planter = _planterService.Verify(str.ReadToEnd());

                    /*  foreach (var seedTube in planter.SeedTubes)
                      {
                          var tubeDataReadingsWithSeed = seedTube.SeedTubeDataReadings.SelectMany(a => a.SeedTubeDataReadings).Where(b => b.HasSeed);

                          string messageBoxText = string.Format("Total: {0}", tubeDataReadingsWithSeed.Count());

                          MessageBox.Show(messageBoxText, "Results", MessageBoxButton.OK, MessageBoxImage.Information);
                      }*/

                    var stb = new StringBuilder();
                    stb.AppendLine(string.Format("Total de tubos de sementes: {0}\n", planter.SeedTubes.Count));

                    foreach (var seedTube in planter.SeedTubes)
                    {
                        stb.AppendLine(string.Format("SeedTube {0} - Total de Leituras: {1}", seedTube.SeedTubeNumber, seedTube.SeedTubeDataReadings.Count));

                        var dataReadingWithSeed = seedTube.SeedTubeDataReadings.SelectMany(b => b.SeedTubeDataReadings).Where(a => a.HasSeed).ToList();
                        var dataReadingWithNoSeed = seedTube.SeedTubeDataReadings.SelectMany(b => b.SeedTubeDataReadings).Where(a => !a.HasSeed);
                        stb.AppendLine(string.Format("Leituras com sementes {0}", dataReadingWithSeed.Count));
                        stb.AppendLine(string.Format("Leituras sem sementes {0}", dataReadingWithNoSeed.Count()));

                        var sensorsWithSeedCount = dataReadingWithSeed.GroupBy(a => a.SensorNumber).ToList();
                        var sensorsWithSeedCountTotal = sensorsWithSeedCount.SelectMany(a => a).ToList().Count();

                        foreach (var sensorGroup in sensorsWithSeedCount)
                        {
                            var count = sensorGroup.Count();
                            double percent = (double)(100 * count) / sensorsWithSeedCountTotal;

                            if (percent > 1)
                            {
                                stb.AppendLine(string.Format("Sensor {0} - Leituras com semente {1} - {2}%", sensorGroup.Key, count, percent));
                            }

                            var valueGroups = sensorGroup.GroupBy(a => a.SensorValue).ToList();
                            var valueGroupsCount = valueGroups.SelectMany(a => a).Count();
                            foreach (var valueGroup in valueGroups)
                            {
                                var countValue = valueGroup.Count();
                                double percentValue = (100 * countValue) / (double)valueGroupsCount;
                                if (percentValue > 0.8)
                                {
                                    stb.AppendLine(string.Format("Sensor {0} - Leituras com valor {1} - {2}- {3}%", sensorGroup.Key, valueGroup.Key, countValue, percentValue.ToString("f2")));
                                }
                            }
                        }
                    }

                    MessageBox.Show(stb.ToString(), "Results", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _logger.Error(e.Message);
                }
            }
            else
            {
                MessageBox.Show("Nenhum arquivo para analisar", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanPatternIdentifier()
        {
            return _serialPortService.IsOpen;
        }

        private void PatternIdentifier()
        {
            IsCalibration = true;

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

            IsCalibration = false;

            string tst = string.Empty;
            string dataLog;
            while (CalibrationDataEnqueue.TryDequeue(out dataLog))
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
                    SensorMaxValue = (int)(maxValuePerSensor * 1.8),
                    SensorNumber = seedTubeDatas.Key
                };

                _seedTubeDataService.AddSensorParameter(sp);
            }

            if (_seedTubeDataService.SensorParameters == null)
            {
                return;
            }

            CalibrationParameters.Clear();
            foreach (var sensorParameter in _seedTubeDataService.SensorParameters)
            {
                CalibrationParameters.Add(sensorParameter);
            }

            RaisePropertyChanged("CalibrationSettingsVisibility");
        }

        private void Save()
        {
            var islogEnableTemp = IsLogEnable;
            IsLogEnable = false;

            GlobalDiagnosticsContext.Set("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            _loggerData = LogManager.GetLogger("logDataRule");

            string dataLog;
            while (_serialPortService.DataEnqueue.TryDequeue(out dataLog))
            {
                _loggerData.Info(dataLog);
            }

            IsLogEnable = islogEnableTemp;
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

        private void NewSerialData(object sender, EventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string newData = sp.ReadExisting();

                if (IsCalibration)
                {
                    CalibrationDataEnqueue.Enqueue(newData);
                }

                /*if (IsDataEnqueueEnable)
                {
                    _serialPortService.DataEnqueue.Enqueue(newData);
                }*/
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Error(exception.Message);
            }
        }

        private void NewSocketData(object sender, EventArgs e)
        {
            // todo: Implementar retorno do socket
        }
    }
}