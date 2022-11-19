using Azure;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace HttpServer.ServerLogic
{
    class ResponseBuilder
    {

        private HttpListenerResponse _response;
        public HttpListenerResponse Response
        {
            get
            {
                if (_response == null)
                    throw new Exception("The request has already been sent.");
                return _response;
            }
        }

        private byte[]? _buffer;
        public ResponseBuilder(HttpListenerResponse response)
        {
            _response = response;
        }

        public ResponseBuilder NewResponse(HttpListenerResponse response)
        {
            _response = response;
            return this;
        }

        public ResponseBuilder SetObject(object o)
        {
            Response.ContentType = "application/json";
            _buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(o));

            return this;
        }

        public ResponseBuilder SetContent(
            byte[] buffer,
            string type)
        {
            Response.ContentType = type;
            _buffer = buffer;
            return this;
        }

        public ResponseBuilder SetContent(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            Response.ContentType = MimeMap.GetMimeType(Path.GetExtension(path));
            _buffer = File.ReadAllBytes(path);
            return this;
        }

        public async Task<ResponseBuilder> SetContentAsync(string path)
        {
            Response.ContentType = MimeMap.GetMimeType(Path.GetExtension(path));
            _buffer = await GetPageAsync(path);
            return this;
        }

        private async Task<byte[]> GetPageAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            using (FileStream fstream = File.OpenRead(path))
            {
                byte[] buffer = new byte[fstream.Length];
                await fstream.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public ResponseBuilder SetMessage(string message)
        {
            Response.ContentType = "text/plain;charset=UTF-8";
            _buffer = Encoding.UTF8.GetBytes(message);
            return this;
        }

        public ResponseBuilder SetNotFoundMessage()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.ContentType = "text/plain;charset=UTF-8";
            _buffer = Encoding.UTF8.GetBytes("Ресурс не найден!");
            return this;
        }
        public ResponseBuilder SetRedirect(string url)
        {
            Response.StatusCode = 303;
            Response.Redirect(url);
            return this;
        }

        public ResponseBuilder SetCookie(Cookie cookie)
        {
            Response.Cookies.Add(cookie);
            return this;
        }

        public ResponseBuilder SetStatusCode(int code)
        {
            Response.StatusCode = code;
            return this;
        }
        public async Task SendAsync()
        {
            if (_buffer != null)
            {
                Response.ContentLength64 = _buffer.Length;
                var output = Response.OutputStream;
                await output.WriteAsync(_buffer, 0, _buffer.Length);
            }
            Response.Close();
            _buffer = null;
        }

    }
}
