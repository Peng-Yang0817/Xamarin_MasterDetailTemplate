using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MasterDetailTemplate.Models.ServerAccessSet;
using Newtonsoft.Json;
using MasterDetailTemplate.Services;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomAquariumName : ContentPage
    {
        public App app = Application.Current as App;
        private serverAccessSet server = new serverAccessSet();
        public string AquariumUnitNum = "AquariumUnitNum";
        public string Auth001Id = "Auth001Id";

        // 準備轉跳頁面所需的物件
        private static ContentPageActivationService ContentPageActivationService =
            new ContentPageActivationService();
        private static ContentNavigationService ContentNavigationService =
            new ContentNavigationService(ContentPageActivationService);

        public CustomAquariumName()
        {
            InitializeComponent();
        }

        // ============================================================================================== 複寫區域
        /// <summary>
        /// 覆寫重新進到該頁面後的刷新。
        /// </summary>
        protected override void OnAppearing()
        {
            // 設定魚缸標題
            SetAquaruimNum();
        }

        /// <summary>
        /// 覆寫離開頁面後把該頁面清空的動作
        /// </summary>
        protected override void OnDisappearing()
        {
            ClearCustomNameEntry();
        }
        // ============================================================================================== 按鈕區域
        /// <summary>
        /// 送出按鈕被點擊後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // 取得用戶填寫的魚缸名稱
            Entry Entry_CustomName = (Entry)FindByName("CustomName");
            string str_CustomName = Entry_CustomName.Text;

            // 要求用戶一定要填寫
            if (str_CustomName == null ||
                str_CustomName == "")
            {
                await DisplayAlert("注意", "請填入完整的資訊!", "確定");
            }
            else // 都有填寫
            {
                Appearing_RefreshView.IsEnabled = true;
                Appearing_RefreshView.IsRefreshing = true;

                ResponseState responseState = await GetMyAquaruimSituationsData(str_CustomName);

                if (responseState.state == false)
                {
                    await DisplayAlert("錯誤", responseState.msg, "確定");
                }

                Appearing_RefreshView.IsEnabled = false;
                Appearing_RefreshView.IsRefreshing = false;
                await ContentNavigationService.PopNavigateAsync();
            }
        }


        // ============================================================================================== 請求API
        /// <summary>
        /// 向Server取得使用者魚缸資訊，主要是取得該用戶有那些魚缸
        /// </summary>
        public async Task<ResponseState> GetMyAquaruimSituationsData(string str_CustomName)
        {
            // 獲得Properties當中目前的使用者Auth001Id
            string _Auth001Id = app.Properties[Auth001Id].ToString();

            // 取得魚缸名稱設定
            string _customName = str_CustomName;

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/CustomAquariumNamePost";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                dataSendUse["AquariumUnitNum"] = getAquariumNum;
                dataSendUse["customAquaruimName"] = _customName;

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
                    return new ResponseState
                    {
                        state = false,
                        msg = "連接超時!"
                    };
                }

                // 如果 responseTask 完成，則繼續執行下面的程式碼
                var responseSendUse = await responseTask;

                string str = Encoding.UTF8.GetString(responseSendUse);

                // 解析JSON string
                ResponseState responseState = JsonConvert.DeserializeObject<ResponseState>(str);

                return responseState;
            }
        }

        // ============================================================================================== 自定義範本
        /// <summary>
        /// 設定魚缸標題編號LABLE
        /// </summary>
        public void SetAquaruimNum()
        {
            AquariumNum.Text = "魚缸編號 : " + getAquariumNum;
        }

        /// <summary>
        /// 返回當前欲查詢的魚缸編號
        /// </summary>
        public string getAquariumNum =>
            // 獲得Properties當中目前欲查詢的AquariumUnitNum
            app.Properties[AquariumUnitNum].ToString();

        /// <summary>
        /// 離開頁面時，清空魚缸名稱輸入的內容
        /// </summary>
        public void ClearCustomNameEntry()
        {
            CustomName.Text = "";
        }
    }

    public class ResponseState
    {
        // 狀態
        public bool state { get; set; }
        // 資訊
        public string msg { get; set; }
    }
}