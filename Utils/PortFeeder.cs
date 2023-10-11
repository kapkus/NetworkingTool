using System;
using System.Collections.Generic;

namespace NetworkingTool.Utils
{
    public class PortFeeder
    {
        private IEnumerator<int> portEnumerator;

        public PortFeeder(string portsOrRange)
        {
            portEnumerator = GetPortEnumerator(portsOrRange).GetEnumerator();
        }

        public int GetNextPort()
        {
            if (portEnumerator.MoveNext())
            {
                return portEnumerator.Current;
            }

            return -1;
        }

        private IEnumerable<int> GetPortEnumerator(string portsOrRange)
        {
            if (portsOrRange.Contains("-"))
            {
                string[] parts = portsOrRange.Split('-');
                if (parts.Length != 2)
                {
                    throw new ArgumentException("Invalid port range format");
                }

                int startPort = int.Parse(parts[0].Trim());
                int endPort = int.Parse(parts[1].Trim());

                if (startPort < 1 || startPort > 65535 || endPort < 1 || endPort > 65535 || startPort > endPort)
                {
                    throw new ArgumentException("Invalid port range");
                }

                for (int port = startPort; port <= endPort; port++)
                {
                    yield return port;
                }
            }
            else
            {
                int port = int.Parse(portsOrRange);
                if (port < 1 || port > 65535)
                {
                    throw new ArgumentException("Invalid port number");
                }

                yield return port;
            }
        }
    }
}
