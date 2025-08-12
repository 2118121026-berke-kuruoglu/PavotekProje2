using System.Net;
using System.Net.WebSockets;
using System.Text;

var listener = new HttpListener();
listener.Prefixes.Add("http://localhost:5000/ws/");
listener.Start();
Console.WriteLine("WebSocket sunucusu başlatıldı: ws://localhost:5000/ws/");

List<WebSocket> clients = new();

while (true)
{
    var context = await listener.GetContextAsync();

    if (context.Request.IsWebSocketRequest)
    {
        var wsContext = await context.AcceptWebSocketAsync(null);
        var socket = wsContext.WebSocket;

        lock (clients)
        {
            clients.Add(socket);
        }

        Console.WriteLine("Yeni istemci bağlandı.");

        _ = Task.Run(async () =>
        {
            var buffer = new byte[1024];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    lock (clients)
                    {
                        clients.Remove(socket);
                    }
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    Console.WriteLine("Bir istemci bağlantıyı kapattı.");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Alındı: {message}");

                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        if (client.State == WebSocketState.Open)
                        {
                            client.SendAsync(
                                Encoding.UTF8.GetBytes(message),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None
                            );
                        }
                    }
                }
            }
        });
    }
    else
    {
        context.Response.StatusCode = 400;
        context.Response.Close();
    }
}
