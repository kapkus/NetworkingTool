using System;
using System.Collections.Generic;
using System.Net;

namespace NetworkingTool.Utils
{
    public class IpAddressFeeder
    {
        private IEnumerator<IPAddress> ipEnumerator;

        public IpAddressFeeder(string ipOrRange)
        {
            ipEnumerator = GetIpEnumerator(ipOrRange).GetEnumerator();
        }

        public IPAddress GetNextIp()
        {
            if (ipEnumerator.MoveNext())
            {
                return ipEnumerator.Current;
            }

            return null;
        }

        private IEnumerable<IPAddress> GetIpEnumerator(string ipOrRange)
        {
            if (ipOrRange.Contains("-"))
            {
                string[] parts = ipOrRange.Split('-');
                if (parts.Length != 2)
                {
                    throw new ArgumentException("Invalid IP range format");
                }

                IPAddress startIp = IPAddress.Parse(parts[0].Trim());
                IPAddress endIp = IPAddress.Parse(parts[1].Trim());

                byte[] startBytes = startIp.GetAddressBytes();
                byte[] endBytes = endIp.GetAddressBytes();

                Array.Reverse(startBytes);
                Array.Reverse(endBytes);

                uint start = BitConverter.ToUInt32(startBytes, 0);
                uint end = BitConverter.ToUInt32(endBytes, 0);

                for (uint i = start; i <= end; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(i);
                    Array.Reverse(bytes);
                    yield return new IPAddress(bytes);
                }
            }
            else
            {
                yield return IPAddress.Parse(ipOrRange);
            }
        }
    }
}
