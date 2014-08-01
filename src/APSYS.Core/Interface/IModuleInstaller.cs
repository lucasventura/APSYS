namespace APSYS.Core.Interface
{
    public class Views
    {
        public Views(string view, string viewModel)
        {
            View = view;
            ViewModel = viewModel;
        }

        public string View { get; set; }

        public string ViewModel { get; set; }
    }

    public interface IModuleInstaller
    {
        void Install(ApsysContainer container);
    }
}
