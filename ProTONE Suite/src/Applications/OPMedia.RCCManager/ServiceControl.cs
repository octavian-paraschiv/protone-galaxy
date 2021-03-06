﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using OPMedia.Core;
using OPMedia.Runtime.ServiceHelpers;
using OPMedia.Runtime.ProTONE;

namespace OPMedia.RCCManager
{
    public static class ServiceControl
    {
        public static void StopService()
        {
            try
            {
                ServiceController sc = new ServiceController(ProTONEConstants.RCCServiceShortName);
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                }
            }
            catch
            {
            }
        }

        public static void StartService()
        {
            try
            {
                ServiceController sc = new ServiceController(ProTONEConstants.RCCServiceShortName);
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                }
            }
            catch
            {
            }
        }

        public static void ReconfigureService()
        {
            try
            {
                ServiceController sc = new ServiceController(ProTONEConstants.RCCServiceShortName);
                sc.ExecuteCommand((int)ServiceCommand.Reconfigure);
            }
            catch
            {
            }
        }
    }
}
