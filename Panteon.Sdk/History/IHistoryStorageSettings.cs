namespace Panteon.Sdk.History
{
    public interface IHistoryStorageSettings
    {
        string ConnectionString { get; set; }
        bool Enabled { get; set; }
    }
}