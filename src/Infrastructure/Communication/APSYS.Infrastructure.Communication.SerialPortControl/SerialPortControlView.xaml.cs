﻿namespace APSYS.Infrastructure.Communication.SerialPortControl
{
    /// <summary>
    /// Interaction logic for SerialPortControlView.xaml
    /// </summary>
    public partial class SerialPortControlView
    {
        public SerialPortControlView(SerialPortControlViewModel serialPortControlViewModel)
        {
            InitializeComponent();

            DataContext = serialPortControlViewModel;
        }
    }
}
