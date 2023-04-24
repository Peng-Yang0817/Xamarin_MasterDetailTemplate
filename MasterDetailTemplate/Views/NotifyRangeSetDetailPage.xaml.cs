using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MasterDetailTemplate.Models.ServerAccessSet;
using System.Net;
using System.Collections.Specialized;
using System.Threading;
using Newtonsoft.Json;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotifyRangeSetDetailPage : ContentPage
    {
        public App app = Application.Current as App;
        public string Auth001Id = "Auth001Id";
        public string AquariumUnitNum = "AquariumUnitNum";

        private serverAccessSet server = new serverAccessSet();

        public NotifyRangeSetDetailPage()
        {
            InitializeComponent();
        }


        // ========================================================================== 覆寫區塊
        /// <summary>
        /// 頁面重新載入時觸發，更新資料參數
        /// </summary>
        protected override async void OnAppearing()
        {
            Appearing_RefreshView.IsEnabled = true;
            Appearing_RefreshView.IsRefreshing = true;
            await Task.Delay(500);
            SetAquaruimNum();
            InitAllEntryAndSwitch();

            // 取得編輯資訊
            NotifySetRange data = await SendAquaruimRangerSetNotifyStateRequest();

            // 設置不為null才開始塞資料
            if (data != null)
            {
                // 定義Entry與Switch
                GetFinishDataAndConstructEntryAndSwitch(data);
            }

            Appearing_RefreshView.IsRefreshing = false;
            Appearing_RefreshView.IsEnabled = false;
        }

        /// <summary>
        /// 覆寫離開頁面後把該頁面清空的動作
        /// </summary>
        protected override void OnDisappearing()
        {
            ReSetAquaruimNum();
            InitAllEntryAndSwitch();
        }


        // ========================================================================== 取得伺服器回傳的該魚缸的編輯狀況
        /// <summary>
        /// 向伺服器發送  魚缸的水質範圍編輯狀況 資訊
        /// </summary>
        public async Task<NotifySetRange> SendAquaruimRangerSetNotifyStateRequest()
        {
            // 獲得Properties當中目前的使用者Auth001Id
            string _Auth001Id = app.Properties[Auth001Id].ToString();

            // 準備裝伺服器回應的資料
            NotifySetRange data = new NotifySetRange();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/AquariumRangeSetGetDeatilToEdit";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                // 準備魚缸編號，用於查詢
                dataSendUse["Auth001Id"] = _Auth001Id;
                dataSendUse["AquariumNum"] = getAquariumNum;

                // 使用 CancellationTokenSource 取消等待
                var cts = new CancellationTokenSource();

                // 建立 timeoutTask 來等待 WaitTimeToServerResponese 的時間
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(server.AccessTimeOut), cts.Token);

                // 建立 responseTask 來執行伺服器回應
                var responseTask = wb.UploadValuesTaskAsync(urlSendUse, "POST", dataSendUse);

                // 等待 timeoutTask 與 responseTask 中的其中一個 Task 完成
                Task completedTask = await Task.WhenAny(timeoutTask, responseTask);

                cts.Cancel();

                if (completedTask == timeoutTask)
                {
                    await DisplayAlert("警告", "伺服器回應超時，請稍後再試。", "確定");
                    return null;
                }

                // 如果 responseTask 完成，則繼續執行下面的程式碼
                var responseSendUse = await responseTask;

                string str = Encoding.UTF8.GetString(responseSendUse);

                // 解析JSON string
                data = JsonConvert.DeserializeObject<NotifySetRange>(str);

                return data;
            }
        }


        // ========================================================================== 自定義方法區
        /// <summary>
        /// 初始化所有輸入
        /// </summary>
        public void InitAllEntryAndSwitch()
        {
            Switch_NotifyIsTaggled.IsToggled = false;

            Entry_WaterLowerBound.Text = "";
            Entry_TurbidityUpperBound.Text = "";
            Entry_TemperatureUpperBound.Text = "";
            Entry_TemperatureLowerBound.Text = "";
            Entry_PhUpperBound.Text = "";
            Entry_PhLowerBound.Text = "";
            Entry_TdsUppderBound.Text = "";
            Entry_TdsLowerBound.Text = "";

            // 收起鍵盤
            Entry_WaterLowerBound.Unfocus();
            Entry_TurbidityUpperBound.Unfocus();
            Entry_TemperatureUpperBound.Unfocus();
            Entry_TemperatureLowerBound.Unfocus();
            Entry_PhUpperBound.Unfocus();
            Entry_PhLowerBound.Unfocus();
            Entry_TdsUppderBound.Unfocus();
            Entry_TdsLowerBound.Unfocus();
        }

        // 取得資料後開始定義Entry與Switch
        public void GetFinishDataAndConstructEntryAndSwitch(NotifySetRange data)
        {
            // 為一代表有開啟
            if (data.NotifyTag == 1.ToString())
            {
                Switch_NotifyIsTaggled.IsToggled = true;
            }
            else
            {
                Switch_NotifyIsTaggled.IsToggled = false;
            }

            Entry_WaterLowerBound.Text = data.WaterLevelLowerBound;
            Entry_TurbidityUpperBound.Text = data.TurbidityUpperBound;
            Entry_TemperatureUpperBound.Text = data.temperatureUpperBound;
            Entry_TemperatureLowerBound.Text = data.temperatureLowerBound;
            Entry_PhUpperBound.Text = data.pHUpperBound;
            Entry_PhLowerBound.Text = data.phLowerBound;
            Entry_TdsUppderBound.Text = data.TDSUpperBound;
            Entry_TdsLowerBound.Text = data.TDSLowerBound;

            // 收起鍵盤
            Entry_WaterLowerBound.Unfocus();
            Entry_TurbidityUpperBound.Unfocus();
            Entry_TemperatureUpperBound.Unfocus();
            Entry_TemperatureLowerBound.Unfocus();
            Entry_PhUpperBound.Unfocus();
            Entry_PhLowerBound.Unfocus();
            Entry_TdsUppderBound.Unfocus();
            Entry_TdsLowerBound.Unfocus();
        }

        /// <summary>
        /// 設定魚缸標題編號LABLE
        /// </summary>
        public void SetAquaruimNum()
        {
            Lable_AquariumNum.Text = "魚缸編號 : " + getAquariumNum;
        }

        /// <summary>
        /// 清空魚缸標題編號LABLE
        /// </summary>
        public void ReSetAquaruimNum()
        {
            Lable_AquariumNum.Text = "魚缸編號 : ";
        }

        /// <summary>
        /// 返回當前欲設置的魚缸編號
        /// </summary>
        public string getAquariumNum =>
            // 獲得Properties當中目前欲查詢的AquariumUnitNum
            app.Properties[AquariumUnitNum].ToString();

    }

    // ========================================================================== 自定義類別區
    // 取得該魚缸水質通資設置的資訊 類別
    public class NotifySetRange
    {
        public int Id { get; set; }
        public string AquariumUnitNum { get; set; }
        public string temperatureUpperBound { get; set; }
        public string temperatureLowerBound { get; set; }
        public string pHUpperBound { get; set; }
        public string phLowerBound { get; set; }
        public string TDSUpperBound { get; set; }
        public string TDSLowerBound { get; set; }
        public string TurbidityUpperBound { get; set; }
        public string WaterLevelLowerBound { get; set; }
        public string NotifyTag { get; set; }
    }
}