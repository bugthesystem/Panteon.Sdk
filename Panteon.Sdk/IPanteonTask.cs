using System;
using Panteon.Sdk.Models;

namespace Panteon.Sdk
{
    public interface IPanteonTask
    {
        string Name { get; }
        bool Bootstrap(bool startImmediately);
        bool Stop();
        bool Start(DateTimeOffset lastKnownEvent = default(DateTimeOffset));
        void Pause(TimeSpan duration);
        PanteonTaskInfo Inspect();
        void Progress(ProgressMessage message);
    }
}