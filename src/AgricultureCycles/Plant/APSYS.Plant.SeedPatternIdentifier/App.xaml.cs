namespace APSYS.Plant.SeedPatternIdentifier
{
    using System.Windows;
    using Autofac;
    using Core;
    using Core.MVVM;
    using View;
    using ViewModel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ApsysContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application"/> class is created per <see cref="T:System.AppDomain"/>.</exception>
        public App()
        {
            _container = new SeedPatternIdentifierContainer();

            _container.RegisterServices();

            _container.BuildContainer();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var windowManager = _container.Container.Resolve<WindowManager>();

            windowManager.ShowWindow<SeedPatternIdentifierView, SeedPatternIdentifierViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
