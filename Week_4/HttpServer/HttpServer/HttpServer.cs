using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class HttpServer
    {
        private readonly string _mainUrl;
        private readonly string _googleUrl;

        private readonly HttpListener _listener;
        private HttpListenerContext _context;
        private bool isRunning;

        private readonly string googlePath;
        private readonly string helloPath;

        public HttpServer(string listeningUrl)
        {
            _listener = new HttpListener();
            _mainUrl = listeningUrl;
            _googleUrl = Path.Combine(_mainUrl, "google/");

            googlePath = @"..\..\..\res\google.html";
            helloPath = @"..\..\..\res\hello.html";

            _listener.Prefixes.Add(_mainUrl);
            _listener.Prefixes.Add(_googleUrl);
        }

        public void Start()
        {
            if (isRunning)
            {
                Console.WriteLine("Сервер уже запущен!");
                return;
            }

            Console.WriteLine("Запуск сервера...");
            _listener.Start();
            Console.WriteLine("Сервер запущен");
            isRunning = true;

            Receive();
        }

        public void Stop()
        {
            if (!isRunning)
            {
                Console.WriteLine("Сервер еще не запущен!");
                return;
            }
            Console.WriteLine("Остановка сервера...");
            _listener.Stop();
            Console.WriteLine("Сервер остановлен");
            isRunning = false;
        }

        public void Restart()
        {
            if (isRunning)
                Stop();
            Thread.Sleep(300);
            Start();
        }

        private void Receive()
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
                    var pagePath = ChoisePath(request);

                    var response = _context.Response;

                    var buffer = await GetPageAsync(pagePath);

                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    await output.WriteAsync(buffer, 0, buffer.Length);
                    output.Close();
                    Console.WriteLine("Запрос обработан");

                    Receive();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GetContext прерван");
                    Thread.Sleep(100);
                    if (isRunning)
                        Stop();
                }
            }
        }

        private string ChoisePath(HttpListenerRequest request)
        {
            var reqUrl = AddSlash(request.Url.OriginalString);
            if (reqUrl == _mainUrl)
                return helloPath;
            else if (reqUrl == _googleUrl)
                return googlePath;
            else
                return @"..\..\..\res\blank.html";
        }

        private string AddSlash(string path)
        {
            if (path[path.Length - 1] != '/')
                path += '/';
            return path;
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
