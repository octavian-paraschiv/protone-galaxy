#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using OPMedia.Core;
using System.Collections.Concurrent;
using OPMedia.Core.Utilities;
#endregion

namespace OPMedia.Core.Logging
{
    #region Logger
    public static class Logger
    {

        #region Methods
        

        public static void LogTranslationTrace(string format, params object[] args)
        {
#if HAVE_TRACE_UNTRANSLATED
            string message = string.Format(format, args);
            Debug.WriteLine("UNTRANSLATED: " + message);
            Debug.Flush();
#endif
        }

        public static void LogHelpTrace(string format, params object[] args)
        {
#if HAVE_TRACE_HELP
            string message = string.Format(format, args);
            Debug.WriteLine("HELP: " + message);
            Debug.Flush();
#endif
        }

        public static void LogTrace_WithStackDump(string format, params object[] args)
        {
            StackTrace st = new StackTrace();
            string message = string.Format(format, args);
            message += st.ToString();

            string[] lines = message.Split('\n');
            foreach (string line in lines)
            {
                Debug.WriteLine("TRC: " + line);
            }
        }

        public static void LogTrace(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Debug.WriteLine("TRC: " + message);
        }

        public static void LogInfo(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Debug.WriteLine("INF: " + message);
        }

        public static void LogWarning(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Debug.WriteLine("WRN: " + message);
        }

        public static void LogError(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Debug.WriteLine("ERR: " + message);
        }

        public static void LogException(Exception ex, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Debug.WriteLine("EXC: " + ErrorDispatcher.GetErrorMessageForException(ex, true));
        }
        
        public static void LogException(Exception ex)
        {
            LogException(ex, string.Empty);
        }

        public static void LogTimeDifference(string text, DateTime start)
        {
            int diff = (int)DateTime.Now.Subtract(start).TotalMilliseconds;
            Logger.LogTrace("{0}: operation took {1} msec", text, diff);
        }
        #endregion
    }
    #endregion
}
