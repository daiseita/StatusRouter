using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusManagement
{    
    public  interface IStatus
    {        
        string SourceName { get; set; }
        int ListenOnUDP { get; set; }        
        void Run();
        void Listen();
    }
}
