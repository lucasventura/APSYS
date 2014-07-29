namespace APSYS.Plant.SeedPatternIdentifier
{
    using System;
    using System.Text;
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
            /* if (_serialPortService == null || _serialPortService.DataEnqueue.Count < 1)
             {
                 return false;
             }*/

            return true;
        }

        private void Save()
        {
            StringBuilder stb = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < 100; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    var rnd = random.Next(0, 1000);
                    stb.Append(j);
                    stb.Append(";");
                    stb.Append(rnd);
                    stb.AppendLine(";");
                }
            }

            string logData = stb.ToString();
            var planter = Planter.Verify(logData);

            /*_serialPortService.Close();
            string message = string.Empty;
            while (_serialPortService.DataEnqueue.TryDequeue(out message))
            {
                _logger.Info(message);
            }*/
        }
    }
}