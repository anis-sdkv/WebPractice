using Azure.Core;
using HttpServer.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttpMethod = HttpServer.Attributes.HttpMethod;

namespace HttpServer
{
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
                    var path = settings.DataDirectoty + (request.Url.LocalPath != "/" ? request.Url.LocalPath : "/index.html");

                    if (File.Exists(path))
                    {
                        await SendContentAsync(response, path);
                    }
                    else if (MethodHandler(request, out var methodBuffer, out var type))
                    {
                        if (request.HttpMethod == "GET")
                            await SendBufferAsync(response, methodBuffer, type);
                        else
                            SendRedirect(response, "https://store.steampowered.com/");
                    }
                    else
                    {
                        await SendErrorMessageAsync(response);
                    }

                    Console.WriteLine("Запрос обработан");
                    Listen();
                }
                catch
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

        private async Task SendBufferAsync(HttpListenerResponse response, byte[] buffer, string type)
        {
            response.ContentType = type;
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
        }

        private void SendRedirect(HttpListenerResponse response, string url)
        {
            response.StatusCode = 303;
            response.Redirect(url);
            response.Close();
        }

        private bool MethodHandler(HttpListenerRequest request, out byte[] buffer, out string type)
        {
            buffer = null;
            type = null;

            if (request.Url.Segments.Length < 2) return false;

            string controllerName = request.Url.Segments[1].Replace("/", "");

            string[] strParams = request.Url.Segments
                .Skip(2)
                .Select(s => s.Replace("/", ""))
                .ToArray();

            var assembly = Assembly.GetExecutingAssembly();

            var controller = assembly
                .GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(HttpController))
                    && ((t.GetCustomAttribute(typeof(HttpController)) as HttpController)?.ControllerName == controllerName
                    || t.Name.ToLower() == controllerName.ToLower()))
                    .FirstOrDefault();
            if (controller == null) return false;

            var methodURI = (strParams.Length > 0) ? strParams[0] : "";
            var method = controller
                .GetMethods()
                .Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"
                        && Regex.IsMatch(methodURI, ((HttpMethod)attr).MethodURI)))
                .FirstOrDefault();
            if (method == null) return false;

            if (request.HttpMethod == "POST")
                strParams = ParseBody(GetPostBody(request));

            object[] queryParams = method
                .GetParameters()
                .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                .ToArray();

            var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);

            if (request.HttpMethod == "GET")
            {
                buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
                type = "Application/json";
            }
            return true;
        }

        private static string GetPostBody(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
                return "";
            using (var stream = request.InputStream)
            {
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static string[] ParseBody(string body)
        {
            return body
                .Split('&')
                .Select(p => p.Replace("+","").Split('=')[1])
                .ToArray();
        }
    }
}
