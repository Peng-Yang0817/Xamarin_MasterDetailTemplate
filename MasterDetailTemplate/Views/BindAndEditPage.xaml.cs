using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MasterDetailTemplate.Models.BindAquarium;
using System.Net;
using System.Collections.Specialized;
using MasterDetailTemplate.Models.ServerAccessSet;
using System.Threading;
using Newtonsoft.Json;
using MasterDetailTemplate.Models.AquariumWaterSituationPage;
using MasterDetailTemplate.Services;
using Xamarin.Essentials;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BindAndEditPage : ContentPage
    {
        public App app = Application.Current as App;
        public string Auth001Id = "Auth001Id";
        public string AquariumUnitNum = "AquariumUnitNum";
        static private int WaterType_Select = 2;

        // 準備轉跳頁面所需的物件
        private static ContentPageActivationService ContentPageActivationService =
            new ContentPageActivationService();
        private static ContentNavigationService ContentNavigationService =
            new ContentNavigationService(ContentPageActivationService);

        public static Dictionary<string, int> WaterTypeStringToInt = new Dictionary<string, int>();


        private serverAccessSet server = new serverAccessSet();

        public BindAndEditPage()
        {
            InitializeComponent();

            WaterTypeStringToInt.Add("中性", 2);
            WaterTypeStringToInt.Add("弱酸", 1);
            WaterTypeStringToInt.Add("弱鹼", 3);
        }

        /// <summary>
        /// 覆寫重新進到該頁面後的刷新。
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!server.IsNetworkAvailable())
            {
                await DisplayAlert("警告", "請先連接至網際網路。", "確定");
            }
            else
            {
                clearAllEntry();
                ClearStackLayoutData();

                Appearing_RefreshView.IsEnabled = true;
                Appearing_RefreshView.IsRefreshing = true;
                await Task.Delay(300);
                Select_AquariumTypeId.SelectedIndex = 0;
                // 取的該用戶所擁有的魚缸資料
                List<ViewAquaruimSituation_ForMobile> DataList = await GetMyAquaruimSituationsData();

                if (DataList != null)
                {
                    // 紀錄資料長度
                    int DataLength = DataList.Count;

                    if (DataLength != 0)
                    {
                        // 初始化一個Grid
                        Grid grid_content = InitGridSet();

                        // 將抓到的資料放入Grid中
                        for (int i = 0; i < DataLength; i++)
                        {
                            ModifyGridToAddData(grid_content, i, DataList[i]);
                        }

                        AddGridInToStackLayout(grid_content);
                    }
                }
            }
            Appearing_RefreshView.IsRefreshing = false;
            myRefreshView.IsRefreshing = false;
            Appearing_RefreshView.IsEnabled = false;
        }

        /// <summary>
        /// 清空當前頁面當中所有的Entry
        /// </summary>
        public void clearAllEntry()
        {
            Entry Entry_Email = (Entry)FindByName("Entry_Email");
            Entry_Email.Text = "";

            Entry Entry_Password = (Entry)FindByName("Entry_Password");
            Entry_Password.Text = "";

            Entry Entry_AquaruimNum = (Entry)FindByName("Entry_AquaruimNum");
            Entry_AquaruimNum.Text = "";

            //Entry Entry_AquariumTypeId = (Entry)FindByName("Entry_AquariumTypeId");
            //Entry_AquariumTypeId.Text = "";
        }

        /// <summary>
        /// 密碼顯示的按鈕Toggle
        /// </summary>
        private void PasswordShowButton_Clicked(object sender, EventArgs e)
        {
            Entry entry_Password = Entry_Password as Entry;
            Entry_Password.IsPassword = !Entry_Password.IsPassword;
        }

        /// <summary>
        /// 送出按鈕被點擊後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // TODO 將資料塞入魚缸編輯
            // await DisplayAlert("注意", "已發出綁定請求!", "確定");
            // 取得 XAML 中的 Entry 物件 ，並將當前內容取出
            Entry Entry_Email = (Entry)FindByName("Entry_Email");
            string string_Email = Entry_Email.Text;

            Entry Entry_Password = (Entry)FindByName("Entry_Password");
            string string_Password = Entry_Password.Text;

            Entry Entry_AquaruimNum = (Entry)FindByName("Entry_AquaruimNum");
            string string_AquaruimNum = Entry_AquaruimNum.Text;

            //Entry Entry_AquariumTypeId = (Entry)FindByName("Entry_AquariumTypeId");
            string string_AquariumTypeId = WaterType_Select.ToString();

            if (string_Email == null ||
                string_Email == "" ||
                string_Password == null ||
                string_Password == "" ||
                string_AquaruimNum == null ||
                string_AquaruimNum == "" ||
                string_AquariumTypeId == null ||
                string_AquariumTypeId == "")
            {
                await DisplayAlert("注意", "請填入完整的資訊!", "確定");
            }
            else
            {
                Appearing_RefreshView.IsEnabled = true;
                Appearing_RefreshView.IsRefreshing = true;

                await Task.Delay(300);

                // 裝等待後的回傳結果
                ReturnMsg returnMsg = await SendBindRequest(string_Email,
                                                             string_Password,
                                                             string_AquaruimNum,
                                                             string_AquariumTypeId);
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
            Appearing_RefreshView.IsEnabled = false;
            Appearing_RefreshView.IsRefreshing = false;
        }

        /// <summary>
        /// 刪除按鈕被點擊後，確認是否解綁
        /// </summary>
        private async void BtnDelete_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            // 取的按鈕的代表的魚缸編號，這裡因為找不到其他方法來賦予按鈕資料，所以暫且只有想到這方法
            var aquariumUnitNum = button.ClassId;

            // 與用戶確認是否要解綁
            bool answer = await DisplayAlert("確認",
                                            "您確定要解綁魚缸\n" + aquariumUnitNum + " ?",
                                            "是", "否");
            // 是
            if (answer)
            {
                // 向Server發送解綁要求
                ReturnMsg returnMsg = await SendUnBindRequest(aquariumUnitNum);

                // 若Server不等於null，那代表有回應
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
        }

        /// <summary>
        /// 命名按鈕被點擊後，確認是否解綁
        /// </summary>
        private async void BtnCustomName_Clicked(object sender, EventArgs e)
        {
            // 先檢查用戶使否有該起網路
            if (!IsNetworkAvailable())
            {
                await DisplayAlert("警告", "手機未連線至網路，請開啟網路連線後再試。", "確定");
            }
            else
            {
                var button = sender as Button;
                // 取的按鈕的代表的魚缸編號，這裡因為找不到其他方法來賦予按鈕資料，所以暫且只有想到這方法
                var aquariumUnitNum = button.ClassId;

                // 將魚缸編號存入Properties，方便跳轉業面後能調用
                app.Properties[AquariumUnitNum] = aquariumUnitNum;
                await app.SavePropertiesAsync();


                await ContentNavigationService.NavigateToAsync(ContentNavigationConstants.CustomAquariumName);
            }
        }

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

        // ========================================================================== 取得伺服器回傳的綁定結果

        /// <summary>
        /// 向伺服器發送  綁定  請求
        /// </summary>
        public async Task<ReturnMsg> SendBindRequest(string Email, string Password,
                                                string AquariumNum, string WaterType)
        {
            // 準備裝伺服器回應的資料
            ReturnMsg returnMsg = new ReturnMsg();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/BindAquariumService";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                // 準備魚缸編號，用於查詢
                dataSendUse["Email"] = Email;
                dataSendUse["Password"] = Password;
                dataSendUse["AquariumNum"] = AquariumNum;
                dataSendUse["WaterType"] = WaterType;

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
                returnMsg = JsonConvert.DeserializeObject<ReturnMsg>(str);

                return returnMsg;
            }

        }


        /// <summary>
        /// 向Server取得使用者魚缸資訊，主要是取得該用戶有那些魚缸
        /// </summary>
        public async Task<List<ViewAquaruimSituation_ForMobile>> GetMyAquaruimSituationsData()
        {
            // 獲得Properties當中目前的使用者Auth001Id
            string _Auth001Id = app.Properties[Auth001Id].ToString();

            List<ViewAquaruimSituation_ForMobile> DataList = new List<ViewAquaruimSituation_ForMobile>();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/GetAquariumDatas";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                dataSendUse["Auth001Id"] = _Auth001Id;

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
                DataList = JsonConvert.DeserializeObject<List<ViewAquaruimSituation_ForMobile>>(str);

                return DataList;
            }
        }



        /// <summary>
        /// 向伺服器發送  解除綁定  請求
        /// </summary>
        public async Task<ReturnMsg> SendUnBindRequest(string AquariumNum)
        {
            // 獲得Properties當中目前的使用者Auth001Id
            string _Auth001Id = app.Properties[Auth001Id].ToString();

            // 準備裝伺服器回應的資料
            ReturnMsg returnMsg = new ReturnMsg();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/unBindService";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                // 準備魚缸編號，用於查詢
                dataSendUse["Auth001Id"] = _Auth001Id;
                dataSendUse["AquariumNum"] = AquariumNum;

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
                returnMsg = JsonConvert.DeserializeObject<ReturnMsg>(str);

                return returnMsg;
            }
        }

        // ========================================================================== 設定模板的區塊

        /// <summary>
        /// 頁面下滑刷新
        /// </summary>
        public void RefreshView_Refreshing(object sender, EventArgs args)
        {
            OnAppearing();
        }


            /// <summary>
            /// 水質類型_被操作後後。
            /// </summary>
            private void Select_AquariumTypeId_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 取得選擇的資料筆數
            var selectedValue = Select_AquariumTypeId.SelectedItem as string;
            WaterType_Select = WaterTypeStringToInt[selectedValue];
        }


        /// <summary>
        /// 初始化要放資料的Grid
        /// </summary>
        /// <returns></returns>
        public Grid InitGridSet()
        {
            var grid_content_temp = new Grid
            {
                ColumnSpacing = 2,
                RowSpacing = 1,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                ColumnDefinitions =
                 {
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            return grid_content_temp;
        }


        /// <summary>
        /// 編輯單行資訊，塞入後再將Grid返回
        /// </summary>
        /// <param name="grid_content">編輯中的Grid</param>
        /// <param name="TargetRow">目前該資料要放在的Row</param>
        /// <param name="data">要塞入的該筆資料</param>
        /// <returns></returns>
        public Grid ModifyGridToAddData(Grid grid_content, int TargetRow, ViewAquaruimSituation_ForMobile data)
        {
            // 魚缸編號 (欄0)
            var stackLayout_AquaruimNum = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new StackLayout
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Children=
                        {
                            new Label
                            {
                                Text = data.AquariumUnitNum,
                                TextColor = Color.Black,
                                FontSize = 16,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                            }
                        }
                    }
                }
            };

            // 名稱按鈕 (欄1)
            Button button_customName = new Button
            {
                Text = data.customAquaruimName,
                ClassId = data.AquariumUnitNum,
                TextColor = Color.White,
                FontSize = 10,
                WidthRequest = 20,
                HeightRequest = 40,
                BackgroundColor = Color.FromHex("#31b0d5"),
                CornerRadius = 10
            };
            button_customName.Clicked += BtnCustomName_Clicked;
            var stackLayout_CustomName = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new StackLayout
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Children=
                        {
                            button_customName
                        }
                    }
                }
            };

            // 刪除按鈕 (欄2)
            Button button = new Button
            {
                Text = "解綁",
                ClassId = data.AquariumUnitNum,
                TextColor = Color.White,
                FontSize = 12,
                WidthRequest = 20,
                HeightRequest = 40,
                BackgroundColor = Color.FromHex("#d45e5e"),
                //BackgroundColor = Color.FromHex("#31b0d5"),
                CornerRadius = 10
            };

            button.Clicked += BtnDelete_Clicked;
            var stackLayout_Delete = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new StackLayout
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Children=
                        {
                            button
                        }
                    }
                }
            };


            grid_content.Children.Add(stackLayout_AquaruimNum);
            Xamarin.Forms.Grid.SetRow(stackLayout_AquaruimNum, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_AquaruimNum, 0);

            grid_content.Children.Add(stackLayout_CustomName);
            Xamarin.Forms.Grid.SetRow(stackLayout_CustomName, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_CustomName, 1);

            grid_content.Children.Add(stackLayout_Delete);
            Xamarin.Forms.Grid.SetRow(stackLayout_Delete, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_Delete, 2);

            return grid_content;
        }


        /// <summary>
        /// 將編輯完成的Grid放入 stackLayout_DataList
        /// </summary>
        /// <param name="grid_content"></param>
        public void AddGridInToStackLayout(Grid grid_content)
        {
            stackLayout_DataList.Children.Add(grid_content);
        }

        /// <summary>
        /// 清空塞資料的StackLayout
        /// </summary>
        public void ClearStackLayoutData()
        {
            stackLayout_DataList.Children.Clear();
        }
    }
}