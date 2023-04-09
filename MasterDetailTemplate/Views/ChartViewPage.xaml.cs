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

        public ChartViewPage()
        {
            InitializeComponent();
            SetAquaruimNum();

            NavigationPage.SetHasBackButton(this, true);
        }

        /// <summary>
        /// 覆寫重新進到該頁面後的刷新。
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // 設定溫度圖表
            SetTemperatureChart();

            // 設定濁度圖表
            SetTurbidityChart();

            // 設定PH圖表
            SetPHChart();

            // 設定TDS圖表
            SetTDSChart();

            // 設定水位高度圖表
            SetWaterLevelChart();
        }

        // ========================================================================== 塞資料區塊


        /// <summary>
        /// 設定溫度圖表
        /// </summary>
        public void SetTemperatureChart()
        {
            // 清空溫度的曲線圖的StackLayout
            StackLatout_Temperature.Children.Clear();

            SetAquaruimNum();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = " °C";

            // 測試用的溫度狀態List<ChartEntry>
            List<ChartEntry> entries_Temperature = new List<ChartEntry>
            {
                new ChartEntry(23)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/04",
                    ValueLabel = "23"+ unit
                },
                new ChartEntry(25)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/05",
                    ValueLabel = "25"+ unit
                },
                new ChartEntry(18)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/06",
                    ValueLabel = "18"+ unit
                },
                new ChartEntry(20)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/07",
                    ValueLabel = "20"+ unit
                },
                new ChartEntry(22)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/08",
                    ValueLabel = "22"+ unit
                },
                new ChartEntry(18)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/09",
                    ValueLabel = "18"+ unit
                }
            };

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
        public void SetTurbidityChart()
        {
            // 清空濁度的曲線圖的StackLayout
            StackLatout_Turbidity.Children.Clear();

            SetAquaruimNum();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = " NTU";

            // 測試用的濁度狀態List<ChartEntry>
            List<ChartEntry> entries_Turbidity = new List<ChartEntry>
            {
                new ChartEntry(1700)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/04",
                    ValueLabel = "1700"+ unit
                },
                new ChartEntry(1749)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/05",
                    ValueLabel = "1749"+ unit
                },
                new ChartEntry(1800)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/06",
                    ValueLabel = "1800"+ unit
                },
                new ChartEntry(1760)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/07",
                    ValueLabel = "1760"+ unit
                },
                new ChartEntry(1922)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/08",
                    ValueLabel = "1922"+ unit
                },
                new ChartEntry(1987)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/09",
                    ValueLabel = "1987"+ unit
                }
            };

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
        public void SetPHChart()
        {
            // 清空濁度的曲線圖的StackLayout
            StackLatout_PH.Children.Clear();

            SetAquaruimNum();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = " mol/L";

            // 測試用的PH狀態List<ChartEntry>
            List<ChartEntry> entries_PH = new List<ChartEntry>
            {
                new ChartEntry(6.3f)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/04",
                    ValueLabel = "6.3"+ unit
                },
                new ChartEntry(7.2f)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/05",
                    ValueLabel = "7.2"+ unit
                },
                new ChartEntry(7.2f)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/06",
                    ValueLabel = "7.2"+ unit
                },
                new ChartEntry(2f)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/07",
                    ValueLabel = "2.0"+ unit
                },
                new ChartEntry(6.3f)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/08",
                    ValueLabel = "6.3"+ unit
                },
                new ChartEntry(9.5f)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/09",
                    ValueLabel = "9.5"+ unit
                },
            };

            // 建立 LineChart
            var chart = GetLineChartTemp();
            // LineChart 放入資料
            chart.Entries = entries_PH;

            // 在 ChartView 當中放入 LineChart
            chartView.Chart = chart;

            // 將 ChartView 放入 StackLatout_Temperature
            StackLatout_PH.Children.Add(chartView);
        }

        /// <summary>
        /// 設定TDS圖表
        /// </summary>
        public void SetTDSChart()
        {
            // 清空濁度的曲線圖的StackLayout
            StackLatout_TDS.Children.Clear();

            SetAquaruimNum();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = " ppm";

            // 測試用的PH狀態List<ChartEntry>
            List<ChartEntry> entries_TDS = new List<ChartEntry>
            {
                new ChartEntry(3350)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/04",
                    ValueLabel = "3350"+ unit
                },
                new ChartEntry(3050)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/05",
                    ValueLabel = "3050"+ unit
                },
                new ChartEntry(1350)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/06",
                    ValueLabel = "1350"+ unit
                },
                new ChartEntry(1350)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/07",
                    ValueLabel = "1350"+ unit
                },
                new ChartEntry(9000)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/08",
                    ValueLabel = "9000"+ unit
                },
                new ChartEntry(1350)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/09",
                    ValueLabel = "1350"+ unit
                },

            };

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
        /// 設定TDS圖表
        /// </summary>
        public void SetWaterLevelChart()
        {
            // 清空濁度的曲線圖的StackLayout
            StackLatout_WaterLevel.Children.Clear();

            SetAquaruimNum();

            // 建立 ChartView
            ChartView chartView = GetChartViewTemp();

            string unit = "";

            // 測試用的PH狀態List<ChartEntry>
            List<ChartEntry> entries_WaterLevel = new List<ChartEntry>
            {
                new ChartEntry(3)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/04",
                    ValueLabel = keyValuePairs_WaterLevel["3"]+ unit
                },
                new ChartEntry(3)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/05",
                    ValueLabel = keyValuePairs_WaterLevel["3"]+ unit
                },
                new ChartEntry(2)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/06",
                    ValueLabel = keyValuePairs_WaterLevel["2"]+ unit
                },
                new ChartEntry(2)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/07",
                    ValueLabel = keyValuePairs_WaterLevel["2"]+ unit
                },
                new ChartEntry(2)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/08",
                    ValueLabel = keyValuePairs_WaterLevel["2"]+ unit
                },
                new ChartEntry(1)
                {
                    Color = SKColor.Parse("#ffffff"),
                    ValueLabelColor =SKColor.Parse("#ffffff"),
                    Label = "2022/03/09",
                    ValueLabel = keyValuePairs_WaterLevel["1"]+ unit
                },

            };

            // 建立 LineChart
            var chart = GetLineChartTemp();
            // LineChart 放入資料
            chart.Entries = entries_WaterLevel;

            // 在 ChartView 當中放入 LineChart
            chartView.Chart = chart;

            // 將 ChartView 放入 StackLatout_Temperature
            StackLatout_WaterLevel.Children.Add(chartView);
        }


        // ========================================================================== 定義模板區塊

        /// <summary>
        /// 定義 ChartView 模板
        /// </summary>
        public ChartView GetChartViewTemp()
        {
            return new ChartView
            {
                HeightRequest = 250,
                WidthRequest = 600,
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

        /// <summary>
        /// 設定魚缸標題編號LABLE
        /// </summary>
        public void SetAquaruimNum()
        {
            AquaruimNum.Text = "魚缸 : " + getAquariumNum;
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
}