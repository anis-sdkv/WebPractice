using HttpServer.Attributes;
using HttpServer.Logger;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HttpServer.ServerLogic
{
    class RequestHandler
    {
        public string DataDirectory { get; set; }
        public RequestHandler(string dataDir)
        {
            DataDirectory = dataDir;
        }

        public async Task Handle(HttpListenerContext context, ServerLogger logger)
        {
            logger.LogMessage($"Запрос получен: {context.Request.Url}");
            var request = context.Request;
            var response = context.Response;

            var path = DataDirectory + (request.Url.LocalPath != "/" ? request.Url.LocalPath : "/index.html");

            if (File.Exists(path))
            {
                var builder = await new ResponseBuilder(response).SetContentAsync(path);
                await builder.SendAsync();
            }
            else if (TryMethodHandle(request, response, out var builder))
            {
                await builder.SendAsync();
            }
            else
            {
                await new ResponseBuilder(response)
                    .SetNotFoundMessage()
                    .SendAsync();
            }
            logger.LogMessage($"Запрос обработан: {context.Request.Url}");
        }

        private bool TryMethodHandle(HttpListenerRequest request, HttpListenerResponse response, out ResponseBuilder builder)
        {
            builder = null;
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

            var methodURI = strParams.Length > 0 ? strParams[0] : "";
            var method = controller
                .GetMethods()
                .Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"
                        && Regex.IsMatch(methodURI, ((Attributes.HttpMethod)attr).MethodURI)))
                .FirstOrDefault();
            if (method == null) return false;

            if (request.HttpMethod == "POST")
                strParams = ParseBody(GetPostBody(request));

            var objParams = strParams
                .Cast<object>()
                .Concat(new[] { (object)response })
                .ToArray();

            object[] queryParams = method
                .GetParameters()
                .Select((p, i) => Convert.ChangeType(objParams[i], p.ParameterType))
                .ToArray();

            var result = method.Invoke(
                Activator.CreateInstance(controller),
                queryParams);
            builder = (ResponseBuilder)result;

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
                .Select(p => p.Replace("+", "").Split('=')[1])
                .ToArray();
        }

    }
}
