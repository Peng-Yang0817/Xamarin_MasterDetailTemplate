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
        public string UserName = "UserName";
        public string LogStatus = "LogStatus";
        public string ViewMode = "ViewMode";

        public AboutPage()
        {
            InitializeComponent();
        }


        // ========================================================================== 覆寫區
        /// <summary>
        /// TODO__
        /// 每次進到這頁面都會做的事
        /// </summary>
        protected override async void OnAppearing()
        {
            await Task.Delay(300);

            // 定義上方TopBar的User Login State Notify
            UserLoginStateShowOnTopBar();

            // TODO 預覽模式__針對沒有網路的DEMO做的!
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

        
        
        // ========================================================================== 自定義方法區
        /// <summary>
        /// TODO__
        /// 針對預覽模式開啟或關閉，所做的事件處理
        /// </summary>
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

        /// <summary>
        /// TopBar 提示用戶目前是否已經登入
        /// </summary>
        public void UserLoginStateShowOnTopBar() 
        {
            if (app.Properties[LogStatus].ToString() == "false")
            {
                UserName_LogInState.Text = "尚未登入";
            }
            else
            {
                UserName_LogInState.Text = "用戶 : "+ app.Properties[UserName].ToString().ToLower();
            }
        }

        /// <summary>
        /// TODO__
        /// 由下往上放入視窗，一開始的想法是把用戶登入介面隱藏
        /// </summary>
        async void AddItem_Clicked(object sender, EventArgs e)
        {
            // 模板視窗
            // await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }


        // ========================================================================== 一開始模板就有
        async void OnItemSelected(object sender, EventArgs args)
        {

            await Navigation.PushAsync(new ResultPage());

        }
    }
}