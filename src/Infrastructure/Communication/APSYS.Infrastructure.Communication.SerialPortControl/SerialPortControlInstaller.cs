namespace APSYS.Infrastructure.Communication.SerialPortControl
{
    using System.ComponentModel;
    using Autofac;
    using Core;
    using Core.Interface;

    public class SerialPortControlInstaller : IModuleInstaller
    {
        public void Install(ApsysContainer container)
        {
            container.Builder.RegisterType<SerialPortControlView>().Named<SerialPortControlView>("SerialPortControlView");
            container.Builder.RegisterType<SerialPortControlViewModel>().Named<SerialPortControlViewModel>("SerialPortControlViewModel");

            var instance = new Views("SerialPortControlView", "SerialPortControlViewModel");
            container.Builder.RegisterInstance(instance).Named<Views>("sa");
            container.Builder.RegisterInstance(instance).Named<Views>("sb");

            // container.Register(Component.For<Views>().Instance(new Views("GPSSimulatorView", "GPSSimulatorViewModel")).Named("sa"));
            // container.Register(Component.For<Views>().Instance(new Views("GPSSimulatorView", "GPSSimulatorViewModel")).Named("sb"));
        }
    }
}
