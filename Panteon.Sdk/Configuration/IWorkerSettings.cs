namespace Panteon.Sdk.Configuration
{
    public interface IWorkerSettings
    {
        bool TestMode { get; set; }
        string RedisConnectionString { get; set; }
        string SchedulePattern { get; set; }
        int DbNo { get; set; }
    }
}