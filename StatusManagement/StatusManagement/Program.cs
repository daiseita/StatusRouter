using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using System.IO;
namespace StatusManagement
{
   

    class Program
    {
        
        public static Manage manage = null;
        static void Main(string[] args)
        {            
            if (args.Length >= 1)
            {
                Pub.AppSetting = args[0];
            }
            else {
                Pub.AppSetting = AppDomain.CurrentDomain.BaseDirectory;
            }            
            manage = new Manage();
            manage.Start();

            //string jjj = JsonConvert.SerializeObject(t);

            //string jstr = "{'data':[    {'Id':'0','Value':'100'},{'Id':'1','Value':'200'},{'Id':'2','Value':'300'}]}";

            //SourceDataModel.FileContent o = JsonConvert.DeserializeObject<SourceDataModel.FileContent>(jstr);

            //StatusFromModbus s = new StatusFromModbus("modbus", 8001);
            //s.Print_Eventhandle += S_Print_Eventhandle;
            //s.Error_Eventhandle += S_Error_Eventhandle;
            //s.StatusRceived_Eventhandle += S_StatusRceived_Eventhandle;
            //s.Run();
   
            Console.ReadLine();
        }

        private static void S_StatusRceived_Eventhandle(string data)
        {
            Console.WriteLine(data);
        }

        private static void S_Error_Eventhandle(string err)
        {
            Console.WriteLine(err);
        }

        private static void S_Print_Eventhandle(string msg)
        {
            Console.WriteLine(msg);
        }



    }

    public class HttpProvider
    {
        
    }
}
