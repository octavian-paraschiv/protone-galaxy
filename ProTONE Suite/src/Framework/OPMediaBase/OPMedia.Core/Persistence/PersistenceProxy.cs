using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using OPMedia.Core.Logging;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;
using OPMedia.Core.Configuration;

namespace OPMedia.Core
{
    public class PersistenceProxy : IDisposable, IPersistenceService, IPersistenceNotification
    {
        protected readonly static PersistenceProxy _proxy = new PersistenceProxy();

        protected IPersistenceService _channel = null;

        protected string _persistenceContext = string.Empty;
        protected Guid _appId = Guid.NewGuid();

        private static int _unsuccesfulAttempts = 0;


        protected PersistenceProxy()
        {
            try
            {
                if (_unsuccesfulAttempts < 5)
                {
                    if (ActivatePersistenceService())
                        _unsuccesfulAttempts = 0;
                    else
                        _unsuccesfulAttempts++;
                }
            }
            catch
            {
            }

            try
            {
                _persistenceContext = WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                _persistenceContext = $"{Environment.UserDomainName}\\{Environment.UserName}";
            }

            Open();
        }

        private bool ActivatePersistenceService()
        {
            int attempts = 5;

            while(attempts > 0)
            {
                try
                {
                    ServiceController srv = new ServiceController(Constants.PersistenceServiceShortName);
                    if (srv.Status == ServiceControllerStatus.Running)
                        return true;

                    srv.Start();
                    Thread.Sleep(500);
                }
                catch(Exception ex)
                {
                    Logger.LogException(ex);
                }

                attempts--;
            }

            return false;
        }

