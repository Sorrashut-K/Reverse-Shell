using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReverseShellServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        TcpServer tcpServer;
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        public MainWindow()
        {
            InitializeComponent();

            tcpServer = new TcpServer(6666);

            tcpServer.StartListening(cancellationToken.Token);

            Application.Current.Dispatcher.ShutdownStarted += (object sentder, EventArgs e) =>
            {
                cancellationToken.Cancel();
            };
        }
    }
}
