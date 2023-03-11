using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MasterDetailTemplate.Services
{
    /// <summary>
    /// 內容頁激活服務
    /// </summary>
    public interface IContentPageActivationService
    {
        /// <summary>
        /// 激活內容頁
        /// </summary>
        /// <param name="pageKey">頁面鍵</param>
        /// <returns></returns>
        ContentPage Activation(string pageKey);
    }
}
