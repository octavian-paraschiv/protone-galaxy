using System.Diagnostics;
using log4net;

namespace Galaxy.Core.Logging
{
    public class Log4NetTraceListener : TraceListener
    {
        ILog logger = LogManager.GetLogger(typeof(Log4NetTraceListener));

        public Log4NetTraceListener()
        {
        }
        
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            logger.Debug(message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                    logger.FatalFormat(format, args);
                    break;

                case TraceEventType.Error:
                    logger.ErrorFormat(format, args);
                    break;

                case TraceEventType.Warning:
                    logger.WarnFormat(format, args);
                    break;

                case TraceEventType.Information:
                    logger.InfoFormat(format, args);
                    break;

                default:
                    logger.DebugFormat(format, args);
                    break;

            }
        }
    }
}
