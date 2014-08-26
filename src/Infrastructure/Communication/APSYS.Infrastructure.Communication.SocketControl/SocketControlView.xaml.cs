namespace APSYS.Infrastructure.Communication.SocketControl
{
    /// <summary>
    /// Interaction logic for SocketControlView.xaml
    /// </summary>
    public partial class SocketControlView
    {
        public SocketControlView(SocketControlViewModel socketControlViewModel)
        {
            InitializeComponent();

            DataContext = socketControlViewModel;
        }
    }
}
