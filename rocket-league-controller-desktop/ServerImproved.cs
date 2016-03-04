using rlc_gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyInterfaceWrap;

namespace rlController
{
    public class ServerImproved
    {
        private const int SERVER_PORT = 101;
        private const int BUFFER_SIZE = 256;
        private const int HEADER_SIZE = 8;
        private const int INTEGER_SIZE = 4;

        private static object Lock = new object();

        private Dictionary<Socket,uint> socketIdMap;
        private Dictionary<uint, Controller> controllerIdMap;
        private Dictionary<uint, bool> idTakenMap;

        private Socket listener;
        private IPEndPoint localEndPoint;

        private List<uint> controllerIds;

        private bool doListen = false;
        private bool started = false;

        private Window window;

        public ServerImproved(Window window, int port)
        {
            this.window = window;
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());//Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            //foreach (var i in ipHostInfo.AddressList) Console.WriteLine(i);
            localEndPoint = new IPEndPoint(ipAddress, port);
        }

        public void start(List<uint> controllerIds)
        {
            this.controllerIds = controllerIds;
            socketIdMap = new Dictionary<Socket, uint>();
            controllerIdMap = new Dictionary<uint, Controller>();
            idTakenMap = new Dictionary<uint, bool>();
            lock (Lock)
            {
                foreach (uint id in controllerIds)
                {
                    Controller controller = new Controller(id);
                    controllerIdMap.Add(id, controller);
                    idTakenMap.Add(id, false);
                }
            }
            Task.Run(()=>listenForConnections());
            started = true; 
            Console.WriteLine("Started server @ port {0}",SERVER_PORT);
        }

        public void freeControllers()
        {
            foreach (Controller c in controllerIdMap.Values)
            {
                c.relinquish();
            }
        }
        public void stop()
        {
            lock (Lock)
            {
                if (!started) return;
                Console.WriteLine("Stopping...");
                stopListeningForConnections();
                freeControllers();

                Socket[] sockets = socketIdMap.Keys.ToArray();

                for (int i = 0; i < sockets.Length; i++)
                {
                    dropConnection(sockets[i], MessageType.SERVER_STOPPED);
                }
                started = false;
            }
        }
        

        private void handleListeningSocketException()
        {
            stop();
            window.BeginInvoke((Action)delegate ()
            {
                window.changeServerStatus(false);
            });
            // TODO:  set server state to false
        }

        private void handleClientSocketException(Socket socket)
        {
            disposeSocket(socket);
        }

