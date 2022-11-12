using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpServer.ServerLogic
{
    class ServerSettings
    {
        public int Port { get; set; } = 8888;
        public string DataDirectoty { get; set; } = "site";

        public static ServerSettings ReadFromJson(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);
            var buffer = File.ReadAllBytes(path);
            var settings = JsonSerializer.Deserialize<ServerSettings>(buffer);
            if (settings == null)
                throw new ArgumentException("Не удалось считать файл.");
            return settings;
        }
    }
}
