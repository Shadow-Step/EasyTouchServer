using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

//TODO #001 -> do correct closing, sending exit data ... method Close()

namespace EasyTouchServer.serlib
{ 
    enum ServerStatus
    {
        OFFLINE,
        ONLINE,
        ABORTED
    }

    class TouchServer
    {
        //Fields
        private ServerStatus server_status = ServerStatus.OFFLINE;
        public ServerStatus ServerStatus
        {
            get { return server_status; }
            set { server_status = value;}
        }
            
        private Socket server_socket = null;
        private Socket client_socket = null; //connected device
        private IPEndPoint local_ip = null; //server_socket listen that IP
        private int local_port; //needed for @abort_socket in @Close() method

        //Constructor
        public TouchServer(string ip, int port)
        {
            server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            local_ip = new IPEndPoint(IPAddress.Parse(ip), port);
            server_socket.Bind(local_ip);
            local_port = port;
        }
        //Methods
        public void Start() //Start listening connections
        {
            server_socket.Listen(1);
            server_status = ServerStatus.ONLINE;
            while (server_status == ServerStatus.ONLINE) //Listen loop
            {
                client_socket = server_socket.Accept();//THREAD BLOCKED!!!
                byte[] data = new byte[13]; // bool, int, int, int -> reverse encoding
                while (client_socket != null)
                {
                    int data_size = client_socket.Receive(data); //THREAD BLOCKED!!!
                    if (data_size == 0) // 0 -> exit data, client disconnected
                    {
                        client_socket?.Shutdown(SocketShutdown.Both);
                        client_socket?.Close();
                        client_socket = null;
                    }
                    else
                    {
                        if (BitConverter.IsLittleEndian) // Big-endian / Little-endian check
                            Array.Reverse(data); 
                        Controller.ControllerEvent(DataPackage.ParseData(data)); //Sending parsed data to Controller
                    }
                }
            }
            server_status = ServerStatus.OFFLINE;
        }
        public void Close() //Close @Start() loop
        {
            if (server_status == ServerStatus.ONLINE)
            {
                server_status = ServerStatus.ABORTED;
                if (client_socket == null) // create temp connection to abort Accept() block
                {
                    TcpClient abort_socket = new TcpClient(); // temp socket
                    abort_socket.Connect(local_ip.Address, local_port); //connect to @server_socket
                    using (NetworkStream stream = abort_socket.GetStream())
                    {
                        byte[] data = new byte[0]; // abort data
                        stream.Write(data, 0, data.Length);      
                    }
                    abort_socket.Close();
                }
                else
                {
                    //TODO #001
                    client_socket.Send(Encoding.UTF8.GetBytes("")); //abort data
                    client_socket.Shutdown(SocketShutdown.Both);
                    client_socket.Close();
                    //TODO /#001
                }
            }
            //server_socket?.Shutdown(SocketShutdown.Both);
            //server_socket?.Close();
        }
    }
}
