namespace Panteon.Sdk.Configuration
{
    public class WorkerSettingsBase : IWorkerSettings
    {
        public bool Enabled { get; set; }
        public bool TestMode { get; set; }
        public string RedisConnectionString { get; set; }
        public string SchedulePattern { get; set; }
        public int DbNo { get; set; }
    }
}