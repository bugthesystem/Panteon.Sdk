using System.IO;
using System.Reflection;
using Autofac.Extras.NLog;
using Panteon.Sdk.IO;
using Panteon.Sdk.Serialization;

namespace Panteon.Sdk.Configuration
{
    public abstract class TaskConfigProviderBase<TSettings>
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IFileReader _fileReader;
        public ILogger Logger { get; set; }

        protected TaskConfigProviderBase(ILogger logger, IJsonSerializer jsonSerializer, IFileReader fileReader)
        {
            _jsonSerializer = jsonSerializer;
            _fileReader = fileReader;
            Logger = logger;
        }

        public virtual TSettings ParseSettings(string configFilePath = null)
        {
            string asmLocation = Assembly.GetExecutingAssembly().Location;

            string directoryName = Path.GetDirectoryName(asmLocation);

            if (!string.IsNullOrEmpty(directoryName))
            {
                configFilePath = configFilePath ?? Path.Combine(directoryName, "config.json");
            }

            FileContentResult result = _fileReader.ReadFileContent(configFilePath);

            TSettings settings = _jsonSerializer.Deserialize<TSettings>(result.Content);

            return settings;
        }
    }
}
