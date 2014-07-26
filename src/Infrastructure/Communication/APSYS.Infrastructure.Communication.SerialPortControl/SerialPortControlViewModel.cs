namespace APSYS.Infrastructure.Communication.SerialPortControl
{
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Windows;
	using System.Windows.Input;
	using APSYS.Infrastructure.Communication.Domain.Serial;
	using APSYS.Infrastructure.Communication.Domain.Utils;
	using NLog;
	using UI.Shared;

	public class SerialPortControlViewModel : BaseViewModel
	{
		private SerialPortService _serialPortService;
		private Logger _logger;
		private string _serialPort;
		private int _baudRate;

		public SerialPortControlViewModel()
		{
			// _logger = LogManager.GetLogger("logFile");
			_logger = LogManager.GetCurrentClassLogger();
	
			SerialPorts = new ObservableCollection<string>();
			var ports = SerialPortUtil.AvaliablesPorts();
			foreach (string port in ports)
			{
				_logger.Info("Serial Port {0} added", port);
				SerialPorts.Add(port);
			}

			BaudRates = new ObservableCollection<int> { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
			BaudRate = BaudRates.FirstOrDefault(a => a == 9600);
			SerialPort = SerialPorts.FirstOrDefault();
		}

		public string SerialButtonText
		{
			get
			{
				if (_serialPortService == null || _serialPortService.Status == SerialPortService.SerialPortStatus.Close)
				{
					return "Open Port";
				}

				return string.Format("Close Port ({0})", SerialPort);
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

		public string SerialPort
		{
			get
			{
				return _serialPort;
			}

			set
			{
				_serialPort = value;
				RaisePropertyChanged("SerialPort");
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
			if (_serialPortService == null)
			{
				if (!OpenSerialPort())
				{
					return;
				}

				return;
			}

			if (_serialPortService.Status == SerialPortService.SerialPortStatus.Close)
			{
				OpenSerialPort();
				return;
			}

			if (_serialPortService.Status == SerialPortService.SerialPortStatus.Open)
			{
				CloseSerialPort();
			}
		}

		private bool OpenSerialPort()
		{
			if (SerialPort == null)
			{
				MessageBox.Show("Please, select a Serial Port and set the Baudrate.", "Serial Port Open Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

			try
			{
				_serialPortService = new SerialPortService(SerialPort, BaudRate);
				RaisePropertyChanged("SerialButtonText");
				RaisePropertyChanged("SerialClosed");
				_logger.Info("Serial Port {0} - {1} opened", SerialPort, BaudRate);
				return true;
			}
			catch (Exception exception)
			{
				string messageText = string.Format("Couldn't open the serial port`{0}. The reason is described below.\n\"{1}\"", SerialPort, exception.Message);
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
				_logger.Info("Serial Port {0} - {1} closed", SerialPort, BaudRate);
			}
			catch (Exception exception)
			{
				string messageText = string.Format("Couldn't close the serial port`{0}. The reason is described below.\n\"{1}\"", SerialPort, exception.Message);
				MessageBox.Show(messageText, "Serial Port Close Error", MessageBoxButton.OK, MessageBoxImage.Error);
				_logger.Error(messageText);
			}

			RaisePropertyChanged("SerialButtonText");
			RaisePropertyChanged("SerialClosed");
		}
	}
}
