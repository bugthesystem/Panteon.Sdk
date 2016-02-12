using System;

namespace Panteon.Sdk.History
{
    public class HistoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public DateTime DateCreated { get; set; }
    }
}