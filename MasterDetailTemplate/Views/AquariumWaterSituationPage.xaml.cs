using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MasterDetailTemplate.Models.AquariumWaterSituationPage;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AquariumWaterSituationPage : ContentPage
    {
        public App app = Application.Current as App;
        public string Auth001Id = "Auth001Id";

        public AquariumWaterSituationPage()
        {
            InitializeComponent();

            AddAquariumTemplate();
        }

        async void RefreshView_Refreshing(object sender, EventArgs args) {
            await Task.Delay(3000);
            StackLayout_Miain.Children.Clear();
            AddAquariumTemplate();
            myRefreshView.IsRefreshing = false;
        }

        public void AddAquariumTemplate() {
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
                        Text = "2022/10/5 上午 10:12:00",
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
                        Text = "Low Level",
                        TextColor = Color.Black,
                        FontSize = 18
                    }
                }
            };
            grid_content.Children.Add(stackLayout_waterLevel_value);
            Xamarin.Forms.Grid.SetRow(stackLayout_waterLevel_value, 1);
            Xamarin.Forms.Grid.SetColumn(stackLayout_waterLevel_value, 1);




            var mainStackLayout = new StackLayout
            {
                Margin = new Thickness(20, 20, 20, 20),
                Children =
                {
                    new Label
                    {
                        Text = "編號 : AAAAAAAA11111111",
                        FontSize = 25,
                        TextColor = Color.Black
                    },
                    new Label
                    {
                        Text = "水質類型 : 中性",
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

        public async Task<AquaruimSituation> GetMyAquaruimSituationsData() 
        {
            // 獲得Properties當中目前的使用者Auth001Id
            string _Auth001Id = app.Properties[Auth001Id].ToString();


        }
    }
}