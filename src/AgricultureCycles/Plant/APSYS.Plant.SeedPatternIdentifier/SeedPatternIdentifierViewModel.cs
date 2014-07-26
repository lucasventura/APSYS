namespace APSYS.Plant.SeedPatternIdentifier
{
	using System;
	using System.Windows.Input;
	using APSYS.Infrastructure.Communication.Domain.Serial;
	using NLog;
	using UI.Shared;

	public class SeedPatternIdentifierViewModel : BaseViewModel
	{
		private SerialPortService _serialPortService;
		private Logger _logger;
		private string _serialPort;
		private int _baudRate;

		public SeedPatternIdentifierViewModel()
		{
			_logger = LogManager.GetLogger("logData");
			GlobalDiagnosticsContext.Set("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
		}

		public ICommand SaveCommand
		{
			get { return new RelayCommand(Save, CanSave); }
		}

		private bool CanSave()
		{
			if (_serialPortService == null || _serialPortService.DataEnqueue.Count < 1)
			{
				return false;
			}

			return true;
		}

		private void Save()
		{
			_serialPortService.Close();
			string message = string.Empty;
			while (_serialPortService.DataEnqueue.TryDequeue(out message))
			{
				_logger.Info(message);
			}
		}
	}
}