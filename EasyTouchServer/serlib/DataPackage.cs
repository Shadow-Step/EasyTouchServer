using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTouchServer.serlib
{
    public abstract class DataPackage
    {
        //Byte index->          12          8           4           0
        //Mouse data ->     byte EVENT, int ACTION, int DIRX,    int DIY
        //Keyboard data ->  byte EVENT, int ACTION, int KEYCODE, int SPECIAL_BUTTONS

        private const bool MOUSE_EVENT = false; //EVENT byte
        private const bool KEYBOARD_EVENT = true; //EVENT byte

        public static DataPackage ParseData(byte[] data)
        {
            bool input_event = BitConverter.ToBoolean(data, 12); //check EVENT byte
            if (input_event == MOUSE_EVENT)
                return new MouseData(data);
            else
                return new KeyboardData(data);
        }
    }
    
    public class MouseData : DataPackage
    {
        //Mouse action codes
        static private Dictionary<int, UInt32> action_list = new Dictionary<int, UInt32>()
        {
            {0 , 0x0002},//Left button down
            {1 , 0x0004},//Left button up
            {2 , 0x0001} //Move
        };

        //Fields
        private int action = 2;
        public uint Action => action_list[action]; //Return value from Dictionary 

        public int DirX { get; set; }
        public int DirY { get; set; }
        
        //Constructor
        public MouseData(byte[] data)
        {
            DirY = BitConverter.ToInt32(data, 0);
            DirX = BitConverter.ToInt32(data, 4);
            action = BitConverter.ToInt32(data, 8);
        }
    }
    public class KeyboardData : DataPackage
    {
        //Keyboard key codes
        static private Dictionary<int, byte> key_codes = new Dictionary<int, byte>()
        {
            {29 ,0x41}, // A
            {30 ,0x42}, // B
            {31 ,0x43}, // C
            {32 ,0x44}, // D
            {33 ,0x45}, // E
            {34 ,0x46}, // F
            {35 ,0x47}, // G
            {36 ,0x48}, // H
            {37 ,0x49}, // I
            {38 ,0x4A}, // J
            {39 ,0x4B}, // K
            {40 ,0x4C}, // L
            {41 ,0x4D}, // M
            {42 ,0x4E}, // N
            {43 ,0x4F}, // O
            {44 ,0x50}, // P
            {45 ,0x51}, // Q
            {46 ,0x52}, // R
            {47 ,0x53}, // S
            {48 ,0x54}, // T
            {49 ,0x55}, // U
            {50 ,0x56}, // V
            {51 ,0x57}, // W
            {52 ,0x58}, // X
            {53 ,0x59}, // Y
            {54 ,0x5A} // Z
        };
        private const int KEYDOWN = 0;
        private const int KEYUP = 2;

        //Fields
        private int action; // KeyDown | KeyUp
        public int Action
        {
            get { return action == 0 ? KEYDOWN : KEYUP;}
            set { action = value; }
        }

        private int key_code; // Button
        public int Key_code // Return code from dictionary
        {
            get
            {
                if (key_codes.ContainsKey(key_code))
                    return key_codes[key_code];
                else
                    return 0x01; // temp
            }
            set { key_code = value; }
        } 

        private int special = 0; //Special buttons (ALT,TAB,CTRL and other)
        //Constructor
        public KeyboardData(byte[] data)
        {
            special = BitConverter.ToInt32(data, 0);
            key_code = BitConverter.ToInt32(data, 4);
            action = BitConverter.ToInt32(data, 8);
        }
    }
}
