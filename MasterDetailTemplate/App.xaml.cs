using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MasterDetailTemplate.Services;
using MasterDetailTemplate.Views;

namespace MasterDetailTemplate
{
    public partial class App : Application
    {
        public string LogStatus = "LogStatus";
        public string UserName = "UserName";
        public string Auth001Id = "Auth001Id";
        public App()
        {
            InitializeComponent();

            if (!Properties.ContainsKey(LogStatus))
            {
                Properties[LogStatus] = "false";
            }
            if (!Properties.ContainsKey(UserName))
            {
                Properties[UserName] = "NULL";
            }
            if (!Properties.ContainsKey(Auth001Id))
            {
                Properties[Auth001Id] = "NULL";
            }

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
            //MainPage = new NavigationPage(new ItemsPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
