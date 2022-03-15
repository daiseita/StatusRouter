using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusManagement
{
    public class SourceDataModel
    {
        //udp接收資料格式
        //{
        //"SourcName" :"Modbus",
        //"CmdType":"Address",
        //"Information":"{'data':[{'SysId':'0','Value':'0'},{'SysId':'1','Value':'0'},{'SysId':'2','Value':'353'}]}",
        //"Timestamp":"20220106"
        //}
        public string SourcName;
        public string CmdType;
        public string Information;
        public string Timestamp;


        /// <summary>
        /// Information格位解析  CmdType=FILE 檔案傳送JSON格式
        /// </summary>
        public class FileCmd {
            public string Location;
            public string Filename;
        }
        /// <summary>
        /// 接收資料內容格式
        /// </summary>
        /// {
        /// 'data':[
        ///    {'SourceName':'modbuns','SysId':'0','Value':'100'},
        ///    {'SourceName':'modbuns','SysId':'1','Value':'200'},
        ///    {'SourceName':'modbuns','SysId':'2','Value':'300'}
        /// ]
        ///}
        public class SourceDataFormat1
        {
            public  List<Address> Data = new List<Address> ();


            public class Address
            {
                public string SourceName;
                public string SysId;
                public string Value;
            }
        }
      
    }
}
