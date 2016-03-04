using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace rlController
{
    class Controller
    {
        private vJoy joystick;
        private vJoy.JoystickState iReport;
        private uint id;

        public Controller(uint id)
        {
            init(id);
        }

        private void init(uint id)
        {
            this.id = id;
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();

            // Device ID can only be in the range 1-16
            if (id <= 0 || id > 16) err(String.Format("Illegal device ID {0} (1-16)", id));

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.vJoyEnabled()) err("vJoy driver not enabled: Failed Getting vJoy attributes.");
            else Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), 
                    joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());
            VjdStat status = joystick.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder.", id);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free.", id);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    err(String.Format("vJoy Device {0} is already owned by another feeder.", id));
                    return;
                case VjdStat.VJD_STAT_MISS:
                    err(String.Format("vJoy Device {0} is not installed or disabled.", id));
                    return;
                default:
                    err(String.Format("vJoy Device {0} general error.", id));
                    return;
            };
            // Check which axes are supported
            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            int nButtons = joystick.GetVJDButtonNumber(id);
            int ContPovNumber = joystick.GetVJDContPovNumber(id);
            int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);
            // Print results
            /*Console.WriteLine("\nvJoy Device {0} capabilities:", id);
            Console.WriteLine("Numner of buttons\t\t{0}", nButtons);
            Console.WriteLine("Numner of Continuous POVs\t{0}", ContPovNumber);
            Console.WriteLine("Numner of Descrete POVs\t\t{0}", DiscPovNumber);
            Console.WriteLine("Axis X\t\t{0}", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Y\t\t{0}", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Z\t\t{0}", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Rx\t\t{0}", AxisRX ? "Yes" : "No");
            Console.WriteLine("Axis Rz\t\t{0}", AxisRZ ? "Yes" : "No");*/
            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})", DllVer);
            else
                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})", DrvVer, DllVer);
            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                err(String.Format("Failed to acquire vJoy device number {0}.", id));
                return;
            }
            else Console.WriteLine("Acquired: vJoy device number {0}.", id);

            initAxes();
        }

        public void relinquish()
        {
            initAxes();
            joystick.RelinquishVJD(id);
        }

        private void defaultValues()
        {
            initAxes();
            initButtons();
        }

        private void initButtons()
        {
            joystick.ResetButtons(id);
        }

        private void initAxes()
        {
            initAxis(HID_USAGES.HID_USAGE_X);
            initAxis(HID_USAGES.HID_USAGE_Y);
            initAxis(HID_USAGES.HID_USAGE_Z);
            initAxis(HID_USAGES.HID_USAGE_RX);
            initAxis(HID_USAGES.HID_USAGE_RY);
            initAxis(HID_USAGES.HID_USAGE_RZ);
        }
        private void initAxis(HID_USAGES axis)
        {
            long maxval = 0;
            long minval = 0;
            joystick.GetVJDAxisMax(id, axis, ref maxval);
            joystick.GetVJDAxisMin(id, axis, ref minval);
            setAxis(axis, (int)(maxval - minval) / 2);
        }

        private void setAxis(HID_USAGES usage, int value)
        {
            joystick.SetAxis(value, id, usage);
        }
        private void useButton(uint button, bool value)
        {
            Console.WriteLine("Using button {0} - {1}", button, value);
            joystick.SetBtn(value, id, button);
        }
        private int counter = 0;

        public void onMessage(int type, byte[] content)
        {
            //Console.WriteLine("On message: type = {0}", type);
            int msg;
            switch (type)
            {
                case MessageType.L_AXIS_X:
                    setAxis(HID_USAGES.HID_USAGE_X, BitConverter.ToInt32(content, 0));
                    break;
                case MessageType.DRIVE:
                    setAxis(HID_USAGES.HID_USAGE_RZ, BitConverter.ToInt32(content, 0));
                    int realVal = 32767 - BitConverter.ToInt32(content, 0);
                    //int normalized = 16384 + (int)(10000*((realVal - 16384.0) / 16384.0));
                    setAxis(HID_USAGES.HID_USAGE_Y, realVal);
                    break;
                case MessageType.BOOST:
                    int boost = BitConverter.ToInt32(content, 0);
                    bool bboost = boost == 1 ? true : false;
                    useButton(2, bboost);
                    break;
                case MessageType.JUMP:
                    int jump = BitConverter.ToInt32(content, 0);
                    bool bjump = jump == 1 ? true : false;
                    counter++;
                    Console.WriteLine("{0} - jump -> {1}", counter, bjump);
                    useButton(1, bjump);
                    break;
                case MessageType.CONTROL_LOCK:
                    defaultValues();
                    break;
                case MessageType.START:
                    Console.WriteLine("START PRESSED");
                    msg = BitConverter.ToInt32(content, 0);
                    useButton(8, msg==1);
                    break;
                case MessageType.DRIFT:
                    msg = BitConverter.ToInt32(content, 0);
                    useButton(3, msg == 1);
                    break;
                case MessageType.CAMERA:
                    msg = BitConverter.ToInt32(content, 0);
                    useButton(4, msg == 1);
                    break;
            }
        }
        
        private void err(string message)
        {
            throw new ControllerException(message);
        }
    }

    public class ControllerException : Exception
    {
        public ControllerException(string message) : base(message)
        {
            
        }
    }
}
