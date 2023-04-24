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
    public interface IContentNavigationService
    {
        /// <summary>
        /// 導航到頁面
        /// </summary>
        /// <param name="pageKey">頁面鍵</param>
        Task NavigateToAsync(string pageKey);

        /// <summary>
        /// 導航到頁面
        /// </summary>
        /// <param name="pagekey">頁面鍵</param>
        /// <param name="parameter">參數</param>
        /// <returns></returns>
        Task NavigateToAsync(string pagekey, object parameter);
    }
    /// <summary>
    /// 內容導航常量
    /// </summary>
    public static class ContentNavigationConstants
    {
        /// <summary>
        /// 關於頁面
        /// </summary>
        public const string AboutPage = nameof(Views.AboutPage);
        public const string DetailPage = nameof(Views.DetailPage);
        public const string ChartViewPage = nameof(Views.ChartViewPage);
        public const string NotifyRangeSetDetailPage = nameof(Views.NotifyRangeSetDetailPage);

        /// <summary>
        /// 頁面鍵- 頁面類型字典
        /// </summary>
        public static readonly Dictionary<string, Type> PageKeyTypeDictionary =
            new Dictionary<string, Type> {
                { AboutPage,typeof(AboutPage) },
                { DetailPage,typeof(DetailPage) },
                { ChartViewPage,typeof(ChartViewPage) },
                { NotifyRangeSetDetailPage,typeof(NotifyRangeSetDetailPage) }
            };
    }
}
