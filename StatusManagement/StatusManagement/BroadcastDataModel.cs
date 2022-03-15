using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusManagement
{
    public  class BroadcastDataModel
    {        
        public string CmdType;
        public string Information;
        public string Timestamp;

        public class FileCmd {
            public string Location;
            public string Filename;
        }
        public class AddressCmd {
            public string SysId;
            public string Value;
        }                    
    }
}
