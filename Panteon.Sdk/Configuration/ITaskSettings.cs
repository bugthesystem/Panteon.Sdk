namespace Panteon.Sdk.Configuration
{
    public interface ITaskSettings
    {
        bool TestMode { get; set; }
        string RedisConnectionString { get; set; }
        string SchedulePattern { get; set; }
        int DbNo { get; set; }
    }
}