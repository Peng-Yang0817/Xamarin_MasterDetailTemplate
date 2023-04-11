using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using Microcharts;
using Microcharts.Forms;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Threading;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartViewPage : ContentPage
    {
        // 創建變量
        public App app = Application.Current as App;
        public string AquariumUnitNum = "AquariumUnitNum";
        public string Auth001Id = "Auth001Id";

        public Dictionary<string, string> keyValuePairs_WaterLevel = new Dictionary<string, string>
        {
            {"1", "Low Level"},
            {"2", "Middle Level"},
            {"3", "Heigh Level"}
        };

        //等待伺服器回應的時間設定
        private const int WaitTimeToServerResponese = 3500;

        public ChartViewPage()
        {
            InitializeComponent();
            SetAquaruimNum();

            NavigationPage.SetHasBackButton(this, true);
        }

        /// <summary>
        /// 覆寫重新進到該頁面後的刷新。
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Appearing_RefreshView.IsEnabled = true;
            Appearing_RefreshView.IsRefreshing = true;

            // 設定魚缸標題
            SetAquaruimNum();

            // 取得圖表所需資料
            List<AquariumSituationMotify> DataList = await GetMyChartNeedData();

            // 定義時間與各基準的集合
            List<string> DateTiemString = new List<string>();
            List<string> DataString_temperature = new List<string>();
            List<string> DataString_Turbidity = new List<string>();
            List<string> DataString_PH = new List<string>();
            List<string> DataString_TDS = new List<string>();
            List<string> DataString_WaterLevel = new List<string>();

            // 放資料到對應集合
            foreach (var item in DataList)
            {
                DateTiemString.Add(item.createTime);

                DataString_temperature.Add(item.temperature);
                DataString_Turbidity.Add(item.Turbidity);
                DataString_PH.Add(item.PH);
                DataString_TDS.Add(item.TDS);
                DataString_WaterLevel.Add(item.WaterLevelNum);
            }
            // ============================================== 溫度
            // 設定溫度圖表
            SetTemperatureChart(DateTiemString, DataString_temperature);
            // ============================================== 濁度
            // 設定濁度圖表
            SetTurbidityChart(DateTiemString, DataString_Turbidity);
            // ============================================== PH
            // 設定PH圖表
            SetPHChart(DateTiemString, DataString_PH);
            // ============================================== TDS
            // 設定TDS圖表
            SetTDSChart(DateTiemString, DataString_TDS);
            // ============================================== 水位高度
            // 設定水位高度圖表
            SetWaterLevelChart(DateTiemString, DataString_WaterLevel);

            // 把所有圖表拉到最右邊
            // await ChartScollToRight();

            Appearing_RefreshView.IsRefreshing = false;
            Appearing_RefreshView.IsEnabled = false;
        }

        /// <summary>
        /// 覆寫離開頁面後把該頁面清空的動作
        /// </summary>
        protected override async void OnDisappearing()
        {
            // 清空魚缸標題
            ClearAquaruimNumLabel();

            // 清空溫度的曲線圖的StackLayout
            StackLatout_Temperature.Children.Clear();

            // 清空濁度的曲線圖的StackLayout
            StackLatout_Turbidity.Children.Clear();

            // 清空PH的曲線圖的StackLayout
            StackLatout_PH.Children.Clear();

            // 清空硬度的曲線圖的StackLayout
            StackLatout_TDS.Children.Clear();

            // 清空水位高度的曲線圖的StackLayout
            StackLatout_WaterLevel.Children.Clear();

            await Task.Delay(100);
        }
        // ========================================================================== 塞資料區塊

        /// <summary>
        /// 設定溫度圖表
        /// </summary>
        /// <param name="DateTimeString">時間label(6個)</param>
        /// <param name="DataString">溫度集合(6個)</param>
        public void SetTemperatureChart(List<string> DateTimeString, List<string> DataString)
        {
            // 清空溫度的曲線圖的StackLayout
            StackLatout_Temperature.Children.Clear();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = " °C";

            // 測試用的溫度狀態List<ChartEntry>
            List<ChartEntry> entries_Temperature = new List<ChartEntry>();

            for (int i = 0; i < 6; i++)
            {
                entries_Temperature.Add(
                    new ChartEntry(float.Parse(DataString[i]))
                    {
                        Color = SKColor.Parse("#ffffff"),
                        ValueLabelColor = SKColor.Parse("#ffffff"),
                        Label = DateTimeString[i],
                        ValueLabel = DataString[i] + unit
                    }
                );
            }

            // 建立 LineChart
            var chart = GetLineChartTemp();
            // LineChart 放入資料
            chart.Entries = entries_Temperature;

            // 在 ChartView 當中放入 LineChart
            chartView.Chart = chart;

            // 將 ChartView 放入 StackLatout_Temperature
            StackLatout_Temperature.Children.Add(chartView);
        }

        /// <summary>
        /// 設定濁度圖表
        /// </summary>
        /// <param name="DateTimeString">時間label(6個)</param>
        /// <param name="DataString">濁度集合(6個)</param>
        public void SetTurbidityChart(List<string> DateTimeString, List<string> DataString)
        {
            // 清空濁度的曲線圖的StackLayout
            StackLatout_Turbidity.Children.Clear();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = " NTU";

            // 測試用的濁度狀態List<ChartEntry>
            List<ChartEntry> entries_Turbidity = new List<ChartEntry>();
            for (int i = 0; i < 6; i++)
            {
                entries_Turbidity.Add(
                    new ChartEntry(float.Parse(DataString[i]))
                    {
                        Color = SKColor.Parse("#ffffff"),
                        ValueLabelColor = SKColor.Parse("#ffffff"),
                        Label = DateTimeString[i],
                        ValueLabel = DataString[i] + unit
                    }
                );
            }


            // 建立 LineChart
            var chart = GetLineChartTemp();
            // LineChart 放入資料
            chart.Entries = entries_Turbidity;

            // 在 ChartView 當中放入 LineChart
            chartView.Chart = chart;

            // 將 ChartView 放入 StackLatout_Temperature
            StackLatout_Turbidity.Children.Add(chartView);
        }

        /// <summary>
        /// 設定PH圖表
        /// </summary>
        /// <param name="DateTimeString">時間label(6個)</param>
        /// <param name="DataString">PH集合(6個)</param>
        public void SetPHChart(List<string> DateTimeString, List<string> DataString)
        {
            // 清空濁度的曲線圖的StackLayout
            StackLatout_PH.Children.Clear();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = " mol/L";

            // 測試用的PH狀態List<ChartEntry>
            List<ChartEntry> entries_PH = new List<ChartEntry>();

            for (int i = 0; i < 6; i++)
            {
                entries_PH.Add(
                    new ChartEntry(float.Parse(DataString[i]))
                    {
                        Color = SKColor.Parse("#ffffff"),
                        ValueLabelColor = SKColor.Parse("#ffffff"),
                        Label = DateTimeString[i],
                        ValueLabel = DataString[i] + unit
                    }
                );
            }

            // 建立 LineChart
            var chart = GetLineChartTemp();
            // LineChart 放入資料
            chart.Entries = entries_PH;

            // 在 ChartView 當中放入 LineChart
            chartView.Chart = chart;
            chart.MaxValue = 14;
            chart.MinValue = 0;

            // 將 ChartView 放入 StackLatout_Temperature
            StackLatout_PH.Children.Add(chartView);
        }

        /// <summary>
        /// 設定TDS圖表
        /// </summary>
        /// <param name="DateTimeString">時間label(6個)</param>
        /// <param name="DataString">TDS集合(6個)</param>
        public void SetTDSChart(List<string> DateTimeString, List<string> DataString)
        {
            // 清空濁度的曲線圖的StackLayout
            StackLatout_TDS.Children.Clear();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = " ppm";

            // 測試用的PH狀態List<ChartEntry>
            List<ChartEntry> entries_TDS = new List<ChartEntry>();
            for (int i = 0; i < 6; i++)
            {
                entries_TDS.Add(
                    new ChartEntry(float.Parse(DataString[i]))
                    {
                        Color = SKColor.Parse("#ffffff"),
                        ValueLabelColor = SKColor.Parse("#ffffff"),
                        Label = DateTimeString[i],
                        ValueLabel = DataString[i] + unit
                    }
                );
            }


            // 建立 LineChart
            var chart = GetLineChartTemp();
            // LineChart 放入資料
            chart.Entries = entries_TDS;

            // 在 ChartView 當中放入 LineChart
            chartView.Chart = chart;

            // 將 ChartView 放入 StackLatout_Temperature
            StackLatout_TDS.Children.Add(chartView);
        }

        /// <summary>
        /// 設定水位高度圖表
        /// </summary>
        /// <param name="DateTimeString">時間label(6個)</param>
        /// <param name="DataString">水位高度集合(6個)</param>
        public void SetWaterLevelChart(List<string> DateTimeString, List<string> DataString)
        {
            // 清空濁度的曲線圖的StackLayout
            StackLatout_WaterLevel.Children.Clear();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = "";

            // 測試用的PH狀態List<ChartEntry>
            List<ChartEntry> entries_WaterLevel = new List<ChartEntry>();
            for (int i = 0; i < 6; i++)
            {
                entries_WaterLevel.Add(
                    new ChartEntry(float.Parse(DataString[i]))
                    {
                        Color = SKColor.Parse("#ffffff"),
                        ValueLabelColor = SKColor.Parse("#ffffff"),
                        Label = DateTimeString[i],
                        ValueLabel = keyValuePairs_WaterLevel[DataString[i]] + unit
                    }
                );
            }


            // 建立 LineChart
            var chart = GetLineChartTemp();
            // LineChart 放入資料
            chart.Entries = entries_WaterLevel;
            chart.MaxValue = 3;
            chart.MinValue = 1;

            // 在 ChartView 當中放入 LineChart
            chartView.Chart = chart;

            // 將 ChartView 放入 StackLatout_Temperature
            StackLatout_WaterLevel.Children.Add(chartView);
        }

        // ========================================================================== 取得伺服器回傳水質曲線圖資料集合

        /// <summary>
        /// 取得伺服器回傳水質曲線圖資料集合
        /// </summary>
        public async Task<List<AquariumSituationMotify>> GetMyChartNeedData()
        {
            // 準備裝曲線圖的資料集合
            List<AquariumSituationMotify> DataList = new List<AquariumSituationMotify>();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = "http://192.168.0.80:52809/MobileService/GetAquariumDatasForAquaruimId";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                // 準備魚缸編號，用於查詢
                dataSendUse["AquariumNum"] = getAquariumNum;

                // 使用 CancellationTokenSource 取消等待
                var cts = new CancellationTokenSource();

                // 建立 timeoutTask 來等待 WaitTimeToServerResponese 的時間
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10), cts.Token);

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
                DataList = JsonConvert.DeserializeObject<List<AquariumSituationMotify>>(str);

                return DataList;

            }
        }


        // ========================================================================== 定義模板區塊

        /// <summary>
        /// 全部把圖表的Scroll拉到最右邊
        /// </summary>
        public async Task ChartScollToRight() {

            var scrollView = FindByName("ScrollView_Temperature") as ScrollView;

            await ScrollView_Temperature.ScrollToAsync(800, 0, true);
            await ScrollView_Turbidity.ScrollToAsync(800, 0, true);
            await ScrollView_PH.ScrollToAsync(800, 0, true);
            await ScrollView_TDS.ScrollToAsync(800, 0, true);
            await ScrollView_WaterLevel.ScrollToAsync(800, 0, true);
        }

        /// <summary>
        /// 定義 ChartView 模板
        /// </summary>
        public ChartView GetChartViewTemp()
        {
            return new ChartView
            {
                HeightRequest = 250,
                WidthRequest = 800,
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
        }

        /// <summary>
        /// 定義 LineChart 模板
        /// </summary>
        public LineChart GetLineChartTemp()
        {
            return new LineChart()
            {
                PointSize = 30,
                LineSize = 12,
                LabelTextSize = 40,
                // 字體顏色
                LabelColor = SKColor.Parse("#ffffff"),
                // 背景顏色
                BackgroundColor = SKColor.Parse("#4cadec"),
                // 設定LineChart 在ChartView 當中的Margin
                Margin = 35,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                ValueLabelOrientation = Microcharts.Orientation.Horizontal,
            };
        }

        // 預設的圖表集合格式
        //[
        //    {
        //        "Id": 15,
        //        "AquariumId": 1008,
        //        "temperature": "23.1",
        //        "Turbidity": "985",
        //        "PH": "6.60",
        //        "TDS": "1007",
        //        "WaterLevel": "Middle Level",
        //        "createTime": "22/10/24-19:00",
        //        "WaterLevelNum": "2"
        //    },
        //    {
        //        "Id": 16,
        //        "AquariumId": 1008,
        //        "temperature": "20.2",
        //        "Turbidity": "1100",
        //        "PH": "6.65",
        //        "TDS": "1200",
        //        "WaterLevel": "Middle Level",
        //        "createTime": "22/10/24-20:00",
        //        "WaterLevelNum": "2"
        //    },
        //    {
        //    "Id": 17,
        //        "AquariumId": 1008,
        //        "temperature": "20.1",
        //        "Turbidity": "1112",
        //        "PH": "6.62",
        //        "TDS": "1220",
        //        "WaterLevel": "Middle Level",
        //        "createTime": "22/10/24-21:00",
        //        "WaterLevelNum": "2"
        //    },
        //    {
        //    "Id": 18,
        //        "AquariumId": 1008,
        //        "temperature": "20.2",
        //        "Turbidity": "1122",
        //        "PH": "6.62",
        //        "TDS": "1222",
        //        "WaterLevel": "Middle Level",
        //        "createTime": "22/10/24-22:00",
        //        "WaterLevelNum": "2"
        //    },
        //    {
        //    "Id": 19,
        //        "AquariumId": 1008,
        //        "temperature": "20.0",
        //        "Turbidity": "1135",
        //        "PH": "6.66",
        //        "TDS": "1222",
        //        "WaterLevel": "Middle Level",
        //        "createTime": "22/10/24-23:00",
        //        "WaterLevelNum": "2"
        //    },
        //    {
        //    "Id": 20,
        //        "AquariumId": 1008,
        //        "temperature": "20.2",
        //        "Turbidity": "1130",
        //        "PH": "6.67",
        //        "TDS": "1225",
        //        "WaterLevel": "Middle Level",
        //        "createTime": "22/10/25-01:00",
        //        "WaterLevelNum": "2"
        //    }
        //]

        /// <summary>
        /// 設定魚缸標題編號LABLE
        /// </summary>
        public void SetAquaruimNum()
        {
            AquaruimNum.Text = "魚缸 : " + getAquariumNum;
        }

        /// <summary>
        /// 離開頁面時，清空魚缸標題編號的LABLE
        /// </summary>
        public void ClearAquaruimNumLabel()
        {
            AquaruimNum.Text = "魚缸 : ";
        }

        /// <summary>
        /// 返回當前欲查詢的魚缸編號
        /// </summary>
        public string getAquariumNum =>
            // 獲得Properties當中目前欲查詢的AquariumUnitNum
            app.Properties[AquariumUnitNum].ToString();

        /// <summary>
        /// 返回當前的使用者的Auth001Id
        /// </summary>
        public string getAuth001Id =>
            // 獲得Properties當中目前的使用者Auth001Id
            app.Properties[Auth001Id].ToString();

    }
    // 自定義魚缸水質狀況類別
    public class AquariumSituationMotify
    {
        public int Id { get; set; }
        public int AquariumId { get; set; }
        public string temperature { get; set; }
        public string Turbidity { get; set; }
        public string PH { get; set; }
        public string TDS { get; set; }
        public string WaterLevel { get; set; }
        public string createTime { get; set; }
        public string WaterLevelNum { get; set; }
    }
}