namespace APSYS.Plant.SeedPatternIdentifier
{
    using Autofac;
    using Core;
    using Infrastructure.Communication.SerialPortControl;
    using Infrastructure.Communication.SocketControl;
    using Service;
    using View;
    using ViewModel;

    public class SeedPatternIdentifierContainer : ApsysContainer
    {
        public SeedPatternIdentifierContainer()
        {
            Builder.RegisterInstance<ApsysContainer>(this);
        }

        public override void RegisterServices()
        {
            Builder.RegisterType<SeedPatternIdentifierView>();
            Builder.RegisterType<SeedPatternIdentifierViewModel>();

            Builder.RegisterType<SerialPortControlView>();
            Builder.RegisterType<SerialPortControlViewModel>();

            Builder.RegisterType<SocketControlView>();
            Builder.RegisterType<SocketControlViewModel>();

            Builder.RegisterType<SeedTubeDataService>().SingleInstance();
            Builder.RegisterType<PlanterService>().SingleInstance();
        }
    }
}