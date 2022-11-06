using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class ServerSettings
    {
        public int Port { get; set; } = 8888;
        public string DataDirectoty { get; set; } = "site";
    }
}
