using System;

namespace Panteon.Sdk.Models
{
    public class PanteonTaskInfo
    {
        public string Name { get; set; }
        public bool IsScheduleRunning { get; set; }
        public DateTimeOffset NextEvent { get; set; }
        public DateTimeOffset PrevEvent { get; set; }
        public decimal Progress { get; set; }
    }
}