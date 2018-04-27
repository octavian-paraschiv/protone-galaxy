using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPMedia.Runtime.ServiceHelpers;
using System.ServiceModel;
using OPMedia.Core;

using OPMedia.Core.Configuration;

namespace OPMedia.PersistenceService
{
    internal class ServiceHelper : ServiceHelperBase
    {
        ServiceHost _host = null;

        public ServiceHelper()
        {
           
        }

        protected override void StartInternal()
        {
            Environment.CurrentDirectory = AppConfig.InstallationPath;

            string address = "http://localhost/PersistenceService.svc";

            var binding = new WSDualHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            
            _host = new ServiceHost(typeof(PersistenceServiceImpl));
            _host.AddServiceEndpoint(typeof(IPersistenceService), binding, address);

            _host.Open();
        }

        protected override void StopInternal()
        {
            _host.Close();
            _host = null;
        }
    }
}
