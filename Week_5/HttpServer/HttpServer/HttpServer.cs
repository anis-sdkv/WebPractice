using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpServer
{
    public enum ServerStatus
    {
        Stopped,
        Started
    }
    class HttpServer
    {
        private readonly HttpListener _listener;
        private HttpListenerContext _context;

        private ServerSettings settings;

        public ServerStatus Status { get; private set; } = ServerStatus.Stopped;


        public HttpServer(int port = 8888)
        {
            _listener = new HttpListener();
        }

        public void Start()
        {
            if (Status == ServerStatus.Started)
            {
                Console.WriteLine("Сервер уже запущен!");
                return;
            }

            settings = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllBytes("settings.json"));
            _listener.Prefixes.Clear();
            _listener.Prefixes.Add($"http://localhost:{settings.Port}/");

            Console.WriteLine("Запуск сервера...");
            _listener.Start();
            Console.WriteLine("Сервер запущен");
            Status = ServerStatus.Started;

            Listen();
        }

        public void Stop()
        {
            if (Status == ServerStatus.Stopped)
            {
                Console.WriteLine("Сервер еще не запущен!");
                return;
            }

            Console.WriteLine("Остановка сервера...");
            _listener.Stop();
            Console.WriteLine("Сервер остановлен");
            Status = ServerStatus.Stopped;
            Thread.Sleep(300);
        }

        public void Restart()
        {
            if (Status == ServerStatus.Started)
                Stop();
            Start();
        }

        private void Listen()
        {
            _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
        }

        private async void ListenerCallback(IAsyncResult result)
        {
            if (_listener.IsListening)
            {
                try
                {
                    _context = _listener.EndGetContext(result);
                    var request = _context.Request;
                    var response = _context.Response;

                    Console.WriteLine(request.Url);
                    var path = settings.DataDirectoty + (request.Url.LocalPath == "/" ? "/index.html" : request.Url.LocalPath);

                    if (File.Exists(path))
                        SendContentAsync(response, path);
                    else
                        SendErrorMessageAsync(response);

                    Console.WriteLine("Запрос обработан");
                    Listen();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GetContext прерван");
                    Thread.Sleep(100);
                    if (Status == ServerStatus.Started)
                        Stop();
                }
            }
        }

        private async Task SendContentAsync(HttpListenerResponse response, string path)
        {
            var mimeType = MimeMap.GetMimeType(Path.GetExtension(path));
            response.Headers.Set("Content-Type", mimeType);

            var buffer = await GetPageAsync(path);
            Stream output = response.OutputStream;
            await output.WriteAsync(buffer);
            output.Close();
        }

        private async Task SendErrorMessageAsync(HttpListenerResponse response)
        {
            response.Headers.Set("Content-Type", "text/plain;charset=UTF-8");
            response.StatusCode = (int)HttpStatusCode.NotFound;

            var message = "Ресурс не найден!";
            var output = response.OutputStream;
            var buffer = Encoding.UTF8.GetBytes(message);

            await output.WriteAsync(buffer);
            output.Close();
        }

        private async Task<byte[]> GetPageAsync(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("Файл страницы не найден!");
            }

            using (FileStream fstream = File.OpenRead(path))
            {
                byte[] buffer = new byte[fstream.Length];
                await fstream.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
