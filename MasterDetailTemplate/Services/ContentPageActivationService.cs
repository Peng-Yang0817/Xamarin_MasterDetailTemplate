using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MasterDetailTemplate.Views;

namespace MasterDetailTemplate.Services
{
    /// <summary>
    /// 內容頁激活服務
    /// </summary>
    public class ContentPageActivationService : IContentPageActivationService
    {

        /// <summary>
        /// 頁面緩存，一個頁面只new一次就好
        /// 下次需要newPage之前，先到這裡看有沒有，
        /// 沒有的話再new，有的話拿出來用就好
        /// </summary>
        private Dictionary<string, ContentPage> cache =
            new Dictionary<string, ContentPage>();


        /// <summary>
        /// 激活內容頁
        /// </summary>
        /// <param name="pageKey">頁面鍵</param>
        /// <returns></returns>
        public ContentPage Activation(string pageKey)
        {
            if (cache.ContainsKey(pageKey))
            {
                return cache[pageKey];
            }

            //if (pageKey == ContentNavigationConstants.AboutPage)
            //{
            //    cache[pageKey] = new Views.AboutPage();
            //}

            cache[pageKey] = (ContentPage)Activator.CreateInstance(
                ContentNavigationConstants.PageKeyTypeDictionary[pageKey]
                );

            return cache[pageKey];
        }
    }
}
