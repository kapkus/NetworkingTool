using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NetworkingTool.Utils.Validators
{
    public class IpAddressValidator
    {

        public List<string> ParseIPs(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Empty IP address string");
            }

            List<string> validIPsAndRanges = new List<string>();

            string[] parts = input.Split(',').Select(sValue => sValue.Trim()).ToArray();
            parts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            foreach (string part in parts)
            {
                if (part.Contains("-"))
                {
                    string startIP = part.Substring(0, part.IndexOf("-"));
                    string endIP = part.Substring(part.IndexOf("-") + 1);
                    
                    int compare = CompareIpAddresses(startIP, endIP);

                    if (ValidateIPv4(startIP) && ValidateIPv4(endIP) && compare <= 0)
                    {
                        validIPsAndRanges.Add(startIP + "-" + endIP);
                    }
                    else 
                    { 
                        throw new ArgumentException("Invalid IP range: " + part);
                    }
                }
                else
                {
                    if (ValidateIPv4(part))
                    {
                        validIPsAndRanges.Add(part);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid IP address: " + part);
                    }
                }
            }

            return validIPsAndRanges;
        }

        private bool ValidateIPv4(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public static int CompareIpAddresses(string firstIP, string secondIP)
        {
            IPAddress first;
            IPAddress second;

            if (!(IPAddress.TryParse(firstIP, out first) && IPAddress.TryParse(secondIP, out second)))
                return -1;

            var int1 = first.ToUint32();
            var int2 = second.ToUint32();
            if (int1 == int2)
                return 0;
            if (int1 > int2)
                return 1;
            return -1;
        }
    }

    public static class IpExtensions
    {
        public static uint ToUint32(this IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();

            return ((uint)(bytes[0] << 24)) |
                   ((uint)(bytes[1] << 16)) |
                   ((uint)(bytes[2] << 8)) |
                   ((uint)(bytes[3]));
        }
    }
}
