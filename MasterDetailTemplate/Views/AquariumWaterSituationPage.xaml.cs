using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MasterDetailTemplate.Models.AquariumWaterSituationPage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections.Specialized;
using System.Net;
using MasterDetailTemplate.Views;

using MasterDetailTemplate.Services;
using MasterDetailTemplate.Models;
using Xamarin.Essentials;
using System.Net.Http.Headers;
using System.Threading;
using MasterDetailTemplate.Models.ServerAccessSet;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AquariumWaterSituationPage : ContentPage
    {
        public App app = Application.Current as App;
        public string Auth001Id = "Auth001Id";
        public string AquariumUnitNum = "AquariumUnitNum";
        public Dictionary<string, string> keyValuePairs_WaterLevel = new Dictionary<string, string>();
        public Dictionary<string, string> keyValuePairs_WaterType = new Dictionary<string, string>();

        private serverAccessSet server = new serverAccessSet();

        // 準備轉跳頁面所需的物件
        private static ContentPageActivationService ContentPageActivationService =
            new ContentPageActivationService();
        private static ContentNavigationService ContentNavigationService =
            new ContentNavigationService(ContentPageActivationService);

        public AquariumWaterSituationPage()
        {
            InitializeComponent();
            WaterLevelDefinit();
        }

        /// <summary>
        /// 頁面每次訪問都會調用
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 先檢查用戶使否有該起網路
            if (!IsNetworkAvailable())
            {
                await DisplayAlert("警告", "手機未連線至網路，請開啟網路連線後再試。", "確定");
            }
            else
            {
                Appearing_RefreshView.IsRefreshing = true;

                StackLayout_Miain.Children.Clear();

                await Task.Delay(500);

                // 按鈕能否點擊資料容器
                Dictionary<string, bool> ButtonStaus = new Dictionary<string, bool>();

                // 取當前使用者資料
                List<AquaruimSituation> DataList = await GetMyAquaruimSituationsData();


                // 查看伺服器是否有正確回應，也就是看DataList是否為null，不為null才能進去做後續的事
                if (DataList != null)
                {
                    // 使用者沒有任何魚缸資料
                    if (DataList.Count <= 0)
                    {
                        await DisplayAlert("提醒",
                            "用戶尚未註冊任何魚缸",
                            "OK");
                    }
                    // 使用者有魚缸資料
                    else
                    {
                        // 取當前按鈕能否點擊資料
                        ButtonStaus = await GetAquariumDataStatus();

                        // 按鈕能不能點的資訊，比對長度是否相同
                        if (DataList.Count != ButtonStaus.Count)
                        {
                            // 若按鈕能不能點的資訊與資料長度不相符，代表渲染前端會失敗，必須做處理
                            await DisplayAlert("錯誤",
                            "資料數量比對異常! 請稍後再試!",
                            "OK");

                        }
                        // 若相同，則直接開始渲染前端畫面
                        else
                        {
                            foreach (var item in DataList)
                            {
                                AddAquariumTemplate(item, ButtonStaus);
                            }
                        }
                    }

                }

                Appearing_RefreshView.IsRefreshing = false;
                myRefreshView.IsRefreshing = false;
            }


        }

        /// <summary>
        /// 定義水為高度對應中文
        /// </summary>
        public void WaterLevelDefinit()
        {
            keyValuePairs_WaterLevel.Add("1", "Low Level");
            keyValuePairs_WaterLevel.Add("2", "Middle Level");
            keyValuePairs_WaterLevel.Add("3", "Heigh Level");
            keyValuePairs_WaterLevel.Add("", "尚未擁有資料");

            keyValuePairs_WaterType.Add("1", "水質類型 : 弱酸");
            keyValuePairs_WaterType.Add("2", "水質類型 : 中性");
            keyValuePairs_WaterType.Add("3", "水質類型 : 弱鹼");
            keyValuePairs_WaterType.Add("", "水質類型 : 未定義");
        }

        /// <summary>
        /// 頁面下滑刷新
        /// </summary>
        async void RefreshView_Refreshing(object sender, EventArgs args)
        {
            // 先檢查用戶使否有該起網路
            if (!IsNetworkAvailable())
            {
                await DisplayAlert("警告", "手機未連線至網路，請開啟網路連線後再試。", "確定");
            }
            else
            {
                Appearing_RefreshView.IsRefreshing = true;
                StackLayout_Miain.Children.Clear();

                await Task.Delay(500);

                // 按鈕能否點擊資料容器
                Dictionary<string, bool> ButtonStaus = new Dictionary<string, bool>();

                // 取當前使用者資料
                List<AquaruimSituation> DataList = await GetMyAquaruimSituationsData();


                // 查看伺服器是否有正確回應，也就是看DataList是否為null，不為null才能進去做後續的事
                if (DataList != null)
                {
                    // 使用者沒有任何魚缸資料
                    if (DataList.Count <= 0)
                    {
                        await DisplayAlert("提醒",
                            "用戶尚未註冊任何魚缸",
                            "OK");
                    }
                    // 使用者有魚缸資料
                    else
                    {
                        // 取當前按鈕能否點擊資料
                        ButtonStaus = await GetAquariumDataStatus();

                        // 按鈕能不能點的資訊，比對長度是否相同
                        if (DataList.Count != ButtonStaus.Count)
                        {
                            // 若按鈕能不能點的資訊與資料長度不相符，代表渲染前端會失敗，必須做處理
                            await DisplayAlert("錯誤",
                            "資料數量比對異常! 請稍後再試!",
                            "OK");

                        }
                        // 若相同，則直接開始渲染前端畫面
                        else
                        {
                            foreach (var item in DataList)
                            {
                                AddAquariumTemplate(item, ButtonStaus);
                            }
                        }
                    }

                }


            }
            Appearing_RefreshView.IsRefreshing = false;
            myRefreshView.IsRefreshing = false;
        }

        /// <summary>
        /// 接收資料塞值到 StackLayout_Miain
        /// </summary>
        public void AddAquariumTemplate(AquaruimSituation item, Dictionary<string, bool> ButtonStaus)
        {
            var grid_content = new Grid
            {
                ColumnSpacing = 2,
                RowSpacing = 1,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            // ============================================================================= DataTime
            var stackLayout_dataTime_title = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Title_Style"],
                Children =
                {
                    new Label
                    {
                        Text = "紀錄時間",
                        TextColor = Color.Wheat,
                        FontSize = 20
                    }
                }
            };
            grid_content.Children.Add(stackLayout_dataTime_title);
            Xamarin.Forms.Grid.SetRow(stackLayout_dataTime_title, 0);
            Xamarin.Forms.Grid.SetColumn(stackLayout_dataTime_title, 0);

            DateTime DataDate = item.createTime;
            string DataDate_string = DataDate.ToString("yy/MM/dd-HH:mm");

            var stackLayout_dataTime_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = DataDate_string,
                        TextColor = Color.Black,
                        FontSize = 18
                    }
                }
            };
            grid_content.Children.Add(stackLayout_dataTime_value);
            Xamarin.Forms.Grid.SetRow(stackLayout_dataTime_value, 0);
            Xamarin.Forms.Grid.SetColumn(stackLayout_dataTime_value, 1);

            // ============================================================================= WaterLevel
            var stackLayout_waterLevel_title = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Title_Style"],
                Children =
                {
                    new Label
                    {
                        Text = "水位高度",
                        TextColor = Color.Wheat,
                        FontSize = 20
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterLevel_title);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterLevel_title, 1);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterLevel_title, 0);

            var stackLayout_waterLevel_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = item.WaterLevelNum == "" ? "尚未擁有資料": keyValuePairs_WaterLevel[item.WaterLevelNum],
                        TextColor = Color.Black,
                        FontSize = 18
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterLevel_value);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterLevel_value, 1);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterLevel_value, 1);

            // ============================================================================= WaterTemperature
            var stackLayout_waterTemperature_title = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Title_Style"],
                Children =
                {
                    new Label
                    {
                        Text = "水中溫度",
                        TextColor = Color.Wheat,
                        FontSize = 20
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterTemperature_title);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterTemperature_title, 2);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterTemperature_title, 0);

            var Temperature = "尚未擁有資料";
            if (item.temperature != "尚未擁有資料")
            {
                Temperature = item.temperature + " °C";
            }

            var stackLayout_waterTemperature_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = Temperature,
                        TextColor = Color.Black,
                        FontSize = 18
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterTemperature_value);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterTemperature_value, 2);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterTemperature_value, 1);

            // ============================================================================= WaterTurbidity
            var stackLayout_waterTurbidity_title = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Title_Style"],
                Children =
                {
                    new Label
                    {
                        Text = "水中濁度",
                        TextColor = Color.Wheat,
                        FontSize = 20
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterTurbidity_title);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterTurbidity_title, 3);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterTurbidity_title, 0);

            var Turbidity = "尚未擁有資料";
            if (item.Turbidity != "尚未擁有資料")
            {
                Turbidity = item.Turbidity + " NTU";
            }

            var stackLayout_waterTurbidity_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = Turbidity,
                        TextColor = Color.Black,
                        FontSize = 18
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterTurbidity_value);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterTurbidity_value, 3);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterTurbidity_value, 1);

            // ============================================================================= WaterTDS
            var stackLayout_waterTDS_title = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Title_Style"],
                Children =
                {
                    new Label
                    {
                        Text = "TDS 值",
                        TextColor = Color.Wheat,
                        FontSize = 20
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterTDS_title);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterTDS_title, 5);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterTDS_title, 0);

            var TDS = "尚未擁有資料";
            if (item.TDS != "尚未擁有資料")
            {
                TDS = item.TDS + " ppm";
            }

            var stackLayout_waterTDS_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new Label
                    {
                        Text = TDS,
                        TextColor = Color.Black,
                        FontSize = 18,
                        HorizontalOptions = LayoutOptions.StartAndExpand
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterTDS_value);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterTDS_value, 5);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterTDS_value, 1);

            // ============================================================================= WaterPH
            var stackLayout_waterPH_title = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Title_Style"],
                Children =
                {
                    new Label
                    {
                        Text = "PH 值",
                        TextColor = Color.Wheat,
                        FontSize = 20
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterPH_title);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterPH_title, 4);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterPH_title, 0);

            var PH = "尚未擁有資料";
            if (item.PH != "尚未擁有資料")
            {
                PH = item.PH + " mol/L";
            }

            var stackLayout_waterPH_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = PH,
                        TextColor = Color.Black,
                        FontSize = 18
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterPH_value);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterPH_value, 4);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterPH_value, 1);


            // ============================================================================= 插入的新StackLayout
            bool AQStatus = ButtonStaus[item.AquariumUnitNum];
            Button button = new Button();

            // 判斷該魚缸資料量足不足夠，足夠才給予圖表顯示按鈕
            if (AQStatus)
            {

                button = new Button
                {
                    Text = "查看圖表",
                    // 將魚缸編號存在按鈕的ClassId當中，以便觸發時可以讀取
                    ClassId = item.AquariumUnitNum,
                    IsEnabled = true,
                    BackgroundColor = Color.FromHex("#94bed6"),
                    TextColor = Color.White,
                    FontSize = 15,
                    Margin = new Thickness(10, 0, 10, 0),
                    WidthRequest = 120,
                    HeightRequest = 40,
                    CornerRadius = 8,
                    VerticalOptions = LayoutOptions.Center
                };

                button.Clicked += Button_ChartView_Clicked;
            }
            else
            {
                button = new Button
                {
                    Text = "資料量不足",
                    BackgroundColor = Color.Red,
                    TextColor = Color.White,
                    FontSize = 15,
                    Margin = new Thickness(10, 0, 10, 0),
                    WidthRequest = 120,
                    HeightRequest = 40,
                    CornerRadius = 8,
                    VerticalOptions = LayoutOptions.Center
                };
            }


            var mainStackLayout = new StackLayout
            {
                Margin = new Thickness(20, 20, 20, 20),
                Children =
                {
                    new Label
                    {
                        Text = "編號 : "+item.AquariumUnitNum,
                        FontSize = 25,
                        TextColor = Color.Black
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children=
                        {
                            new Label
                            {
                                Text = keyValuePairs_WaterType[item.WaterType],
                                FontSize = 20,
                                TextColor = Color.Black,
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                VerticalOptions = LayoutOptions.Center,
                            },
                            button
                        }
                    },

                    new StackLayout
                    {
                        Margin = new Thickness(10, 10, 10, 10),
                        Children =
                        {
                            grid_content
                        }
                    }
                }

            };

            StackLayout_Miain.Children.Add(mainStackLayout);
        }

        /// <summary>
        /// 向Server取得使用者魚缸資訊
        /// </summary>
        /// <returns></returns>
        public async Task<List<AquaruimSituation>> GetMyAquaruimSituationsData()
        {
            // 獲得Properties當中目前的使用者Auth001Id
            string _Auth001Id = app.Properties[Auth001Id].ToString();

            List<AquaruimSituation> DataList = new List<AquaruimSituation>();

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
                DataList = JsonConvert.DeserializeObject<List<AquaruimSituation>>(str);

                return DataList;
            }
        }


        /// <summary>
        /// 向Server取得使用者魚缸是否可以點 "查看圖表"
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, bool>> GetAquariumDataStatus()
        {
            // 獲得Properties當中目前的使用者Auth001Id
            string _Auth001Id = app.Properties[Auth001Id].ToString();
            Dictionary<string, bool> DataList = new Dictionary<string, bool>();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/GetAquariumDataStatus";

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
                DataList = JsonConvert.DeserializeObject<Dictionary<string, bool>>(str);

                return DataList;
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
        /// 轉跳至新頁面，顯示圖表，跳轉前必須先把當前的魚缸目標紀錄在 Properties 中。
        /// </summary>
        public async void Button_ChartView_Clicked(object sender, EventArgs e)
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


                await ContentNavigationService.NavigateToAsync(ContentNavigationConstants.ChartViewPage);
            }

        }


    }
}