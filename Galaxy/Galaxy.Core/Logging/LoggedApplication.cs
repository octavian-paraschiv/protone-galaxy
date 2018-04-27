using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using GalaxyRuntime.InstanceManagement;
using System.IO;
using System.Security.AccessControl;
using System.Diagnostics;
using System.Windows;

namespace GalaxyRuntime.Logging
{
    public class LoggedApplication : GalaxyApplication
    {
        protected Mutex _appMutex = null;
        protected string _appMutexName = null;
        
        public new static void Start(string appName)
        {
            if (appInstance == null)
            {
                appInstance = new LoggedApplication();
                appInstance.Start(appName);
            }
            else
            {
                Trace.TraceError("Error encountered: {0}",
                    "Only one instance of GalaxyApplication (or derived) can be started per process !!");
            }
        }

        public new static void Stop()
        {
            if (appInstance != null)
            {
                appInstance.Stop();
            }
        }

        public new static void Restart()
        {
            Trace.TraceInformation("Application is restarting.");
            GalaxyApplication.Restart();
        }

        protected LoggedApplication()
        {
            string configFile = Path.ChangeExtension(Application.ResourceAssembly.Location, ".exe.config");
            FileInfo fi = new FileInfo(configFile);
            
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);
            log4net.GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            

            Trace.Listeners.Clear();
            Trace.Listeners.Add(new Log4NetTraceListener());
        }

        ~LoggedApplication()
        {
            Trace.TraceInformation("Application has finished.");
        }

        private void OnApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ErrorDispatcher.DispatchFatalError(e.Exception);
        }

        protected override void DoInitialize(string appName)
        {
            Trace.TraceInformation("Application is starting up...");

            _appMutexName = appName.Replace(" ", "").ToLowerInvariant() + @".mutex";

            RegisterAppMutex(appName);
        }

        protected virtual void RegisterAppMutex(string appName)
        {
            bool isNew = false;
            _appMutex = new Mutex(false, "Global\\" + _appMutexName, out isNew);
        }

        protected override void DoTerminate()
        {
            ReleaseAppMutex();
        }

        protected void ReleaseAppMutex()
        {
            if (_appMutex != null)
            {
                Trace.TraceInformation("Tring to release the app instance mutex ...");

                _appMutex.Close();
                _appMutex = null;

                Trace.TraceInformation("App instance mutex is released now");

            }
        }
    }
}
