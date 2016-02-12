using System;
using System.Collections.Generic;
using Panteon.Sdk.History;
using Panteon.Sdk.Models;

namespace Panteon.Sdk
{
    public interface IPanteonWorker
    {
        string Name { get; }
        bool Init(bool autoRun);
        bool Stop();
        bool Start(DateTimeOffset lastKnownEvent = default(DateTimeOffset));
        void Pause(TimeSpan duration);
        bool Update(ScheduleInfo scheduleInfo);

        PanteonTaskInfo Inspect();
        void Progress(ProgressMessage message);

        IEnumerable<HistoryModel> LoadHistory(DateTime? from = null, DateTime? to = null);
    }
}