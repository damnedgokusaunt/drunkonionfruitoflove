/*
 * Questa classe contiene i messaggi standard usati per il protocollo
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyProject
{
    static class MyProtocol
    {

        public const int STD_COMMAND_LENGTH = 4;
        public static int STD_MESSAGE_LENGTH = STD_COMMAND_LENGTH + END_OF_MESSAGE.Length;
        public const string CONNECTION = "PASS";
        public const string CONTROL = "CTRL";
        public const string QUIT = "QUIT";
        public const string KEYDOWN = "KEYD";
        public const string KEYUP = "KEYU";
        public const string TARGET = "TARG";
        public const string PAUSE = "PAUS";
        public const string CLIPBOARD_IMPORT = "CIMP";
        
        public const string MOUSE_DOWN_RIGHT = "DR_MOUSE";
        public const string MOUSE_DOWN_LEFT = "DL_MOUSE";
        public const string MOUSE_UP_LEFT = "UL_MOUSE";
        public const string MOUSE_WHEEL = "WHEL";
   

       
        public const string COPY = "COPY";
        public const string COPY_S = "COPV";
        public const string CLIENT = "CLIE";
        
        public const string FILE_SEND = "FILE";
        public const string FILE_SEND_S = "FILV";
        
        public const string POSITIVE_ACK = "+OK";
        public const string NEGATIVE_ACK = "-ERR";
        public const string END_OF_MESSAGE = "<EOF>";
        public const string END_OF_DROPLIST = "<EOD>";
        public const string END_OF_DIR = "<EDI>";
        public const string CLIPBOARD_DIR = "clipboard";
        public const string DIRE_SEND = "DIRE";
        public const string DIRE_SEND_S = "DIRV";
        
        public const string CLEAN = "CLEA";
        public const string CLEAN_S = "CLEV";
        public const string IMG = "IMAG";
        public const string IMG_S = "IMAV";
        
        public const int DEFAULT_PORT = 5000;
        public const int CHUNK_SIZE = 8*1024;

        public const int KEEPALIVE_TIME = 1000*2; // ms
        public const int KEEPALIVE_INTERVAL = 1000; // ms

        public const int MAX_ATTEMPTS = 3;

        public static string message(string code, string pwd)
        {
            return code + pwd + END_OF_MESSAGE;
        }

        public static string message(string code)
        {
            return code + END_OF_MESSAGE;
        }
    }
}
