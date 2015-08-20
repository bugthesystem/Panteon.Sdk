namespace Panteon.Sdk.Models
{
    public class ProgressMessage
    {
        public string TaskName { get; set; }
        public string Message { get; set; }
        public decimal Percent { get; set; }
    }
}