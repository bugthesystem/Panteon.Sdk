namespace Panteon.Sdk.Utils
{
    public interface IEnvironmentWrapper
    {
        string GetMachineName();
        string GetMachineIp();
        string GetOperatingSystemVersion();
    }
}