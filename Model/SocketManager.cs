using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTool.Model
{
    public class SocketManager : IDisposable
    {
        private Socket socket;
        private byte[] buffer = new byte[1024];

        public void Connect(string serverAddress, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse(serverAddress), port));
        }

        public async Task ConnectAsync(string serverAddress, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endpoint = new IPEndPoint(IPAddress.Parse(serverAddress), port);

            await socket.ConnectAsync(endpoint);
        }

        public void Send(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            socket.Send(data);
        }

        public string Receive()
        {
            int receivedBytes = socket.Receive(buffer);
            return Encoding.ASCII.GetString(buffer, 0, receivedBytes);
        }

        public void Disconnect()
        {
            socket.Close();
        }

        public void Dispose()
        {
            socket?.Dispose();
        }
    }
}
