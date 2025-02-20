using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Guss_Name_Server_App
{
    internal class Server
    {
        // ip for server
        public IPAddress ip;
        // that listen to the clients
        public TcpListener Listener { set; get; }

        // make server synchronous
        public Thread ThreadServer { set; get; }

        // that for each client to read and write data throw connection
        public NetworkStream Stream { set; get; }

        public Server()
        {
            try
            {
                ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
                Listener = new TcpListener(ip, 12345);
                ThreadServer = new Thread(ServerListen);
            }
            catch (Exception e)
            {
                Console.WriteLine("in server ctor " + e.Message);
            }
        }
        public void Start()
        {
            Console.WriteLine("Server is Started ...");
            Listener.Start();
            ThreadServer.Start();
        }


        void ServerListen()
        {
            try
            {
                // for accepting many clients
                while (true)
                {
                    // accept client request to connect
                    Socket ClientSocket = this.Listener.AcceptSocket();
                    bool flag = true;
                    Stream = new NetworkStream(ClientSocket);
                    TcpClient tcpClient = new TcpClient { Client = ClientSocket };

                    // accept client data send
                    while (flag)
                    {
                        if (Stream.DataAvailable)
                        {
                            // store New Client info
                            string PlayerName = new BinaryReader(Stream).ReadString();
                            //Console.WriteLine(PlayerName + "  ");
                            Player player = new Player(Stream, PlayerName,tcpClient);
                            flag = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("in server listen " + e.Message);
            }
        }
    }
}