        private void listenForConnections()
        {
            Console.WriteLine("in listenForConnections");
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);
            doListen = true;
            while (doListen)
            {
                Console.WriteLine("Accepting sockets...");
                try
                {
                    Socket socket = listener.Accept();
                    Console.WriteLine("Socket accepted");
                    Task.Run(() => handleSocket(socket));
                } catch (SocketException e)
                {
                    handleListeningSocketException();
                    return;
                }
            }
        }

        private void updateControllerInfo(int which, bool status)
        {
            window.BeginInvoke((Action)delegate ()
            {
                window.updateController(which, status);
            });
        }

        private void stopListeningForConnections()
        {
            listener.Close();
        }

        private void handleSocket(Socket socket)
        {
            Console.WriteLine("Handling socket");
            int length, type;
            readHeader(socket, out length, out type);
            Console.WriteLine("Received length = {0}, type = {1}", length, type);
            if (type != MessageType.ID) dropConnection(socket, MessageType.INVALID_MSG_TYPE);
            byte[] content = readContent(socket, length);
            uint id = BitConverter.ToUInt32(content, 0);
            Console.WriteLine("Received id = {0}", id);
            checkId(id,socket);
        }
        private void checkId(uint id, Socket socket)
        {
            lock (Lock)
            {
                if (!idTakenMap.ContainsKey(id))
                {
                    dropConnection(socket, MessageType.ID_INVALID);
                    Console.WriteLine("dropping, ID_INVALID");
                    return;
                }
                else if (idTakenMap[id] == true)
                {
                    dropConnection(socket, MessageType.ID_TAKEN);
                    Console.WriteLine("dropping, ID_TAKEN");
                    return;
                }
                else
                {
                    socketIdMap.Add(socket, id);
                    idTakenMap[id] = true;
                    updateControllerInfo(controllerIds.IndexOf(id), true);
                }
            }
            Console.WriteLine("Accepting socket");
            send(socket, MessageType.ACCEPTED);
            listenToSocket(socket);
        }

        private void handleMessage(Socket socket, int type, byte[] content)
        {
            uint id = socketIdMap[socket];
            controllerIdMap[id].onMessage(type, content);
        }

        private void listenToSocket(Socket socket)
        {
            Console.WriteLine("Listening to socket");
            while (true)
            {
                int type, length;
                try {
                    //Console.WriteLine("reading header");
                    readHeader(socket, out length, out type);
                    //Console.WriteLine(type);
                    byte[] content = readContent(socket, length);
                    //Console.WriteLine("reading content");
                    handleMessage(socket, type, content);
                } catch (SocketReadException)
                {
                    Console.WriteLine("SocketReadException, disposing of socket");
                    disposeSocket(socket);
                    return;   
                }
            }
        }

        private void dropConnection(Socket socket, int reason)
        {
            send(socket, reason);
            disposeSocket(socket);
        }

        private void disposeSocket(Socket socket)
        {
            
            lock (Lock)
            {
                if (socketIdMap.ContainsKey(socket))
                {
                    uint id = socketIdMap[socket];
                    socketIdMap.Remove(socket);
                    if (idTakenMap[id] == true) idTakenMap[id] = false;
                    updateControllerInfo(controllerIds.IndexOf(id), false);
                    Console.WriteLine("Disposing of socket using id = {0}", id);
                }
            }
            socket.Close();
        }

        private void send(Socket socket, int type, int content)
        {
            writeMessage(socket, MessageWrapper.wrapMessage(type, content));
        }

        private void send(Socket socket, int type)
        {
            writeMessage(socket, MessageWrapper.wrapMessage(type));
        }

        private void writeMessage(Socket socket, byte[] content)
        {
            //Console.WriteLine("Writing message to socket");
            //foreach (byte b in content) Console.Write(b + " ");
            //Console.WriteLine();
            socket.Send(content);
        } 

        private void readHeader(Socket socket, out int length, out int type)
        {
            int read = 0;
            byte[] header = new byte[HEADER_SIZE];
            //Console.WriteLine("Header size = {0}", HEADER_SIZE);
            //Console.WriteLine("read = {0}", read);
            while (read < HEADER_SIZE)
            {
                try
                {
                    int readNow = socket.Receive(header, read, HEADER_SIZE - read, SocketFlags.None);
                    //Console.WriteLine(readNow);
                    if (readNow == 0) throw new SocketReadException();
                    read += readNow;
                } catch (SocketException e)
                {
                    handleClientSocketException(socket);
                }
            }
            header = header.Reverse().ToArray();
            type = BitConverter.ToInt32(header, 0);
            length = BitConverter.ToInt32(header, INTEGER_SIZE) - INTEGER_SIZE;
        }

        private byte[] readContent(Socket socket, int length)
        {
            int savedLength = length;
            byte[] buffer = new byte[BUFFER_SIZE];

            MemoryStream stream = new MemoryStream();

            while (length > 0)
            {
                try
                {
                    int read = socket.Receive(buffer, 0, length, SocketFlags.None);
                    if (read == 0) throw new SocketReadException();
                    stream.Write(buffer, 0, read);
                    length -= read;
                } catch (SocketException e)
                {
                    handleClientSocketException(socket);
                }
            }
            byte[] content = stream.GetBuffer().Take(savedLength).ToArray();
            content = content.Reverse().ToArray();
            return content;
        }

        private class MessageWrapper
        {
            public static byte[] wrapMessage(int type, int content)
            {
                int length = 2 * INTEGER_SIZE;
                byte[] output = new byte[3 * INTEGER_SIZE];
                byte[] lengthBytes = BitConverter.GetBytes(length);
                byte[] typeBytes = BitConverter.GetBytes(type);
                byte[] contentBytes = BitConverter.GetBytes(content);
                Array.Copy(lengthBytes, 0, output, 0, lengthBytes.Length);
                Array.Copy(typeBytes, 0, output, lengthBytes.Length, typeBytes.Length);
                Array.Copy(contentBytes, 0, output, lengthBytes.Length+typeBytes.Length, contentBytes.Length);
                return output;
            }
            public static byte[] wrapMessage(int type)
            {
                int length = INTEGER_SIZE;
                byte[] output = new byte[2 * INTEGER_SIZE];
                byte[] lengthBytes = BitConverter.GetBytes(length);
                byte[] typeBytes = BitConverter.GetBytes(type);
                Array.Copy(lengthBytes, 0, output, 0, lengthBytes.Length);
                Array.Copy(typeBytes, 0, output, lengthBytes.Length, typeBytes.Length);
                return output;
            }

        }

    }

    public class MessageType
    {
        public const int ACCEPTED = 0x01;
        public const int ID = 0x02;

        public const int L_AXIS_X = 0x11;
        public const int DRIVE = 0x12;
        public const int JUMP = 0x13;
        public const int BOOST = 0x14;
        public const int CONTROL_LOCK = 0x15;
        public const int START = 0x16;
        public const int DRIFT = 0x17;
        public const int CAMERA = 0x18;

        public const int ID_INVALID = 0x31;
        public const int ID_TAKEN = 0x32;
        public const int SERVER_STOPPED = 0x33;
        public const int INVALID_MSG_TYPE = 0x34;
        

    }

    public class SocketReadException : Exception
    {

        public SocketReadException()
        {

        }
    }

}
