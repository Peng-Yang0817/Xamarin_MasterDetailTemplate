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
        public string AquariumUnitNum = "AquariumUnitNum";
        public App()
        {
            InitializeComponent();

            PropertiesInit();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
            //MainPage = new NavigationPage(new ItemsPage());
        }

        /// <summary>
        ///初始化系統需要的Properties，將其存到app當中
        /// </summary>
        public void PropertiesInit() 
        {
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
            if (!Properties.ContainsKey(AquariumUnitNum))
            {
                Properties[AquariumUnitNum] = "NULL";
            }
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
