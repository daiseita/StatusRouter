using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
namespace StatusManagement
{
    public  class HttpStatusResponse
    {
        private Thread nThread;
        private StautCenter nStatusCenter;
        private  string nPort;
        public string  Port
        {
            get { return nPort; }
            set { nPort = value; }
        }
        public StautCenter StautCenter
        {            
            set { nStatusCenter = value; }
        }
        public void Run() {

            nThread = new Thread(start);
            nThread.Start();
        }

        public void start()
        {

            HttpListener listener = new HttpListener();
            try
            {
                listener.Prefixes.Add("http://*:" + nPort + "/status/"); //要監聽的URL範圍
                listener.Start(); //開始監聽端口，接收客户端請求
                Console.WriteLine("[HttpStatusResponse]  run at : " + nPort);
                while (true)
                {
                    //獲取一個客户端請求為止
                    HttpListenerContext context = listener.GetContext();
                    //將其處理過程放入線程池(Pool)
                    System.Threading.ThreadPool.QueueUserWorkItem(ProcessHttpClient, context);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            finally
            {
                listener.Stop();
            }
        }
        private void ProcessHttpClient(object obj)
        {
            HttpListenerContext context = obj as HttpListenerContext;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
           
            DisplayJsonFormat objData = new DisplayJsonFormat();
            foreach (KeyValuePair<string, StatusInformation > i in  nStatusCenter.DicStatusInformation){
                DisplayJsonFormat.DisplayData data = new DisplayJsonFormat.DisplayData();
                data.id = i.Value.Id;
                data.updateTime = i.Value.Tiimestamp ;
                data.value = i.Value.Value;                
                objData.DataList.Add(data);
            }
            string json = JsonConvert.SerializeObject(objData);
            
            string responseString = json;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            
            output.Close();
        }

        public class DisplayJsonFormat {

            public List<DisplayData> DataList = new List<DisplayData>();
            
            public class DisplayData {
                public string id;
                public string updateTime;
                public string value;
                
            }
        }
    }
}
