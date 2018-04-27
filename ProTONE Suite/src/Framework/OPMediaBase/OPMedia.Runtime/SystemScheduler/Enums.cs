using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using OPMedia.Core.Logging;
using OPMedia.Core.Utilities;
using OPMedia.Core;

namespace OPMedia.Runtime.SystemScheduler
{
    public enum SchedulerAction
    {
        None,
        Shutdown,
        StandBy,
        Hibernate,
    }

    [Flags]
    public enum Weekday
    {
        None = 0x00,
        Monday = 0x01,
        Tuesday = 0x02,
        Wednesday = 0x04,
        Thursday = 0x08,
        Friday = 0x10,
        Saturday = 0x20,
        Sunday = 0x40,
        Everyday = 0x7F,
    }
}
