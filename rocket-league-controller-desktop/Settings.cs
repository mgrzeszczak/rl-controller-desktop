using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace rlController
{
    public class Settings
    {
        private static string DOCUMENTS_PATH = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+"\\";
        private static string FILENAME = "rlc_server.config";

        private static int DEFAULT_PORT = 101;
        private static bool DEFAULT_TWO_CONTROLLERS = false;
        private static int[] DEFAULT_CONTROLLER_IDS = new int[2] { 1, 2 };

        public int port = DEFAULT_PORT;
        public bool twoControllers = DEFAULT_TWO_CONTROLLERS;
        public int[] controllerIds = DEFAULT_CONTROLLER_IDS;
        
        private Settings()
        {

        }

        public static Settings loadSettings()
        {
            Settings instance = null;
            if (!File.Exists(DOCUMENTS_PATH + FILENAME))
            {
                Console.WriteLine("File doesnt exist");
                instance = new Settings();
                instance.saveSettings();
                return instance;
            }
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            Stream stream = new FileStream(DOCUMENTS_PATH + FILENAME, FileMode.Open);
            try
            {
                instance = (Settings)ser.Deserialize(stream);
            }
            catch (Exception e) {
                instance = new Settings();
            }
            if (instance.controllerIds[0] < 0 || instance.controllerIds[0] > 15) instance.controllerIds[0] = 0;
            if (instance.controllerIds[1] < 0 || instance.controllerIds[1] > 15) instance.controllerIds[1] = 0;
            stream.Close();
            return instance;
        }

        public void saveSettings()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            Stream stream = new FileStream(DOCUMENTS_PATH + FILENAME, FileMode.Create);
            ser.Serialize(stream, this);
            stream.Close();
        }

        public int getPort()
        {
            return port;
        }
        public bool areTwoControllersEnable()
        {
            return twoControllers;
        }
        public int[] getControllerIds()
        {
            return controllerIds;
        }
    }
}
