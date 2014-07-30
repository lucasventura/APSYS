namespace APSYS.Plant.SeedPatternIdentifier
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;
    using APSYS.Infrastructure.Communication.Domain.Serial;
    using NLog;
    using NLog.Layouts;
    using NLog.Targets;
    using NLog.Targets.Wrappers;
    using UI.Shared;

    public class SeedPatternIdentifierViewModel : BaseViewModel
    {
        private SerialPortService _serialPortService;
        private Logger _logger;
        private string _serialPort;
        private int _baudRate;

        public SeedPatternIdentifierViewModel()
        {
            /*
                        _logger = LogManager.GetLogger("logDataRule");
                        GlobalDiagnosticsContext.Set("StartTime", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            */
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