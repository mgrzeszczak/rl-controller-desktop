using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace rlController
{
    public class Server
    {
        private const int SERVER_PORT   = 101;
        private const int BUFFER_SIZE   = 256;
        private const int HEADER_SIZE   = 8;
        private const int INTEGER_SIZE  = 4;

        private int counter = 0;

        private Socket socket = null;

        public Action<int, byte[]> onMessage = null;

        public Server()
        {
            
        }

        public void connect()
        {
            Console.WriteLine("Starting server at port {0}", SERVER_PORT);
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress addr = new IPAddress(0);

            Console.WriteLine("IP ADDRESS : " + ipAddress);

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, SERVER_PORT);
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            socket = listener.Accept();
            Console.WriteLine("Phone connected");
            listener.Close();

            try {
                listen();
            } catch (SocketReadException e)
            {
                Console.WriteLine("SocketReadException caught.");
                disconnect();
            }
            
        }

        public void disconnect()
        {
            socket.Close();
            socket = null;
            Console.WriteLine("Disconnected");
        }

        private void listen()
        {
            while (true)
            {
                int type, length;
                readHeader(out length,out type);
                byte[] content = readContent(length);
                handleMessage(type, content);
            }
        }

        private void handleMessage(int type, byte[] content)
        {
            if (onMessage!=null) onMessage(type, content);
        }

        private void readHeader(out int length, out int type)
        {
            int read = 0;
            byte[] header = new byte[HEADER_SIZE];
            while (read < HEADER_SIZE) {
                int readNow = socket.Receive(header, read, HEADER_SIZE - read, SocketFlags.None);
                if (readNow == 0) throw new SocketReadException();
                read += readNow;
            }
            header = header.Reverse().ToArray();
            type = BitConverter.ToInt32(header, 0);
            length = BitConverter.ToInt32(header, INTEGER_SIZE) - INTEGER_SIZE;
            
            /*
            if (type == 4)
            {
                counter++;
                Console.WriteLine("New message: type = " + type + " - total count == "+counter);
            }*/
        }
        private byte[] readContent(int length)
        {
            //Console.WriteLine("Length: " + length);
            int savedLength = length;
            byte[] buffer = new byte[BUFFER_SIZE];

            MemoryStream stream = new MemoryStream();

            while (length > 0)
            {
                int read = socket.Receive(buffer, 0, length, SocketFlags.None);
                if (read == 0) throw new SocketReadException();
                stream.Write(buffer, 0, read);
                length -= read;
            }
            byte[] content = stream.GetBuffer().Take(savedLength).ToArray();
            content = content.Reverse().ToArray();
            return content;
        }

        static void handleMessage(int type, byte[] content, vJoy joystick)
        {
            int val;
            switch (type)
            {
                case MessageType.L_AXIS_X:

                    val = BitConverter.ToInt32(content, 0);
                    //Console.WriteLine("VALUE " + val);
                    joystick.SetAxis(val, 1, HID_USAGES.HID_USAGE_X);
                    break;
                case MessageType.L_AXIS_Y:

                    break;
                case MessageType.DRIVE:
                    val = BitConverter.ToInt32(content, 0);
                    Console.WriteLine("VALUE " + val);
                    joystick.SetAxis(val, 1, HID_USAGES.HID_USAGE_RZ);


                    joystick.SetAxis(15000, 1, HID_USAGES.HID_USAGE_RX);
                    joystick.SetAxis(15000, 1, HID_USAGES.HID_USAGE_RY);

                    
                    joystick.SetAxis(0, 1, HID_USAGES.HID_USAGE_SL0);
                    joystick.SetAxis(0, 1, HID_USAGES.HID_USAGE_SL1);
                    joystick.SetAxis(0, 1, HID_USAGES.HID_USAGE_Z);

                    joystick.SetAxis(0, 1, HID_USAGES.HID_USAGE_WHL);
                    break;
            }
        }
    }

    public class MessageType
    {

        public const int L_AXIS_X          = 0x00000001;
        public const int L_AXIS_Y          = 0x00000002;

        public const int DRIVE             = 0x00000003;

        public const int JUMP              = 0x00000004;
        public const int BOOST             = 0x00000005;

    }

    public class SocketReadException : Exception {

        public SocketReadException()
        {

        }
    }

}
