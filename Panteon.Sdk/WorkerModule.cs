using Autofac;
using Autofac.Extras.NLog;
using NLog;
using ILogger = Autofac.Extras.NLog.ILogger;

namespace Panteon.Sdk
{
    public class WorkerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new LoggerAdapter(LogManager.GetCurrentClassLogger())).As<ILogger>().SingleInstance();

            base.Load(builder);
        }
    }
}