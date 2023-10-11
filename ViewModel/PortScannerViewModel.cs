using NetworkingTool.Services;
using NetworkingTool.Utils;
using NetworkingTool.Utils.Validators;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NetworkingTool.ViewModel
{
    public class PortScannerViewModel : INotifyPropertyChanged
    {
        private string ipsAndRanges;
        private string portsAndRanges;
        private ObservableCollection<ScanResult> allScanResults;
        private bool isScanning;
        private bool hideClosedPorts;

        public ICommand ScanCommand { get; }

        public PortScannerViewModel()
        {
            ScanCommand = new RelayCommand(async () => await Scan(), () => ValidateInput());
        }

        public bool HideClosedPorts
        {
            get { return hideClosedPorts; }
            set
            {
                if (hideClosedPorts != value)
                {
                    hideClosedPorts = value;
                    OnPropertyChanged(nameof(HideClosedPorts));
                    ApplyPortStatusFilter();
                }
            }
        }

        private string currentScanStatus = "Ready";
        public string CurrentScanStatus
        {
            get { return currentScanStatus; }
            set
            {
                if (currentScanStatus != value)
                {
                    currentScanStatus = value;
                    OnPropertyChanged(nameof(CurrentScanStatus));
                }
            }
        }

        public string IpsAndRanges
        {
            get { return ipsAndRanges; }
            set
            {
                if (ipsAndRanges != value)
                {
                    ipsAndRanges = value;
                    OnPropertyChanged(nameof(IpsAndRanges));
                }
            }
        }

        public string PortsAndRanges
        {
            get { return portsAndRanges; }
            set
            {
                if (portsAndRanges != value)
                {
                    portsAndRanges = value;
                    OnPropertyChanged(nameof(PortsAndRanges));
                }
            }
        }

        private ObservableCollection<ScanResult> filteredScanResults;
        public ObservableCollection<ScanResult> FilteredScanResults
        {
            get { return filteredScanResults; }
            set
            {
                if (filteredScanResults != value)
                {
                    filteredScanResults = value;
                    OnPropertyChanged(nameof(FilteredScanResults));
                }
            }
        }

        public ObservableCollection<ScanResult> AllScanResults
        {
            get { return allScanResults; }
            set
            {
                if (allScanResults != value)
                {
                    allScanResults = value;
                    OnPropertyChanged(nameof(AllScanResults));
                    ApplyPortStatusFilter();
                }
            }
        }

        public bool IsScanning
        {
            get { return isScanning; }
            set
            {
                if (isScanning != value)
                {
                    isScanning = value;
                    OnPropertyChanged(nameof(IsScanning));
                }
            }
        }

        private void ApplyPortStatusFilter()
        {
            if (AllScanResults == null)
            {
                FilteredScanResults = new ObservableCollection<ScanResult>();
            }
            else if (HideClosedPorts)
            {
                FilteredScanResults = new ObservableCollection<ScanResult>(AllScanResults.Where(result => result.Status == "Open"));
            }
            else
            {
                FilteredScanResults = new ObservableCollection<ScanResult>(AllScanResults);
            }
        }

        private async Task Scan()
        {
            AllScanResults = new ObservableCollection<ScanResult>();

            Debug.WriteLine(IpsAndRanges);
            Debug.WriteLine(PortsAndRanges);

            IpAddressValidator ipAddressValidator = new IpAddressValidator();
            PortValidator portValidator = new PortValidator();

            List<string> IpList = ipAddressValidator.ParseIPs(IpsAndRanges);
            List<string> PortList = portValidator.ParsePorts(PortsAndRanges);

            var scanTasks = IpList.Select(ip => PerformScanAsync(ip, PortList));

            await Task.WhenAll(scanTasks);
            

        }

        private async Task PerformScanAsync(string ip, List<string> portList)
        {
            IpAddressFeeder ipFeeder = new IpAddressFeeder(ip);
            var tasks = new List<Task>();

            while (true)
            {
                IPAddress nextIp = ipFeeder.GetNextIp();
                if (nextIp == null)
                    break;

                foreach (string portRange in portList)
                {
                    PortFeeder portFeeder = new PortFeeder(portRange);

                    while (true)
                    {
                        int portNumber = portFeeder.GetNextPort();
                        if (portNumber == -1)
                            break;

                        

                        tasks.Add(Task.Run(async () =>
                        {
                            bool isOpened = await PortCheck.IsPortOpenAsync(nextIp.ToString(), portNumber);
                            //Debug.WriteLine($"{nextIp}:{portNumber} port open? - " + isOpened);
                            CurrentScanStatus = $"Scanning {nextIp}:{portNumber}...";
                            var newScanResult = new ScanResult
                            {
                                IpAddress = nextIp.ToString(),
                                Port = portNumber,
                                Status = isOpened ? "Open" : "Closed"
                            };

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                AllScanResults.Add(newScanResult);
                                ApplyPortStatusFilter();
                            });
                        }));
                    }
                }
            }

            await Task.WhenAll(tasks);
            CurrentScanStatus = "Ready";
        }

        private bool ValidateInput()
        {
            IpAddressValidator ipAddressValidator = new IpAddressValidator();
            PortValidator portValidator = new PortValidator();

            try { ipAddressValidator.ParseIPs(IpsAndRanges); }
            catch { return false; }

            try{ portValidator.ParsePorts(PortsAndRanges); }
            catch { return false; }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ScanResult
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Status { get; set; }
    }
}
