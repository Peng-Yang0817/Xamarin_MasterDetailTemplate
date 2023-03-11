using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MasterDetailTemplate.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class LoginAndLogoutPage : ContentPage
    {


        public App app = Application.Current as App;
        public string LogStatus = "LogStatus";

        public LoginAndLogoutPage()
        {
            InitializeComponent();
            if (app.Properties[LogStatus].ToString() == "false")
            {
                StackLayout_UserLoginFirst.IsVisible = true;
                StackLayout_UserCanLogoutNow.IsVisible = false;
            }
            else
            {
                StackLayout_UserLoginFirst.IsVisible = false;
                StackLayout_UserCanLogoutNow.IsVisible = true;
            }
        }

        /// <summary>
        /// 用戶登出功能
        /// </summary>
        public void Button_Logout_Clicked(object sender, EventArgs args)
        {

            app.Properties[LogStatus] = "false";
            app.SavePropertiesAsync();
            DisplayAlert("已經登出",
                       "登出了~~~~",
                       "OK");
            StackLayout_UserLoginFirst.IsVisible = true;
            StackLayout_UserCanLogoutNow.IsVisible = false;
        }

        /// <summary>
        /// 用戶註冊功能
        /// </summary>
        public void Button_Register_Clicked(object sender, EventArgs args)
        {
            if (true == true)
            {
                DisplayAlert("註冊成功",
                       "請使用新註冊的Email與Password到登入介面進行登入。",
                       "OK");
            }
            else
            {
                DisplayAlert("註冊失敗",
                       "失敗原因...。\n如有疑慮請...",
                       "OK");
            }

        }

        /// <summary>
        /// 用戶登入功能
        /// </summary>
        public async void Button_Login_Clicked(object sender, EventArgs args)
        {
            // 取得 XAML 中的 Entry 物件 ，並將當前內容取出
            Entry entry_Email = (Entry)FindByName("Login_Email");
            string Entry_Email = entry_Email.Text;

            Entry entry_Password = (Entry)FindByName("Login_Password");
            string Entry_Password = entry_Password.Text;


            // var status = await GetStatus(Entry_Email, Entry_Password);

            // var status = await GetLineStatus("你媽白~~~XDXD");

            var status = await GetMineServerStatus(Entry_Email, Entry_Password);


            if (status == true) // 登入成功
            {
                app.Properties[LogStatus] = "true";
                await app.SavePropertiesAsync();

                await DisplayAlert("已經登入",
                       "登入了! 可以開始進行其餘操作。",
                       "OK");

                StackLayout_UserLoginFirst.IsVisible = false;
                StackLayout_UserCanLogoutNow.IsVisible = true;

                // 登入後重置當前輸入框
                entry_Password.Text = "";
            }
            else // 登入失敗
            {
                await DisplayAlert("登入失敗",
                       "請檢察信箱與密碼是否正確。",
                       "OK");
            }

        }

        /// <summary>
        /// 將byte[] 轉成 Json的方法
        /// </summary>
        /// <param name="jsonSrting">要被轉換的 byte[]</param>
        /// <returns></returns>
        private static Dictionary<string, string> ToDictionary(string jsonSrting)
        {
            var jsonObject = JObject.Parse(jsonSrting);
            var jTokens = jsonObject.Descendants().Where(p => !p.Any());
            var tmpKeys = jTokens.Aggregate(new Dictionary<string, string>(),
                (properties, jToken) =>
                {
                    properties.Add(jToken.Path, jToken.ToString());
                    return properties;
                });
            return tmpKeys;
        }

        /// <summary>
        /// 測試Line的Server能不能返回，如果可以代表是我們自己Server的問題
        /// </summary>
        public async Task<bool> GetLineStatus(string message)
        {
            var wb = new WebClient();
            var dataSendUse = new NameValueCollection();

            string urlSendUse = "https://notify-api.line.me/api/notify";

            string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
            wb.Headers.Add("Authorization", Bearer);

            dataSendUse["message"] = "安安~~~ 連動成功拉~~!";

            var responseSendUse = await wb.UploadValuesTaskAsync(urlSendUse, "POST", dataSendUse);

            string str = Encoding.UTF8.GetString(responseSendUse);

            Dictionary<string, string> ResponseJsonData = ToDictionary(str);

            string ResponseState = ResponseJsonData["message"];

            if (ResponseState == "ok")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 跟我們架設的Server取得這email與password是否是使用者。
        /// </summary>
        /// <param name="email">接收前端的email Entry</param>
        /// <param name="password">接收前端的password Entry</param>
        /// <returns></returns>
        public async Task<bool> GetMineServerStatus(string email, string password)
        {
            var wb = new WebClient();
            var dataSendUse = new NameValueCollection();

            string urlSendUse = "http://192.168.0.80:52809/MobileService/LoginUserCheckingPost";

            string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
            wb.Headers.Add("Authorization", Bearer);

            // dataSendUse["email"] = "gg.2009@gmail.com";
            // dataSendUse["password"] = "5487XDXD";

            dataSendUse["email"] = email;
            dataSendUse["password"] = password;

            var responseSendUse = await wb.UploadValuesTaskAsync(urlSendUse, "POST", dataSendUse);

            string str = Encoding.UTF8.GetString(responseSendUse);

            Dictionary<string, string> ResponseJsonData = ToDictionary(str);

            bool ResponseState = bool.Parse(ResponseJsonData["State"]);

            return ResponseState;
        }


        /*
        public async Task<string> GetStatus(string email, string password)
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"http://192.168.0.80:52809/MobileService/LoginUserChecking?email={email}&password={password}";

                var httpResponse = await httpClient.GetAsync(url);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    // 將 JSON 格式的字串轉成對應的物件，並取出 Status 屬性的值
                    var statusValue = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, bool>>(responseContent)["Status"];

                    return statusValue.ToString();
                }
                else
                {
                    throw new Exception($"Web request failed with status code {httpResponse.StatusCode}");
                }
            }
        }
        */
    }
}