namespace APSYS.Plant.SeedPatternIdentifier
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for SeedPatternIdentifierView.xaml
    /// </summary>
    public partial class SeedPatternIdentifierView : Window
    {
        public SeedPatternIdentifierView()
        {
            InitializeComponent();

            DataContext = new SeedPatternIdentifierViewModel();
        }
    }
}