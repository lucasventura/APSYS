namespace APSYS.Plant.SeedPatternIdentifier
{
    using System;
    using System.Windows.Input;
    using Infrastructure.Communication.Serial;
    using Infrastructure.Communication.Utils;
    using NLog;
    using UI.Shared;

    public class SeedPatternIdentifierViewModel : BaseViewModel
    {
        private bool _isInitialized = false;
        private SerialPortService _serialPortService;
        private Logger _logger;

        public SeedPatternIdentifierViewModel()
        {
            // _logger = LogManager.GetLogger("file");
            _logger = LogManager.GetCurrentClassLogger();
        }

        public ICommand InitCommand
        {
            get { return new RelayCommand(Init, CanInit); }
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

            _isInitialized = false;
        }

        private void Init()
        {
            try
            {
                GlobalDiagnosticsContext.Set("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));

                var ports = SerialPortUtil.AvaliablesPorts();

                _logger.Info("OPA");

                foreach (var port in ports)
                {
                    _logger.Info(port);
                }

                _serialPortService = new SerialPortService("COM4", 115200);
                if (_serialPortService.IsOpen)
                {
                    _isInitialized = true;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        private bool CanInit()
        {
            if (_isInitialized)
            {
                return false;
            }

            return true;
        }
    }
}