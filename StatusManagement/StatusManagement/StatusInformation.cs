using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusManagement
{
    public  class StatusInformation
    {
        /// <summary>
        /// 資料來源app
        /// </summary>
        public string Source;
        /// <summary>
        /// 點位識別碼
        /// </summary>
        public string Id;
        /// <summary>
        /// 當前狀態值
        /// </summary>
        public string Value;
        /// <summary>
        /// 時間
        /// </summary>
        public string Tiimestamp;
        /// <summary>
        /// 前一次狀態值
        /// </summary>
        public string LastValue;
        /// <summary>
        /// 前一次時間
        /// </summary>
        public string LastTiimestamp;
        /// <summary>
        /// 狀態
        /// </summary>
        public bool OnOff = false ;
        /// <summary>
        /// 是否有狀態變更
        /// </summary>
        public bool HasChange = false;
        
    }
}