        protected virtual void Open()
        {
            try
            {
                var myBinding = new WSDualHttpBinding();
                myBinding.MaxReceivedMessageSize = int.MaxValue;
                myBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;

                var instanceContext = new InstanceContext(this);
                var myEndpoint = new EndpointAddress("http://localhost/PersistenceService.svc");
                var myChannelFactory = new DuplexChannelFactory<IPersistenceService>(instanceContext, myBinding, myEndpoint);

                _channel = myChannelFactory.CreateChannel();

                var appId = _appId.ToString();
                _channel.Subscribe(appId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        protected void Abort()
        {
            try
            {
                ICommunicationObject channel = _channel as ICommunicationObject;
                if (channel != null)
                    channel.Abort();
            }
            catch { }
        }

        public void Dispose()
        {
            try
            {
                if (_channel != null)
                {
                    var appId = _appId.ToString();
                    _channel.Unsubscribe(appId);

                    ICommunicationObject channel = _channel as ICommunicationObject;
                    if (channel != null)
                        channel.Close();
                }
            }
            catch { }

            _channel = null;
        }

        static string BuildPersistenceId(bool includeAppName, string persistenceId)
        {
            if (includeAppName)
                return $"{ApplicationInfo.ApplicationName}_{persistenceId}";

            return persistenceId;
        }

        #region ReadObject

        public static T ReadObject<T>(bool includeAppName, string persistenceId, T defaultValue, bool usePersistenceContext = true)
        {
            string id = BuildPersistenceId(includeAppName, persistenceId);
            return ReadObject<T>(id, defaultValue, usePersistenceContext);
        }

        static TicToc _readTicToc = new TicToc("Persistence.Proxy.ReadObject");

        static int _readCount = 0;

        public static T ReadObject<T>(string persistenceId, T defaultValue, bool usePersistenceContext = true)
        {
            T retVal = defaultValue;

            try
            {
                _readTicToc.Tic();

                {
                    string content = usePersistenceContext ?
                        (_proxy as IPersistenceService).ReadObject(persistenceId, _proxy._persistenceContext) :
                        (_proxy as IPersistenceService).ReadObject(persistenceId, string.Empty);

                    _readCount++;

                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            if (typeof(T).IsSubclassOf(typeof(Enum)))
                            {
                                retVal = (T)Enum.Parse(typeof(T), content);
                            }

                            else if (typeof(T) == typeof(TimeSpan))
                            {
                                TimeSpan ts = TimeSpan.Parse(content);
                                retVal = (T)Convert.ChangeType(ts, typeof(T));
                            }

                            else if (typeof(T) == typeof(DateTime))
                            {
                                DateTime dt = DateTime.Parse(content);
                                retVal = (T)Convert.ChangeType(dt, typeof(T));
                            }

                            else
                            {
                                try
                                {
                                    retVal = (T)Convert.ChangeType(content, typeof(T));
                                }
                                catch (InvalidCastException)
                                {
                                    retVal = (T)Enum.Parse(typeof(T), content);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                            retVal = defaultValue;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
                retVal = defaultValue;
            }
            finally
            {
                _readTicToc.Toc();
            }

            return retVal;
        }

        string IPersistenceService.ReadObject(string persistenceId, string persistenceContext)
        {
            try
            {
                if (_channel != null) 
                    return _channel.ReadObject(persistenceId, persistenceContext);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Abort();
                Open();
            }

            return null;
        }
        #endregion

        #region SaveObject

        public static void SaveObject<T>(bool includeAppName, string persistenceId, T objectContent, bool usePersistenceContext = true)
        {
            string id = BuildPersistenceId(includeAppName, persistenceId);
            SaveObject<T>(id, objectContent, usePersistenceContext);
        }

        static TicToc _saveTicToc = new TicToc("Persistence.Proxy.SaveObject");

        public static void SaveObject<T>(string persistenceId, T objectContent, bool usePersistenceContext = true)
        {
            try
            {
                _saveTicToc.Tic();

                {
                    if (usePersistenceContext)
                        (_proxy as IPersistenceService).SaveObject(persistenceId, _proxy._persistenceContext, objectContent.ToString());
                    else
                        (_proxy as IPersistenceService).SaveObject(persistenceId, string.Empty, objectContent.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            finally
            {
                _saveTicToc.Toc();
            }
        }

        void IPersistenceService.SaveObject(string persistenceId, string persistenceContext, string objectContent)
        {
            try
            {
                if (_channel != null)
                    _channel.SaveObject(persistenceId, persistenceContext, objectContent);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Abort();
                Open();
            }
        }
        #endregion

        #region DeleteObject

        public static void DeleteObject(bool includeAppName, string persistenceId, bool usePersistenceContext = true)
        {
            string id = BuildPersistenceId(includeAppName, persistenceId);
            DeleteObject(id, usePersistenceContext);
        }

        static TicToc _deleteTicToc = new TicToc("Persistence.Proxy.DeleteObject", 5);

        public static void DeleteObject(string persistenceId, bool usePersistenceContext = true)
        {
            try
            {
                _deleteTicToc.Tic();

                {
                    if (usePersistenceContext)
                        (_proxy as IPersistenceService).DeleteObject(persistenceId, _proxy._persistenceContext);
                    else
                        (_proxy as IPersistenceService).DeleteObject(persistenceId, string.Empty);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            finally
            {
                _deleteTicToc.Toc();
            }
        }

        void IPersistenceService.DeleteObject(string persistenceId, string persistenceContext)
        {
            try
            {
                if (_channel != null)
                    _channel.DeleteObject(persistenceId, persistenceContext);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                Abort();
                Open();
            }
        }
        #endregion

        void IPersistenceService.Subscribe(string appId)
        {
            throw new NotSupportedException();
        }

        void IPersistenceService.Unsubscribe(string appId)
        {
            throw new NotSupportedException();
        }

        void IPersistenceNotification.Notify(ChangeType changeType, string persistenceId, string persistenceContext, string objectContent)
        {
            if (persistenceContext == null || persistenceContext == "*" || persistenceContext == _persistenceContext)
                ThreadPool.QueueUserWorkItem((c) => ThreadedNotify(changeType, persistenceId, persistenceContext, objectContent));
        }

        void ThreadedNotify(ChangeType changeType, string persistenceId, string persistenceContext, string objectContent)
        {
            AppConfig.OnSettingsChanged(changeType, persistenceId, persistenceContext, objectContent);
        }
    }
}
