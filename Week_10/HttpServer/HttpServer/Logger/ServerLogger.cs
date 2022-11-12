using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Logger
{
    abstract class ServerLogger
    {
        public static readonly string SERVER_ALREADY_STARTED = "Сервер уже запущен!";
        public static readonly string SERVER_ON_START = "Запуск сервера...";
        public static readonly string SERVER_AFTER_START = "Сервер запущен";

        public static readonly string SERVER_NOT_STARTED = "Сервер еще не запущен!";
        public static readonly string SERVER_ON_STOP = "Остановка сервера...";
        public static readonly string SERVER_AFTER_STOP = "Сервер остановлен";

        public abstract void LogMessage(string message);
        public abstract void LogError(string error);
    }
}
