using System;
using System.Net;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace HttpServer
{
    class Program
    {
        static bool _appIsRunning = true;
        static readonly string commandsInfo = "Введите команду:\n" +
                "1. start - запустить сервер\n" +
                "2. stop - остановить сервер\n" +
                "3. restart - перезапустить сервер\n" +
                "4. status - показать статус сервера\n" +
                "5. exit - завершить выполнение программы\n";

        static void Main(string[] args)
        {
            var server = new HttpServer();

            Console.WriteLine(commandsInfo);
            while (_appIsRunning)
            {
                HandleConsole(Console.ReadLine().ToLower(), server);
            }
            Console.WriteLine("Программа завершена.");

        }

        static void HandleConsole(string command, HttpServer server)
        {
            Console.Clear();
            switch (command)
            {
                case ("start"):
                    Console.WriteLine(commandsInfo);
                    server.Start();
                    break;

                case ("stop"):
                    Console.WriteLine(commandsInfo);
                    server.Stop();
                    break;

                case ("restart"):
                    Console.WriteLine(commandsInfo);
                    server.Restart();
                    break;

                case ("status"):
                    Console.WriteLine(commandsInfo);
                    Console.WriteLine(server.Status);
                    break;

                case ("exit"):
                    _appIsRunning = false;
                    break;

                default:
                    Console.WriteLine(commandsInfo);
                    Console.WriteLine("Команда не найдена!");
                    break;
            }
        }
    }
}
