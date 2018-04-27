using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Core.Services
{
    public static class GalaxyServiceDirectory
    {
        const int SVC_BASE_PORT = 9000;
        static int i = 0;

        public static readonly int DIRECTORY_SVC_PORT = SVC_BASE_PORT + i++;
        public static readonly int AUTH_SVC_PORT = SVC_BASE_PORT + i++;
    }
}
