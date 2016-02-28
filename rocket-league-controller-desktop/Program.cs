using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using vJoyInterfaceWrap;

namespace rlController
{
    class Program
    {
        public const uint ID = 1;

        public static void Main(string[] args)
        {
            try {
                Controller controller = new Controller(ID);

                while (true) { 
                    Server server = new Server();
                    server.onMessage += controller.onMessage;
                    server.connect();
                }

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
