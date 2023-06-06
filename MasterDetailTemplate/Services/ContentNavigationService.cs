using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MasterDetailTemplate.Views;
using Xamarin.Forms;

namespace MasterDetailTemplate.Services
{
    /// <summary>
    /// 內容導航服務街口
    /// </summary>
    public class ContentNavigationService : IContentNavigationService
    {
        public ContentNavigationService(IContentPageActivationService contentPageActivationService)
        {
            _contentPageActivationService = contentPageActivationService;
        }


        // 私有變量
        /// <summary>
        /// MainPage。
        /// </summary>
        private MainPage _mainPage;

        /// <summary>
        /// 內容頁激活服務
        /// </summary>
        private IContentPageActivationService _contentPageActivationService;

        /// <summary>
        /// 取得當前的MainPage
        /// 需要透過App.xaml.cs 來找到MainPage
        /// </summary>
        public MainPage MainPage =>
            _mainPage ?? (_mainPage = Application.Current.MainPage as MainPage);


        /// <summary>
        /// 導航到頁面
        /// </summary>
        /// <param name="pageKey">頁面鍵</param>
        public async Task NavigateToAsync(string pageKey)
        {
            // 透過MainPage的Detail進行導航! 
            await MainPage.Detail.Navigation.PushAsync(
                _contentPageActivationService.Activation(pageKey));
        }

        /// <summary>
        /// 移除最上層 Navigate
        /// </summary>
        /// <param name="pageKey">頁面鍵</param>
        public async Task PopNavigateAsync()
        {
            // 透過MainPage的Detail進行 Navigate 頁面移除! 
            await MainPage.Detail.Navigation.PopAsync();
        }

        /// <summary>
        /// 導航到頁面
        /// </summary>
        /// <param name="pagekey">頁面鍵</param>
        /// <param name="parameter">參數</param>
        /// <returns></returns>
        public async Task NavigateToAsync(string pagekey, object parameter)
        {
            // 取得目標頁面，設置，導航

            // 獲得目標頁面
            var page = _contentPageActivationService.Activation(pagekey);

            // 設定參數
            NavigationContext.SetParameter(page, parameter);

            // 導航
            await MainPage.Detail.Navigation.PushAsync(page);
        }
    }
}
