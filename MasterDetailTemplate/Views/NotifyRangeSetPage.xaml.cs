using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using MasterDetailTemplate.Models.ServerAccessSet;
using Newtonsoft.Json;
using Xamarin.Essentials;

using MasterDetailTemplate.Services;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotifyRangeSetPage : ContentPage
    {
        public App app = Application.Current as App;
        public string Auth001Id = "Auth001Id";
        public string AquariumUnitNum = "AquariumUnitNum";

        private serverAccessSet server = new serverAccessSet();

        // 準備轉跳頁面所需的物件
        private static ContentPageActivationService ContentPageActivationService =
            new ContentPageActivationService();
        private static ContentNavigationService ContentNavigationService =
            new ContentNavigationService(ContentPageActivationService);

        public NotifyRangeSetPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 頁面重新載入時觸發，更新資料參數
        /// </summary>
        protected override async void OnAppearing()
        {
            if (!server.IsNetworkAvailable())
            {
                await DisplayAlert("警告", "請先連接至網際網路。", "確定");
            }
            else
            {
                ClearStackLayoutData();
                Appearing_RefreshView.IsEnabled = true;
                Appearing_RefreshView.IsRefreshing = true;
                await Task.Delay(300);

                // 取的該用戶所擁有的魚缸通知狀態資料
                List<AquaruimNotifyState> DataList = await SendAquaruimRangerSetNotifyStateRequest();

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
                    // 若資料長度為0，則代表用戶並沒有綁定中的魚缸可以設置通知。
                    else
                    {
                        await DisplayAlert("提醒", "目前該用戶並無綁定中的魚缸，請先綁定後再使用此功能", "確定");
                    }
                }

            }

            Appearing_RefreshView.IsRefreshing = false;
            Appearing_RefreshView.IsEnabled = false;
        }

        /// <summary>
        /// 設置範圍被點選後要執行轉跳頁面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void SetRangeButton_Clicked(object sender, EventArgs args)
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

                await ContentNavigationService.NavigateToAsync(ContentNavigationConstants.NotifyRangeSetDetailPage);
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

        /// <summary>
        /// 測試連結瀏覽器
        /// </summary>
        public async void Button_OpenBrowser_Clicked(object sender, EventArgs e)
        {
            string uri = server.ServerIP;
            uri += "/MobileService/BindLineNotifyNeedLogInFirst";

            await Browser.OpenAsync(uri, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredToolbarColor = Color.AliceBlue,
                PreferredControlColor = Color.Violet
            });
        }

        // ========================================================================== 自定義模板
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
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                 {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            return grid_content_temp;
        }

        /// <summary>
        /// 清空塞資料的StackLayout
        /// </summary>
        public void ClearStackLayoutData()
        {
            stackLayout_DataList.Children.Clear();
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
        /// 編輯單行資訊，塞入後再將Grid返回
        /// </summary>
        /// <param name="grid_content">編輯中的Grid</param>
        /// <param name="TargetRow">目前該資料要放在的Row</param>
        /// <param name="data">要塞入的該筆資料</param>
        /// <returns></returns>
        public Grid ModifyGridToAddData(Grid grid_content, int TargetRow, AquaruimNotifyState data)
        {
            // 序列 (欄0)
            var stackLayout_RowNum = new StackLayout
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
                                Text = (TargetRow+1).ToString(),
                                TextColor = Color.Black,
                                FontSize = 16,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                            }
                        }
                    }
                }
            };

            // 魚缸編號 (欄1)
            var stackLayout_AquariumNum = new StackLayout
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
                                Text = data.aquariumNum,
                                TextColor = Color.Black,
                                FontSize = 16,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                            }
                        }
                    }
                }
            };

            // 魚缸通知狀態 (欄2)
            var stackLayout_NotifyState = new StackLayout
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
                                Text = data.notifyTage,
                                TextColor = Color.Black,
                                FontSize = 16,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                            }
                        }
                    }
                }
            };


            // 魚缸通知狀態 (欄3)
            Button button = new Button
            {
                Text = "設置",
                ClassId = data.aquariumNum,
                TextColor = Color.FromHex("#ffffff"),
                FontSize = 10,
                WidthRequest = 20,
                HeightRequest = 40,
                BackgroundColor = Color.FromHex("#48830e"),
                CornerRadius = 10
            };
            button.Clicked += SetRangeButton_Clicked;
            var stackLayout_RangeSetButton = new StackLayout
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



            grid_content.Children.Add(stackLayout_RowNum);
            Xamarin.Forms.Grid.SetRow(stackLayout_RowNum, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_RowNum, 0);

            grid_content.Children.Add(stackLayout_AquariumNum);
            Xamarin.Forms.Grid.SetRow(stackLayout_AquariumNum, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_AquariumNum, 1);

            grid_content.Children.Add(stackLayout_NotifyState);
            Xamarin.Forms.Grid.SetRow(stackLayout_NotifyState, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_NotifyState, 2);

            grid_content.Children.Add(stackLayout_RangeSetButton);
            Xamarin.Forms.Grid.SetRow(stackLayout_RangeSetButton, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_RangeSetButton, 3);

            return grid_content;
        }


        // ========================================================================== 取得伺服器回傳的魚缸通知狀態與魚缸編號資訊
        /// <summary>
        /// 向伺服器發送 魚缸通知狀態設置 資訊
        /// </summary>
        public async Task<List<AquaruimNotifyState>> SendAquaruimRangerSetNotifyStateRequest()
        {
            // 獲得Properties當中目前的使用者Auth001Id
            string _Auth001Id = app.Properties[Auth001Id].ToString();

            // 準備裝伺服器回應的資料
            List<AquaruimNotifyState> DataList = new List<AquaruimNotifyState>();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/AquariumRangeSetNotifyState";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                // 準備魚缸編號，用於查詢
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
                DataList = JsonConvert.DeserializeObject<List<AquaruimNotifyState>>(str);

                return DataList;
            }
        }


    }

    // 自定義用戶魚缸  通知範圍設置狀態  類別
    public class AquaruimNotifyState
    {
        public string aquariumNum { get; set; }
        public string notifyTage { get; set; }
    }
}