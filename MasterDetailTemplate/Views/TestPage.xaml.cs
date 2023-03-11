using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MasterDetailTemplate.Services;
using MasterDetailTemplate.Models;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private static ContentPageActivationService ContentPageActivationService =
            new ContentPageActivationService();

        private static ContentNavigationService ContentNavigationService =
            new ContentNavigationService(ContentPageActivationService);

        
        

        public async void TestPage_Button_Clicked(object sender, EventArgs args)
        {
            // var cns = new ContentNavigationService(new ContentPageActivationService());
            await ContentNavigationService.NavigateToAsync(ContentNavigationConstants.DetailPage,
                                        new Poetry { 
                                            Name="夕陽西下XD",
                                            Content= "柳暗花明春事深。\n小嫣紅芶藥,已抽斟。\n雨餘風軟碎名禽。\n遲遲日，猶帶一分陰。",
                                            Layout = Poetry.IndentLayout
                                        });
        }
    }
}