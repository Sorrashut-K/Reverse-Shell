using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReverseShellServer
{

    class TcpServer
    {
        public TcpListener tcpListener { get; set; }
        public IPAddress ip { get; set; }
        public int port { get; set; }

        public event EventHandler ClientConnectedEvent;
        public event EventHandler DataRecievedEvent;

        public TcpServer(IPAddress _ip, int _port)
        {
            ip = _ip;
            port = _port;

            tcpListener = new TcpListener(ip, this.port);
        }

        public TcpServer(int _port)
        {
            ip = IPAddress.Any;
            port = _port;

            tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public async void StartListening(CancellationToken cancellationToken)
        {
            tcpListener.Start();

            cancellationToken.Register(() => {
                tcpListener.Stop();
                Console.WriteLine("Stop Listener");
            });


            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("Connection accepted.");

                var childSocketThread = new Thread(() =>
                {
                    HandleClient(client, cancellationToken);
                });
                childSocketThread.Start();
            }
        }


        private async static void HandleClient(TcpClient client, CancellationToken cancellationToken)
        {
            while (client.Connected && !cancellationToken.IsCancellationRequested)
            {
                Console.Write(client.Connected + " " + port);
                Console.WriteLine(!cancellationToken.IsCancellationRequested);

                //---get the incoming data through a network stream---
                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];

                //---read incoming stream---
                int bytesRead = await nwStream.ReadAsync(buffer, 0, client.ReceiveBufferSize);
                Console.WriteLine($"Bytes recieved: {bytesRead}");
                if (bytesRead == 0)
                {
                    Thread.Sleep(1000);
                    break;
                }
                //---convert the data received into a string---
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"{client.Client.RemoteEndPoint.ToString()} Received : {dataReceived}");
            }
        }
    }

    class ClientConnectedArgs : EventArgs    // guideline: derive from EventArgs
    {
        public string Client { get; set; }
    }

    class DataRecievedArgs : EventArgs    // guideline: derive from EventArgs
    {
        public string Client { get; set; }
    }

}
