using NetworkingTool.ViewModel;
using System.Windows.Controls;

namespace NetworkingTool.Views
{
    /// <summary>
    /// Interaction logic for PortScanner.xaml
    /// </summary>
    public partial class PortScannerView : UserControl
    {
        public PortScannerView()
        {
            InitializeComponent();
            DataContext = new PortScannerViewModel();
        }


    }
}
