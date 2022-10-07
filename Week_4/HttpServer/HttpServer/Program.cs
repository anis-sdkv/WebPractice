using System;
using System.Net;
using System.IO;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var message = "Введите команду:\n" +
                "1. start - запустить сервер\n" +
                "2. stop - остановить сервер\n" +
                "3. restart - перезапустить сервер\n";
            var url = "http://localhost:8888/connection/";


            var server = new HttpServer(url);
            Console.WriteLine(message);
            while (true)
            {
                var choise = Console.ReadLine().ToLower();
                Console.Clear();
                switch (choise)
                {
                    case ("start"):
                        Console.WriteLine(message);
                        server.Start();
                        break;
                    case ("stop"):
                        Console.WriteLine(message);
                        server.Stop();
                        break;
                    case ("restart"):
                        Console.WriteLine(message);
                        server.Restart();
                        break;
                    default:
                        Console.WriteLine("Команда не найдена!\n" + message);
                        break;
                }
            }
        }
    }
}
