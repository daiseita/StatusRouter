using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security;
using System.Security.Principal;
namespace StatusManagement
{
    public  class Manage
    {
        public StautCenter nStsCenter = new StautCenter();
        public HttpStatusResponse nHttpRespon = new HttpStatusResponse();
        
        public Manage() {
            nStsCenter.DicSource = new Dictionary<string, StatusFromApp>();
            nStsCenter.DicBroadcast  = new Dictionary<string, StatusToBroadcast>();
        }
        
        public void Start() {
            string fname = Path.Combine(Pub.AppSetting, "StatusManagement.json");
           
            if (File.Exists(fname) == false) {
                Console.WriteLine("找不到設定檔-1");
                return;
            }

            StreamReader sr = new StreamReader(fname, Encoding.UTF8 );            
            string str = sr.ReadToEnd();
            sr.Close();
            if (str != "") {
                JObject jo = JObject.Parse(str);
                var obj = jo["StatusSource"];                
                for (int i = 0; i < obj.Count();i++) {
                    try
                    {
                        StatusFromApp o = new StatusFromApp(obj[i]["Name"].ToString(), Convert.ToInt32(obj[i]["Port"]));
                        o.StatusRceivedEventhandle += O_StatusRceivedEventhandle; ;
                        o.PrintEventhandle += O_PrintEventhandle; ;
                        o.ErrorEventhandle += O_ErrorEventhandle; ;
                        o.Run();
                        if (nStsCenter.DicSource.ContainsKey(obj[i]["Name"].ToString().ToUpper()) == false) {
                            nStsCenter.DicSource.Add(obj[i]["Name"].ToString().ToUpper(), o);
                        }                        
                    }
                    catch (Exception ex) {

                    }                   
                }
                var obj2= jo["BroadcastTo"];
                for (int i = 0; i < obj.Count(); i++) {
                    try {
                        StatusToBroadcast b = new StatusToBroadcast();
                        b.AppName = obj2[i]["Name"].ToString();
                        string key = obj2[i]["Name"].ToString();                        
                        for (int a = 0; a < obj2[i]["Apps"].Count(); a++) {
                            Console.WriteLine("Broadcast to "+ obj2[i]["Apps"][a]["Name"] + " Port:" +obj2[i]["Apps"][a]["Port"]);                            
                            b.Add(obj2[i]["Apps"][a]["Ip"].ToString(), (int)obj2[i]["Apps"][a]["Port"]);
                        }

                        nStsCenter.DicBroadcast.Add(b.AppName.ToUpper(), b);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
                if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
                {

                    var httpresp = jo["StatusResponseToHttp"];
                    if (httpresp["HttpPort"].ToString() != "" && httpresp["HttpPort"].ToString() != null)
                    {
                        nHttpRespon.Port = httpresp["HttpPort"].ToString();
                        nHttpRespon.StautCenter = nStsCenter;
                        nHttpRespon.Run();
                    }
                }
                else {
                    Console.WriteLine("====程式不具備系統管理者權限,無法提供http狀態資料顯示服務====");
                }
            }
        }
       

        private void O_ErrorEventhandle(string err)
        {
            throw new NotImplementedException();
        }

        private void O_PrintEventhandle(string msg)
        {
            Console.WriteLine(msg);
        }

        private void O_StatusRceivedEventhandle(string data)
        {
            this.nStsCenter.ReadInformation(data);            
        }

        
    }



    public class StautCenter {
        public Dictionary<string, StatusInformation> DicStatusInformation = new Dictionary<string, StatusInformation>();
        public Dictionary<string, StatusFromApp> DicSource { set;get;}
        public Dictionary<string, StatusToBroadcast> DicBroadcast { set; get; }

        private void StatusReceivedEvent(string source, string id, StatusInformation s)
        {
            if (DicBroadcast.ContainsKey(source.ToUpper()) == true) {
                BroadcastDataModel b = new BroadcastDataModel() { CmdType = "Address", Information = "", Timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") };
                BroadcastDataModel.AddressCmd ad = new BroadcastDataModel.AddressCmd { SysId = id, Value = s.Value };
                b.Information = JsonConvert.SerializeObject(ad);
                string sJson = JsonConvert.SerializeObject(b);
                
                DicBroadcast[source.ToUpper()].Send(sJson);
                
            }
            
        }

        private void StatusChangeEvent( string source, string id, StatusInformation s) {

            Console.WriteLine($"app {source} - {id} change from {s.LastValue} -> {s.Value }");
        }

        private void CreateRecord(string source, string id,string value){
            StatusInformation s = new StatusInformation();
            s.Source = source;
            s.Id = id;
            s.Value = value;
            s.Tiimestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            s.HasChange = false;
            s.LastValue = "";
            s.LastTiimestamp = "";
            if (value.ToUpper() == "TRUE" || value.ToUpper() == "1")
            {
                s.OnOff = true;
            }
            else {
                s.OnOff = false;
            }
            DicStatusInformation.Add(id, s);           
            StatusReceivedEvent(source, id, s);
            if (s.HasChange == true) { StatusChangeEvent(source, id, s); }
        }
        private void FreshRecord( string source, string id, string value)
        {
            StatusInformation l = DicStatusInformation[id];
            StatusInformation s = new StatusInformation();
            if (value != l.Value) {s.HasChange = true;}            
            s.LastValue = l.Value;
            s.LastTiimestamp = l.Tiimestamp;
            if (value.ToUpper() == "TRUE" || value.ToUpper() == "1")
            {
                s.OnOff = true;
            }
            else
            {
                s.OnOff = false;
            }
            s.Source = source;
            s.Id = id;
            s.Value = value;
            s.Tiimestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            DicStatusInformation[id] = s;
            StatusReceivedEvent(source, id, s);
            if (s.HasChange == true) { StatusChangeEvent(source, id, s); }
        }

        public void WriteAddress( SourceDataModel.SourceDataFormat1.Address    addr) {
            if (DicStatusInformation.ContainsKey(addr.SysId) == false)
            {
                Console.WriteLine($"CreateRecord {addr.SysId} valus {addr.Value }");
                CreateRecord( addr.SourceName, addr.SysId, addr.Value);                
            }
            else {
                Console.WriteLine($"FreshRecord {addr.SysId} valus {addr.Value }");
                FreshRecord( addr.SourceName , addr.SysId, addr.Value);                
            }            
        }


        public  void ReadInformation(string data)
        {            
            try
            {
                SourceDataModel o = JsonConvert.DeserializeObject<SourceDataModel>(data);
                string type = o.CmdType.ToString().ToUpper ();
                string sourcName = o.SourcName;
                string content = "";
                if (type == "FILE") {
                    SourceDataModel.FileCmd f = JsonConvert.DeserializeObject<SourceDataModel.FileCmd>(o.Information);
                    StreamReader sr = new StreamReader(Path.Combine(f.Location ,f.Filename ), Encoding.UTF8);
                    content = sr.ReadToEnd();
                    sr.Close();
                }
                if (type == "ADDRESS") { content = o.Information; }
                if(content != "")
                {
                    SourceDataModel.SourceDataFormat1 rows = JsonConvert.DeserializeObject<SourceDataModel.SourceDataFormat1>(content);
                    for (int i = 0; i < rows.Data.Count; i++)
                    {

                        this.WriteAddress(new SourceDataModel.SourceDataFormat1.Address() { SourceName = sourcName, SysId  = rows.Data[i].SysId, Value = rows.Data[i].Value });
                    }


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
