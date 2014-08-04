namespace APSYS.Plant.SeedPatternIdentifier.View
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Interaction logic for SeedPatternIdentifierView.xaml
    /// </summary>
    public partial class SeedPatternIdentifierView
    {
        public SeedPatternIdentifierView()
        {
            InitializeComponent();

            Closing += ClosingHandler;
        }

        private void ClosingHandler(object sender, CancelEventArgs e)
        {
            /*if (MessageBox.Show("Deseja realmente fechar?", "Fechar", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }*/
        }
    }
}