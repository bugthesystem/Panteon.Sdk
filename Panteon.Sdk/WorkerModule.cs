using Autofac;
using Autofac.Extras.NLog;
using NLog;
using Panteon.Sdk.IO;
using ILogger = Autofac.Extras.NLog.ILogger;

namespace Panteon.Sdk
{
    public class WorkerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new LoggerAdapter(LogManager.GetCurrentClassLogger())).As<ILogger>().SingleInstance();

            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            builder.RegisterType<FileReader>().As<IFileReader>().SingleInstance();

            base.Load(builder);
        }
    }
}