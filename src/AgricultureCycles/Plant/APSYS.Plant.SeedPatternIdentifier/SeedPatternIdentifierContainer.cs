namespace APSYS.Plant.SeedPatternIdentifier
{
    using System.ComponentModel;
    using Autofac;
    using Core;
    using Infrastructure.Communication.SerialPortControl;
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
            Builder.RegisterType<SerialPortControlView>();
            Builder.RegisterType<SerialPortControlViewModel>();

            var obj = new SerialPortControlInstaller();
            obj.Install(this);

            Builder.RegisterType<SeedPatternIdentifierView>();
            Builder.RegisterType<SeedPatternIdentifierViewModel>();
        }
    }
}