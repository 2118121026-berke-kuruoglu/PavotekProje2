using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapTileDownloader
{
    public class ProgressNotifierClient : IDisposable
    {
        private readonly ClientWebSocket _webSocket;
        private readonly Uri _serverUri;
        private bool _connected = false;

        public ProgressNotifierClient(string url = "ws://localhost:5000/ws/")
        {
            _webSocket = new ClientWebSocket();
            _serverUri = new Uri(url);
        }

        public async Task ConnectAsync()
        {
            if (_connected) return;

            try
            {
                await _webSocket.ConnectAsync(_serverUri, CancellationToken.None);
                _connected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket bağlantı hatası: {ex.Message}");
            }
        }

        public async Task SendProgressAsync(string mapName, double progress)
        {
            if (!_connected || _webSocket.State != WebSocketState.Open) return;

            var message = new
            {
                map = mapName,
                progress = Math.Round(progress, 1),
                time = DateTime.UtcNow.ToString("o")
            };

            string json = System.Text.Json.JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task CloseAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client kapatıldı", CancellationToken.None);
            }

            _connected = false;
        }

        public void Dispose()
        {
            _webSocket?.Dispose();
        }
    }
}
