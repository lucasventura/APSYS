namespace APSYS.Core
{
    using Autofac;
    using Autofac.Core;
    using Infrastructure.Communication.Domain;
    using Infrastructure.Communication.Domain.Serial;
    using Service;

    public abstract class ApsysContainer
    {
        public ContainerBuilder Builder { get; set; }

        public IContainer Container { get; set; }

        public ApsysContainer()
        {
            Builder = new ContainerBuilder();

            AutomaticRegister();
        }

        private void AutomaticRegister()
        {
            Builder.RegisterType<SerialPortService>().As<ICommunicationOBC>();
            Builder.RegisterType<WindowManager>().SingleInstance();
            // Builder.RegisterModule<NLogModule>();
        }

        public abstract void RegisterServices();

        public void BuildContainer()
        {
            Container = Builder.Build();
        }
    }
}
