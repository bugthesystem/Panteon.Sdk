using System;
using System.Collections.Generic;
using System.Linq;

namespace Panteon.Sdk.History
{
    public class NullHistoryStorage : IHistoryStorage
    {
        public bool Store(HistoryModel historyModel)
        {
            return true;
        }

        public IEnumerable<HistoryModel> Load(string name, DateTime? @from = null, DateTime? to = null)
        {
            return Enumerable.Empty<HistoryModel>();
        }
    }
}