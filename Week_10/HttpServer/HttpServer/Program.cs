using HttpServer.Logger;
using HttpServer.ServerLogic;
using Microsoft.Win32;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(ConsoleHandler.logger);
            ConsoleHandler.Run(server);
        }
    }
}
