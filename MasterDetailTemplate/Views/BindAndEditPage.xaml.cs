using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BindAndEditPage : ContentPage
    {
        public BindAndEditPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 送出按鈕被點擊後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // TODO 將資料塞入魚缸編輯
            await DisplayAlert("注意", "已發出綁定請求!", "確定");

        }
    }
}