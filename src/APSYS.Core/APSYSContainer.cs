namespace APSYS.Core
{
    using Autofac;
    using Autofac.Core;
    using Infrastructure.Communication.Domain;
    using Infrastructure.Communication.Domain.Serial;
    using MVVM;

    public abstract class ApsysContainer
    {
        public ApsysContainer()
        {
            Builder = new ContainerBuilder();

            AutomaticRegister();
        }

        public ContainerBuilder Builder { get; set; }

        public IContainer Container { get; set; }

        public abstract void RegisterServices();

        public void BuildContainer()
        {
            Container = Builder.Build();
        }

        private void AutomaticRegister()
        {
            // Builder.RegisterType<SerialPortService>().As<ICommunicationOBC>();
            Builder.RegisterType<SerialPortService>().SingleInstance();
            Builder.RegisterType<WindowManager>().SingleInstance();
/*
            // Builder.RegisterModule<NLogModule>();
*/
        }
    }
}
