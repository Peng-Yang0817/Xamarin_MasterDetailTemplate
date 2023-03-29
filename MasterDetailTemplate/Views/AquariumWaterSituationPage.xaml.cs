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

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AquariumWaterSituationPage : ContentPage
    {
        public App app = Application.Current as App;
        public string Auth001Id = "Auth001Id";
        public Dictionary<string, string> keyValuePairs_WaterLevel = new Dictionary<string, string>();
        public Dictionary<string, string> keyValuePairs_WaterType = new Dictionary<string, string>();


        public AquariumWaterSituationPage()
        {
            InitializeComponent();
            WaterLevelDefinit();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            StackLayout_Miain.Children.Clear();

            List<AquaruimSituation> DataList = await GetMyAquaruimSituationsData();

            foreach (var item in DataList)
            {
                AddAquariumTemplate(item);
            }
        }

        /// <summary>
        /// 定義水為高度對應中文
        /// </summary>
        public void WaterLevelDefinit() {
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
        async void RefreshView_Refreshing(object sender, EventArgs args) {

            StackLayout_Miain.Children.Clear();
            await Task.Delay(3000);

            // 取當前使用者資料
            List<AquaruimSituation> DataList = await GetMyAquaruimSituationsData();

            foreach (var item in DataList) {
                AddAquariumTemplate(item);
            }

            myRefreshView.IsRefreshing = false;
        }

        /// <summary>
        /// 接收資料塞值到 StackLayout_Miain
        /// </summary>
        public void AddAquariumTemplate(AquaruimSituation item) {
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

            var stackLayout_dataTime_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = item.createTime.ToString(),
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

            var stackLayout_waterTemperature_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = item.temperature +" °C",
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

            var stackLayout_waterTurbidity_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = item.Turbidity + " NTU",
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

            var stackLayout_waterTDS_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new Label
                    {
                        Text = item.TDS,
                        TextColor = Color.Black,
                        FontSize = 18,
                        HorizontalOptions = LayoutOptions.StartAndExpand
                    },
                    new Label{
                        Text ="ppm",
                        TextColor = Color.Black,
                        FontSize = 18,
                        BackgroundColor = Color.White,
                        HorizontalOptions = LayoutOptions.End
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

            var stackLayout_waterPH_value = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = item.PH+" mol/L",
                        TextColor = Color.Black,
                        FontSize = 18
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterPH_value);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterPH_value, 4);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterPH_value, 1);

            

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
                    new Label
                    {
                        Text = keyValuePairs_WaterType[item.WaterType],
                        FontSize = 20,
                        TextColor = Color.Black
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

            var wb = new WebClient();
            var dataSendUse = new NameValueCollection();

            string urlSendUse = "http://192.168.0.80:52809/MobileService/GetAquariumDatas";

            string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
            wb.Headers.Add("Authorization", Bearer);

            dataSendUse["Auth001Id"] = _Auth001Id;

            var responseSendUse = await wb.UploadValuesTaskAsync(urlSendUse, "POST", dataSendUse);

            string str = Encoding.UTF8.GetString(responseSendUse);

            // 解析JSON string
            DataList = JsonConvert.DeserializeObject<List<AquaruimSituation>>(str);

            return DataList;
        }
    
    }
}