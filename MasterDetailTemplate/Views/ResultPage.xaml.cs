using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Behaviors;
using MasterDetailTemplate.Services;
using MasterDetailTemplate.Models;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultPage : ContentPage
    {
        public ResultPage()
        {
            InitializeComponent();
        }

        private static ContentPageActivationService ContentPageActivationService =
            new ContentPageActivationService();

        private static ContentNavigationService ContentNavigationService =
            new ContentNavigationService(ContentPageActivationService);

        public async void ListView_item_onTapped(object sender, ItemTappedEventArgs args)
        {
            var poetryitem = (Poetry)args.Item;
            await ContentNavigationService.NavigateToAsync(ContentNavigationConstants.DetailPage,
                                        poetryitem);
        }
    }
}