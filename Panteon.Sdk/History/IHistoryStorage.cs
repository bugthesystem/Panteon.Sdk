using System;
using System.Collections.Generic;

namespace Panteon.Sdk.History
{
    public interface IHistoryStorage
    {
        bool Store(HistoryModel historyModel);
        IEnumerable<HistoryModel> Load(string name, DateTime? from = null, DateTime? to = null);
    }
}
