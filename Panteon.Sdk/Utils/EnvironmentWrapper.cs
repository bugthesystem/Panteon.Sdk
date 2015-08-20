using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Panteon.Sdk.Utils
{
    public class EnvironmentWrapper : IEnvironmentWrapper
    {
        private readonly Regex _ipRegex;

        public EnvironmentWrapper()
        {
            _ipRegex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
        }
        public string GetMachineName()
        {
            return Environment.MachineName;
        }

        public string GetMachineIp()
        {
            IPHostEntry hostAddresses = Dns.GetHostEntry(GetMachineName());

            foreach (string input in hostAddresses.AddressList
                .Select(ipAddress => ipAddress.ToString())
                .Where(input => _ipRegex.IsMatch(input)))
            {
                return input;
            }

            return "0.0.0.0";
        }

        public string GetOperatingSystemVersion()
        {
            return Environment.OSVersion.ToString();
        }
    }
}