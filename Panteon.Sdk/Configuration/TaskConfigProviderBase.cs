using Autofac.Extras.NLog;

namespace Panteon.Sdk.Configuration
{
    public abstract class TaskConfigProviderBase<TSettings>
    {
        public ILogger Logger { get; set; }

        protected TaskConfigProviderBase(ILogger logger)
        {
            Logger = logger;
        }

        public abstract TSettings ParseSettings(string configFilePath = null);
    }
}
