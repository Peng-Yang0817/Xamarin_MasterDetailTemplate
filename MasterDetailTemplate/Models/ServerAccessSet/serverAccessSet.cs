using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace MasterDetailTemplate.Models.ServerAccessSet
{
    public class serverAccessSet
    {
        // 伺服器的位置
        public  string ServerIP = "http://192.168.0.80:52809";
        // 等待伺服器回應的時間設定
        public  int AccessTimeOut = 5;

        /// <summary>
        /// 發出請求之前先檢查手機是否有網路連線
        /// </summary>
        public bool IsNetworkAvailable()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
                return true;

            return false;
        }
    }
}
