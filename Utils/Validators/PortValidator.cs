using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkingTool.Utils.Validators
{
    public class PortValidator
    {
        public List<string> ParsePorts(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Empty port list string");
            }

            List<string> validPorts = new List<string>();

            string[] parts = input.Split(',').Select(sValue => sValue.Trim()).ToArray();
            parts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            foreach (string part in parts)
            {
                if (part.Contains("-"))
                {
                    string startIndex = part.Substring(0, part.IndexOf("-"));
                    string endIndex = part.Substring(part.IndexOf("-") + 1);

                    bool canParseStart = int.TryParse(startIndex, out int start);
                    bool canParseEnd = int.TryParse(endIndex, out int end);

                    if(IsValidPort(start) && IsValidPort(end) && start <= end)
                    {
                        validPorts.Add(startIndex + "-" + endIndex);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid port range: " + part);
                    }
                }
                else
                {
                    int port;
                    if (int.TryParse(part, out port) && IsValidPort(port))
                    {
                        validPorts.Add(port.ToString());
                    }
                    else
                    {
                        throw new ArgumentException("Invalid port: " + part);
                    }
                }
            }
            return validPorts;
        }

        private bool IsValidPort(int port)
        {
            return port >= 1 && port <= 65535;
        }
    }
}