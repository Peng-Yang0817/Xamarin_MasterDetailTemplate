using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MasterDetailTemplate.Models.ServerAccessSet;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoricalRecordSearchPage : ContentPage
    {
        public int EntryLength = 0;

        private serverAccessSet server = new serverAccessSet();
        public HistoricalRecordSearchPage()
        {
            InitializeComponent();
        }

        private void TxtFishTankNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            EntryLength = e.NewTextValue.Length;

            if (e.NewTextValue.Length != 16)
            {
                btnSearch.BackgroundColor = Color.FromHex("#74bbf3");
                btnSearch.TextColor = Color.FromHex("#ffffff");

            }
            else
            {
                btnSearch.BackgroundColor = Color.FromHex("#2196F3");
                btnSearch.TextColor = Color.FromHex("#ffffff");
            }
        }

        private async void BtnSearch_Clicked(object sender, EventArgs e)
        {
            if (!server.IsNetworkAvailable())
            {
                await DisplayAlert("警告", "請先連接至網際網路。", "確定");
            }
            else
            {
                if (EntryLength != 16)
                {
                    await DisplayAlert("警告", "請輸入完整的16位元組。", "確定");
                }
                else
                {
                    await DisplayAlert("成功", "開始查詢。", "確定");
                }
            }
        }
    }
}