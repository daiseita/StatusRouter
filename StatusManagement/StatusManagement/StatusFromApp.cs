using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace StatusManagement
{
    public  class StatusFromApp:IStatus
    {
        public delegate void Error(string err);
        public delegate void Print(string msg);
        public delegate void StatusRceived(string data);
        public event StatusRceived StatusRceivedEventhandle;
        public event Print PrintEventhandle;
        public event Error ErrorEventhandle;

        public string SourceName { get; set; }
        public int ListenOnUDP { get; set; }
        

        private Thread nThread = null;
        public StatusFromApp( string sourcename,int port) {

            this.SourceName = sourcename;
            this.ListenOnUDP = port;                        
        }
       

        public void Run() {
            nThread = new Thread(Listen);
            nThread.Start();
        }
        public void SocketReceived(object o) {
            
            StatusRceivedEventhandle((string)o);
        }

        public void Listen()
        {
            PrintEventhandle($"[{this.SourceName}] start listen at upd : {this.ListenOnUDP }");
            try
            {
                while (true)
                {                    
                    int port = this.ListenOnUDP;
                    UdpClient udpclient = new UdpClient(port);
                    IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, port);
                    byte[] bytes = udpclient.Receive(ref ipendpoint);
                    string data = Encoding.UTF8.GetString(bytes, 0, bytes.Length);                                        
                    if (data != null && data != "") {                        
                        ThreadPool.QueueUserWorkItem(new WaitCallback(SocketReceived) ,data);
                    }                    
                    udpclient.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorEventhandle("UDPServer 啟動失敗");                                
            }
        }

    }
}
