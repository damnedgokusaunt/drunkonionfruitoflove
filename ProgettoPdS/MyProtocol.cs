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
        public const string POSITIVE_ACK = "+OK<EOF>";
        public const string NEGATIVE_ACK = "-ERR<EOF>";
        
        public const string CONTROL_REQUEST = "CTRL<EOF>";
    }
}
