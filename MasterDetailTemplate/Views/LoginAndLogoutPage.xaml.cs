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
using MasterDetailTemplate.Models.LoginAndLogoutPage;
using MasterDetailTemplate.Models.ServerAccessSet;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class LoginAndLogoutPage : ContentPage
    {

        public App app = Application.Current as App;
        public string LogStatus = "LogStatus";
        public string UserName = "UserName";
        public string LoginUser = "";
        public string Auth001Id = "Auth001Id";

        private serverAccessSet server = new serverAccessSet();

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
                label_UserName.Text = app.Properties[UserName].ToString();
            }
        }



        /// <summary>
        /// 用戶登出功能
        /// </summary>
        public void Button_Logout_Clicked(object sender, EventArgs args)
        {
            Create_Username.Text = "";
            Create_Password.Text = "";
            Create_Email.Text = "";
            Create_LineID.Text = "";

            label_UserName.Text = "";

            app.Properties[LogStatus] = "false";
            app.Properties[UserName] = "NULL";
            app.Properties[Auth001Id] = "NULL";
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
        public async void Button_Register_Clicked(object sender, EventArgs args)
        {
            // 取得 XAML 中的 Entry 物件 ，並將當前內容取出
            Entry create_Username = (Entry)FindByName("Create_Username");
            string Create_Username = create_Username.Text;

            Entry create_Password = (Entry)FindByName("Create_Password");
            string Create_Password = create_Password.Text;

            Entry create_Email = (Entry)FindByName("Create_Email");
            string Create_Email = create_Email.Text;

            Entry create_LineID = (Entry)FindByName("Create_LineID");
            string Create_LineID = create_LineID.Text;

            var returnMsg = await GetMineRegisterReturnMsg(Create_Username, Create_Password, Create_Email, Create_LineID);
            string message = returnMsg.Message;

            if (returnMsg.Status == true)
            {
                await DisplayAlert("成功",
                       message,
                       "OK");
            }
            else
            {
                await DisplayAlert("失敗",
                       message,
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

            var returnMsg = await GetMineServerResponse(Entry_Email, Entry_Password);


            if (returnMsg.Status == true) // 登入成功
            {
                app.Properties[LogStatus] = "true";
                app.Properties[UserName] = returnMsg.UserName;
                app.Properties[Auth001Id] = returnMsg.Auth001Id;
                await app.SavePropertiesAsync();

                label_UserName.Text = LoginUser;
                StackLayout_UserLoginFirst.IsVisible = false;
                StackLayout_UserCanLogoutNow.IsVisible = true;

                await DisplayAlert("已經登入",
                       "登入了! 可以開始進行其餘操作。",
                       "OK");
                

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
        public async Task<ReturnMsg> GetMineServerResponse(string email, string password)
        {
            var wb = new WebClient();
            var dataSendUse = new NameValueCollection();

            string urlSendUse = server.ServerIP + "/MobileService/LoginUserCheckingPost";

            string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
            wb.Headers.Add("Authorization", Bearer);

            // dataSendUse["email"] = "gg.2009@gmail.com";
            // dataSendUse["password"] = "5487XDXD";

            dataSendUse["email"] = email;
            dataSendUse["password"] = password;

            var responseSendUse = await wb.UploadValuesTaskAsync(urlSendUse, "POST", dataSendUse);

            string str = Encoding.UTF8.GetString(responseSendUse);

            Dictionary<string, string> ResponseJsonData = ToDictionary(str);

            ReturnMsg returnMsg = new ReturnMsg();

            returnMsg.Status = bool.Parse(ResponseJsonData["Status"]);
            returnMsg.Message = ResponseJsonData["Message"];
            returnMsg.Auth001Id = ResponseJsonData["Auth001Id"];
            returnMsg.UserName = ResponseJsonData["UserName"];

            LoginUser = ResponseJsonData["UserName"].ToString();

            return returnMsg;
        }

        public async Task<ReturnMsg> GetMineRegisterReturnMsg(string userName, string password,
                                                              string email, string lineID)
        {
            var wb = new WebClient();
            var dataSendUse = new NameValueCollection();

            string urlSendUse = server.ServerIP + "/MobileService/RegisterUser";

            string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
            wb.Headers.Add("Authorization", Bearer);

            // dataSendUse["email"] = "gg.2009@gmail.com";
            // dataSendUse["password"] = "5487XDXD";

            dataSendUse["username"] = userName;
            dataSendUse["password"] = password;
            dataSendUse["email"] = email;
            dataSendUse["lineID"] = lineID;

            var response = await wb.UploadValuesTaskAsync(urlSendUse, "POST", dataSendUse);

            string str = Encoding.UTF8.GetString(response);

            Dictionary<string, string> ResponseJsonData = ToDictionary(str);

            ReturnMsg returnMsg = new ReturnMsg();
            returnMsg.Status = bool.Parse(ResponseJsonData["Status"]);
            returnMsg.Message = ResponseJsonData["Message"].ToString();

            // TODO
            int a = 3;

            return returnMsg;
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