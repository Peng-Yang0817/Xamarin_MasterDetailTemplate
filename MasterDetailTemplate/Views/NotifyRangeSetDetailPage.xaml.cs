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
            SendButton.IsEnabled = false;

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

                Switch_NotifyIsTaggled.IsToggled = false;
                if (data.NotifyTag == "1")
                {
                    Switch_NotifyIsTaggled.IsToggled = true;
                }
                SendButton.IsEnabled = true;
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



        /// <summary>
        ///  向Server發送更新魚缸通知上下限的請求
        /// </summary>
        //{
        //    "AquariumUnitNum": "AAAAAAAA11111111",
        //    "temperatureUpperBound": "",
        //    "temperatureLowerBound": "",
        //    "pHUpperBound": "",
        //    "phLowerBound": "",
        //    "TDSUpperBound": "",
        //    "TDSLowerBound": "",
        //    "TurbidityUpperBound": "",
        //    "WaterLevelLowerBound": "",
        //    "NotifyTag": ""
        // }
        public async Task<ReturnMsg> SendAquariumRangeSetRequest(NotifySetRange data)
        {
            // 準備裝伺服器回應的資料
            ReturnMsg dataResponse = new ReturnMsg();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/SetAquaruimRangeService";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                // 準備魚缸更新所要的資訊
                dataSendUse["AquariumUnitNum"] = data.AquariumUnitNum;
                dataSendUse["temperatureUpperBound"] = data.temperatureUpperBound;
                dataSendUse["temperatureLowerBound"] = data.temperatureLowerBound;
                dataSendUse["pHUpperBound"] = data.pHUpperBound;
                dataSendUse["phLowerBound"] = data.phLowerBound;
                dataSendUse["TDSUpperBound"] = data.TDSUpperBound;
                dataSendUse["TDSLowerBound"] = data.TDSLowerBound;
                dataSendUse["TurbidityUpperBound"] = data.TurbidityUpperBound;
                dataSendUse["WaterLevelLowerBound"] = data.WaterLevelLowerBound;
                dataSendUse["NotifyTag"] = data.NotifyTag;

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
                dataResponse = JsonConvert.DeserializeObject<ReturnMsg>(str);

                return dataResponse;
            }
        }


        // ========================================================================== 自定義方法區
        /// <summary>
        /// 按下送出按鈕
        /// </summary>
        public async void Button_SendQualityNotifyRange_Clicked(object sender, EventArgs e)
        {

            // 取得目前所有的輸入
            var valueText_WaterLowerBound = Entry_WaterLowerBound.Text;
            var valueText_TurbidityUpperBound = Entry_TurbidityUpperBound.Text;
            var valueText_TemperatureUpperBound = Entry_TemperatureUpperBound.Text;
            var valueText_TemperatureLowerBound = Entry_TemperatureLowerBound.Text;
            var valueText_PhUpperBound = Entry_PhUpperBound.Text;
            var valueText_PhLowerBound = Entry_PhLowerBound.Text;
            var valueText_TdsUppderBound = Entry_TdsUppderBound.Text;
            var valueText_TdsLowerBound = Entry_TdsLowerBound.Text;

            // 定義裝浮點數的參數
            double temperatureUpperBound,
                temperatureLowerBound,
                phUpperBound,
                phLowerBound,
                tdsUpperBound,
                tdsLowerBound,
                turbidityUpperBound,
                waterLevelLowerBound;

            Dictionary<string, string> waterLevel = new Dictionary<string, string>();
            waterLevel.Add("1", "低水位");
            waterLevel.Add("2", "中水位");

            // 確定傳過來的都為浮點數，並且水位高低通知是合理的
            if (double.TryParse(valueText_TemperatureUpperBound, out temperatureUpperBound) &&
                double.TryParse(valueText_TemperatureLowerBound, out temperatureLowerBound) &&
                double.TryParse(valueText_PhUpperBound, out phUpperBound) &&
                double.TryParse(valueText_PhLowerBound, out phLowerBound) &&
                double.TryParse(valueText_TdsUppderBound, out tdsUpperBound) &&
                double.TryParse(valueText_TdsLowerBound, out tdsLowerBound) &&
                double.TryParse(valueText_TurbidityUpperBound, out turbidityUpperBound) &&
                double.TryParse(valueText_WaterLowerBound, out waterLevelLowerBound) &&
                waterLevel.ContainsKey(valueText_WaterLowerBound))
            {


                // 打包要傳送的資料
                NotifySetRange data = new NotifySetRange
                {
                    AquariumUnitNum = getAquariumNum,
                    temperatureUpperBound = temperatureUpperBound.ToString(),
                    temperatureLowerBound = temperatureLowerBound.ToString(),
                    pHUpperBound = phUpperBound.ToString(),
                    phLowerBound = phLowerBound.ToString(),
                    TDSUpperBound = tdsUpperBound.ToString(),
                    TDSLowerBound = tdsLowerBound.ToString(),
                    TurbidityUpperBound = turbidityUpperBound.ToString(),
                    WaterLevelLowerBound = waterLevelLowerBound.ToString()
                };

                // 查看用戶是否想開啟通知
                bool notifyState = Switch_NotifyIsTaggled.IsToggled;
                if (notifyState)
                {
                    data.NotifyTag = "1";
                }
                else
                {
                    data.NotifyTag = "0";
                }

                // 發資料到Server，等候Server回傳狀態與資訊
                ReturnMsg returnMsg = await SendAquariumRangeSetRequest(data);

                if (returnMsg != null)
                {
                    if (returnMsg.Status)
                    {
                        await DisplayAlert("完成", returnMsg.Message.ToString(), "確定");
                        OnAppearing();
                    }
                    else
                    {
                        await DisplayAlert("錯誤", returnMsg.Message.ToString(), "確定");
                    }
                }
            }
            else
            {
                await DisplayAlert("錯誤", "填寫參數異常，請填寫對應整數或浮點數。", "確定");
            }

        }

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

    // 自定義:
    // 接收到的訊息格式
    public class ReturnMsg
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Auth001Id { get; set; }
        public string UserName { get; set; }
    }
}