using MasterDetailTemplate.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MasterDetailTemplate.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public App app = Application.Current as App;
        public string ViewMode = "ViewMode";

        public AboutPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await Task.Delay(300);
            // 轉換狀態
            string status_viewModel = app.Properties[ViewMode].ToString();
            bool status_viewModel_bool = bool.Parse(status_viewModel);

            if (status_viewModel_bool)
            {
                switch_ViewMode.IsToggled = true;
            }
            else
            {
                switch_ViewMode.IsToggled = false;
            }
        }

        private async void switch_ViewMode_Tapped(object sender, EventArgs e)
        {
            if (switch_ViewMode.IsToggled)
            {
                // 使用者按下「是」，執行相關程式碼
                app.Properties[ViewMode] = switch_ViewMode.IsToggled;
            }
            else
            {
                // 使用者按下「否」，執行相關程式碼
                app.Properties[ViewMode] = switch_ViewMode.IsToggled;
            }

            await app.SavePropertiesAsync();
        }


        async void OnItemSelected(object sender, EventArgs args)
        {

            await Navigation.PushAsync(new ResultPage());

        }
    }
}