// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;
using TpClientChat;

/*Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 2345);
Guid id = Guid.NewGuid();

await client.ConnectAsync(endPoint);

string message = id.ToString();
byte[] buffer = Encoding.UTF8.GetBytes(message);

await client.SendAsync(buffer);
Console.WriteLine(message);

message = "loulou";
buffer = Encoding.UTF8.GetBytes(message);
await client.SendAsync(buffer);
Console.WriteLine(message);

message = "Il fait beau et chaud.";
buffer = Encoding.UTF8.GetBytes(message);
await client.SendAsync(buffer);

Console.WriteLine(message);

client.Shutdown(SocketShutdown.Both);
client.Close();
*/
ClientChat clientChat = new ClientChat();
await clientChat.RunAsync();
