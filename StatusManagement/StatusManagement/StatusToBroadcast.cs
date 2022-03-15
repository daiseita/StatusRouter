using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Microsoft.VisualBasic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace StatusManagement
{
    public class StatusToBroadcast
    {
        public string AppName { get; set; }
        private System.Threading.Thread thisSendThread = null;
        private ConcurrentQueue<string> thisSendTQueue_Txt = new ConcurrentQueue<string>();
        private ConcurrentQueue<byte[]> thisSendTQueue_Byte = new ConcurrentQueue<byte[]>();
        public int EncodingCode { get; set; } = 950;

        public StatusToBroadcast()
        {
            thisSendThread = new System.Threading.Thread(_Send_Pool);
            thisSendThread.Start();
        }

        
        

        private Dictionary<string, IPEndPoint> thisInfo = new Dictionary<string, IPEndPoint>();
        public void Add(string strIp, int intPort)
        {
            ////IPAddress mIP = HsienYang_CommonNet.IPAddress_Pars(strIp);
            ////if (IsNothing(mIP) == true)
            ////    return;
            IPAddress mIP = IPAddress.Parse(strIp);
            IPEndPoint mIPEP = new IPEndPoint(mIP, intPort);
            string strKey = mIPEP.ToString();
            if (this.thisInfo.ContainsKey(strKey) == false)
                this.thisInfo.Add(strKey, mIPEP);
        }

        public void Send(string strTxt)
        {
            if (string.IsNullOrEmpty(strTxt) == true)
                return;
            thisSendTQueue_Txt.Enqueue(strTxt);
        }

        public void Send(byte[] arrByte)
        {
            //if (Information.IsNothing(arrByte) == true)
            //    return;
            if (arrByte.Length <= 0)
                return;
            thisSendTQueue_Byte.Enqueue(arrByte);
        }

       

        private void _Send_Pool()
        {
            do
            {
                string strTxt = "";
                if (thisSendTQueue_Txt.TryDequeue(out strTxt) == true)
                {
                    IPEndPoint[] arrIPEP = this.thisInfo.Values.ToArray();
                    foreach (IPEndPoint mIPEP in arrIPEP)
                        this._Send_Go(mIPEP, strTxt);
                }

                byte[] arrByte = null;
                if (thisSendTQueue_Byte.TryDequeue(out arrByte) == true)
                {
                    IPEndPoint[] arrIPEP = this.thisInfo.Values.ToArray();
                    foreach (IPEndPoint mIPEP in arrIPEP)
                        this._Send_Go(mIPEP, arrByte);
                }

                Thread.Sleep(1);
            }
            while (true);
        }

        private void _Send_Go(IPEndPoint mIPEP, byte[] arrByte)
        {
            string strLogTag_1 = "Send Bytes at " + mIPEP.ToString();
            try
            {
                UdpClient mUDPClient = new UdpClient();
                if (arrByte.Length > 0)
                    mUDPClient.Send(arrByte, arrByte.Length, mIPEP);
                else
                {
                }
                mUDPClient.Close();
            }
            catch (Exception ex)
            {
            }
        }

        private void _Send_Go(IPEndPoint mIPEP, string strTxt)
        {
            if (string.IsNullOrEmpty(strTxt) == true)
                return;
            string strLogTag_1 = "Send at " + mIPEP.ToString();
            try
            {
                byte[] bytSend = Encoding.GetEncoding(EncodingCode).GetBytes(strTxt);

                _Send_Go(mIPEP, bytSend);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
