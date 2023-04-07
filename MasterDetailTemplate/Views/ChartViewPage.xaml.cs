using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using Entry = Microcharts.Entry;
using Microcharts;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartViewPage : ContentPage
    {
        // 創建變量
        public App app = Application.Current as App;
        public string AquariumUnitNum = "AquariumUnitNum";
        public string Auth001Id = "Auth001Id";

        public List<Entry> entries = new List<Entry>
        {
            new Entry(23)
            {
                Color = SKColor.Parse("#1e1e1e"),
                Label = "2022/03/04",
                ValueLabel = "23"
            },
            new Entry(25)
            {
                Color = SKColor.Parse("#1e1e1e"),
                Label = "2022/03/05",
                ValueLabel = "25"
            },
            new Entry(18)
            {
                Color = SKColor.Parse("#1e1e1e"),
                Label = "2022/03/06",
                ValueLabel = "18"
            },
            new Entry(20)
            {
                Color = SKColor.Parse("#1e1e1e"),
                Label = "2022/03/07",
                ValueLabel = "20"
            },
            new Entry(22)
            {
                Color = SKColor.Parse("#1e1e1e"),
                Label = "2022/03/08",
                ValueLabel = "22"
            },
            new Entry(25)
            {
                Color = SKColor.Parse("#1e1e1e"),
                Label = "2022/03/09",
                ValueLabel = "25"
            }
        };

        public ChartViewPage()
        {
            InitializeComponent();
            SetAquaruimNum();

            Chart1.Chart = new LineChart { 
                Entries = entries,
                PointSize = 30,
                LineSize = 12,
                LabelTextSize = 35f,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                BackgroundColor = SKColors.Transparent,
                LabelColor = SKColor.Parse("#ffffff"),
            };
            Chart1.Chart.LabelTextSize = 25;

            NavigationPage.SetHasBackButton(this, true);
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