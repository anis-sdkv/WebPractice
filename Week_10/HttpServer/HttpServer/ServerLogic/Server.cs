using Azure.Core;
using HttpServer.Attributes;
using HttpServer.Logger;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime;

namespace HttpServer.ServerLogic
{
    class Server
    {
        private readonly HttpListener _listener;
        private readonly ServerLogger _logger;
        private RequestHandler handler;
        public ServerStatus Status { get; private set; } = ServerStatus.Stopped;

        public Server(ServerLogger logger)
        {
            _listener = new HttpListener();
            _logger = logger;
            var settings = ServerSettings.ReadFromJson("settings.json");
            _listener.Prefixes.Clear();
            _listener.Prefixes.Add($"http://localhost:{settings.Port}/");
            handler = new RequestHandler(settings.DataDirectoty);
        }

        public void Start()
        {
            if (Status == ServerStatus.Started)
            {
                _logger.LogMessage(ServerLogger.SERVER_ALREADY_STARTED);
                return;
            }

            _logger.LogMessage(ServerLogger.SERVER_ON_START);
            _listener.Start();
            _logger.LogMessage(ServerLogger.SERVER_AFTER_START);
            Status = ServerStatus.Started;

            Listen();
        }

        public void Stop()
        {
            if (Status == ServerStatus.Stopped)
            {
                _logger.LogMessage(ServerLogger.SERVER_NOT_STARTED);
                return;
            }
            _logger.LogMessage(ServerLogger.SERVER_ON_STOP);
            _listener.Stop();
            _logger.LogMessage(ServerLogger.SERVER_AFTER_STOP);
            Status = ServerStatus.Stopped;
        }

        public void Restart()
        {
            if (Status == ServerStatus.Started)
                Stop();
            Start();
        }

        private async Task Listen()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    await handler.Handle(context, _logger);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace + ex.Message);
                }
            }
        }
    }
}
