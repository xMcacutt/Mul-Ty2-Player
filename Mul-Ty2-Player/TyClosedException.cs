using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MT2PClient
{
    public class TyClosedException : Exception
    {
        public override string Message => "Ty the Tasmanian Tiger has closed or stopped responding. Waiting for Ty to reopen.";
    }
}
