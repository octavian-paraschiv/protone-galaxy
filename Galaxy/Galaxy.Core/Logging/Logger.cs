using Galaxy.Core.Logging;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Galaxy.Core.Logging
{
    public static class Logger
    {
        public static void Initialize()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            string configFile = Path.ChangeExtension(asm.Location, ".exe.config");
            FileInfo fi = new FileInfo(configFile);

            log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);
            log4net.GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;

            Trace.Listeners.Clear();
            Trace.Listeners.Add(new Log4NetTraceListener());

        }
    }
}
