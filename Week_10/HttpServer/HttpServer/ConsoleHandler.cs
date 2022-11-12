using HttpServer.Logger;
using HttpServer.ServerLogic;

namespace HttpServer
{
    class ConsoleHandler
    {
        public static bool IsRunning = true;
        public static readonly ConsoleLogger logger = new ConsoleLogger();

        public static void Run(Server server)
        {
            logger.LogMessage(ConsoleLogger.CONSOLE_COMMANDS);
            while (IsRunning)
                HandleConsole(Console.ReadLine().ToLower(), server);

        }
        static void HandleConsole(string command, Server server)
        {
            Console.Clear();
            logger.LogMessage(ConsoleLogger.CONSOLE_COMMANDS);
            switch (command)
            {
                case ("start"):
                    server.Start();
                    break;

                case ("stop"):
                    server.Stop();
                    break;

                case ("restart"):
                    server.Restart();
                    break;

                case ("status"):
                    logger.LogMessage(server.Status.ToString());
                    break;

                case ("exit"):
                    IsRunning = false;
                    Console.Clear();
                    logger.LogMessage(ConsoleLogger.AT_TERMINATION);
                    break;

                default:
                    logger.LogError(ConsoleLogger.COMMAND_NOT_FOUND);
                    break;
            }
        }
    }
}
