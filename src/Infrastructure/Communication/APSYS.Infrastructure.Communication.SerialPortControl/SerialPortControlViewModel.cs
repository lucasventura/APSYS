namespace APSYS.Infrastructure.Communication.SerialPortControl
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Core.MVVM;
    using Domain;
    using Domain.Serial;
    using Domain.Utils;
    using NLog;

    public class SerialPortControlViewModel : BaseViewModel<SerialPortControlView>
    {
        private SerialPortService _serialPortService;
        private Logger _logger;
        private Logger _loggerData;
        private string _serialPortName;
        private int _baudRate;

        // public SerialPortControlViewModel()
        public SerialPortControlViewModel(SerialPortService serialPortService)
        {
            _serialPortService = serialPortService;
            _logger = LogManager.GetLogger("logFileRule");

            SerialPorts = new ObservableCollection<string>();
            var ports = SerialPortUtil.AvaliablesPorts();
            foreach (string port in ports)
            {
                _logger.Info("Serial Port {0} added", port);
                SerialPorts.Add(port);
            }

            BaudRates = new ObservableCollection<int> { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
            BaudRate = BaudRates.FirstOrDefault(a => a == 115200);
            SerialPortName = SerialPorts.FirstOrDefault();
        }

        public string SerialButtonText
        {
            get
            {
                if (_serialPortService == null || _serialPortService.Status == SerialPortService.SerialPortStatus.Close)
                {
                    return "Open Port";
                }

                return string.Format("Close Port ({0})", SerialPortName);
            }
        }

        public ICommand SerialPortOnOffCommand
        {
            get { return new RelayCommand(SerialPortOnOff); }
        }

        public bool SerialClosed
        {
            get
            {
                if (_serialPortService == null)
                {
                    return true;
                }

                return !_serialPortService.IsOpen;
            }
        }

        public ObservableCollection<string> SerialPorts { get; set; }
        public ObservableCollection<int> BaudRates { get; set; }

        public string SerialPortName
        {
            get
            {
                return _serialPortName;
            }

            set
            {
                _serialPortName = value;
                RaisePropertyChanged("SerialPortName");
            }
        }

        public int BaudRate
        {
            get
            {
                return _baudRate;
            }

            set
            {
                _baudRate = value;
                RaisePropertyChanged("BaudRate");
            }
        }

        private void SerialPortOnOff()
        {
            GlobalDiagnosticsContext.Set("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            _loggerData = LogManager.GetLogger("logDataRule");
            if (_serialPortService == null || _serialPortService.Status == SerialPortService.SerialPortStatus.Close)
            {
                OpenSerialPort();
                return;
            }

            if (_serialPortService.Status == SerialPortService.SerialPortStatus.Open)
            {
                CloseSerialPort();

                string dataLog;
                while (_serialPortService.DataEnqueue.TryDequeue(out dataLog))
                {
                    _loggerData.Info(dataLog);
                }
            }
        }

        private bool OpenSerialPort()
        {
            if (SerialPortName == null)
            {
                MessageBox.Show("Please, select a Serial Port and set the Baudrate.", "Serial Port Open Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                _serialPortService.InitializeSerialPort(SerialPortName, BaudRate);
                RaisePropertyChanged("SerialButtonText");
                RaisePropertyChanged("SerialClosed");
                _logger.Info("Serial Port {0} - {1} opened", SerialPortName, BaudRate);
                return true;
            }
            catch (Exception exception)
            {
                string messageText = string.Format("Couldn't open the serial port`{0}. The reason is described below.\n\"{1}\"", SerialPortName, exception.Message);
                MessageBox.Show(messageText, "Serial Port Open Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(messageText);
                return false;
            }
        }

        private void CloseSerialPort()
        {
            try
            {
                _serialPortService.Close();
                _logger.Info("Serial Port {0} - {1} closed", SerialPortName, BaudRate);
            }
            catch (Exception exception)
            {
                string messageText = string.Format("Couldn't close the serial port`{0}. The reason is described below.\n\"{1}\"", SerialPortName, exception.Message);
                MessageBox.Show(messageText, "Serial Port Close Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(messageText);
            }

            RaisePropertyChanged("SerialButtonText");
            RaisePropertyChanged("SerialClosed");
        }
    }
}
