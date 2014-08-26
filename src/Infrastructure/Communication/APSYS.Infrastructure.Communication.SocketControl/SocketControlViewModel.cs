namespace APSYS.Infrastructure.Communication.SocketControl
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Core.MVVM;
    using Domain.Socket;

    public class SocketControlViewModel : BaseViewModel<SocketControlView>
    {
        private readonly UdpListenerService _udpListenerService;
        private string _socketAddress;
        private int _localPort;

        public SocketControlViewModel(UdpListenerService udpListenerService)
        {
            _udpListenerService = udpListenerService;
            SocketAddress = "192.168.1.104";
            LocalPort = 2390;
        }

        public string SocketAddress
        {
            get
            {
                return _socketAddress;
            }

            set
            {
                _socketAddress = value;
                RaisePropertyChanged("SocketAddress");
            }
        }

        public int LocalPort
        {
            get
            {
                return _localPort;
            }

            set
            {
                _localPort = value;
                RaisePropertyChanged("LocalPort");
            }
        }

        public string SocketButtonText
        {
            get
            {
                if (_udpListenerService.IsConnected)
                {
                    return "Disconnect";
                }

                return "Connect";
            }
        }

        public ICommand SocketOnOffCommand
        {
            get
            {
                return new RelayCommand(SocketOnOff);
            }
        }

        private void SocketOnOff()
        {
            if (_udpListenerService == null || !_udpListenerService.IsConnected)
            {
                OpenSocket();
                return;
            }

            if (_udpListenerService.IsConnected)
            {
                CloseSocket();
            }
        }

        private bool OpenSocket()
        {
            if (LocalPort <= 0)
            {
                MessageBox.Show("Please, set the LocalPort.", "Socket Open Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                _udpListenerService.InitializeSocket(SocketAddress, LocalPort);
                RaisePropertyChanged("SocketButtonText");
                // RaisePropertyChanged("SerialClosed");
                // _logger.Info("Serial Port {0} - {1} opened", SerialPortName, BaudRate);
                return true;
            }
            catch (Exception exception)
            {
                string messageText = string.Format("Couldn't open the Socket. The reason is described below.\n\"{0}\"", exception.Message);
                MessageBox.Show(messageText, "Socket Open Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // _logger.Error(messageText);
                return false;
            }
        }

        private void CloseSocket()
        {
            try
            {
                _udpListenerService.Close();
                // _logger.Info("Serial Port {0} - {1} closed", SerialPortName, BaudRate);
            }
            catch (Exception exception)
            {
                string messageText = string.Format("Couldn't close the Socket. The reason is described below.\n\"{0}\"", exception.Message);
                MessageBox.Show(messageText, "Socket Close Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // _logger.Error(messageText);
            }

            RaisePropertyChanged("SocketButtonText");
            // RaisePropertyChanged("SerialClosed");
        }
    }
}
