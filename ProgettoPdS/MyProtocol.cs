/*
 * Questa classe contiene i messaggi standard usati per il protocollo
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoPdS
{
    static class MyProtocol
    {
        #region Requests
        public const string CONNECTION = "PASS";
        public const string CONTROL = "CTRL";
        public const string QUIT = "QUIT";
        public const string COPY = "COPY";
        #endregion
        #region Answers
        public const string POSITIVE_ACK = "+OK";
        public const string NEGATIVE_ACK = "-ERR";
        #endregion
        public const string END_OF_MESSAGE = "<EOF>";

        public const int DEFAULT_PORT = 5000;

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
