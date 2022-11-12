using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Logger
{
    class ConsoleLogger : ServerLogger
    {
        public static readonly string CONSOLE_COMMANDS = "Введите команду:\n" +
                "1. start - запустить сервер\n" +
                "2. stop - остановить сервер\n" +
                "3. restart - перезапустить сервер\n" +
                "4. status - показать статус сервера\n" +
                "5. exit - завершить выполнение программы\n";
        public static readonly string COMMAND_NOT_FOUND = "Команда не найдена!";
        public static readonly string AT_TERMINATION = "Программа завершена";

        public override void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        public override void LogError(string error)
        {
            Console.WriteLine($"Error: {error}");
        }
    }
}
