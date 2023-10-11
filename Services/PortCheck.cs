using NetworkingTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingTool.Services
{
    public class PortCheck
    {
        public static bool IsPortOpen(string serverAddress, int port)
        {
            try
            {
                using (SocketManager socketManager = new SocketManager())
                {
                    socketManager.Connect(serverAddress, port);
                    socketManager.Disconnect();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> IsPortOpenAsync(string serverAddress, int port)
        {
            try
            {
                using (SocketManager socketManager = new SocketManager())
                {
                    await socketManager.ConnectAsync(serverAddress, port);
                    socketManager.Disconnect();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
