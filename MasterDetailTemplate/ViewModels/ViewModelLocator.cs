using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MasterDetailTemplate.Services;
using MasterDetailTemplate.Models;

namespace MasterDetailTemplate.ViewModels
{
    public class ViewModelLocator
    {
        /// <summary>
        /// ViewModelLocator
        /// </summary>
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<IPoetryStorage, PoetryStorage>();
            SimpleIoc.Default.Register<IContentNavigationService, ContentNavigationService>();
            SimpleIoc.Default.Register<IContentPageActivationService, ContentPageActivationService>();
            SimpleIoc.Default.Register<IPreferenceStorage, PreferenceStorage>();
            SimpleIoc.Default.Register<ResultPageViewModel>();
            SimpleIoc.Default.Register<DetailPageViewModel>();
        }
        /// <summary>
        /// 搜索結果頁
        /// </summary>

        public ResultPageViewModel ResultPageViewModel =>
            SimpleIoc.Default.GetInstance<ResultPageViewModel>();

        /// <summary>
        /// 詩詞頁詳情
        /// </summary>
        public DetailPageViewModel DetailPageViewModel =>
            SimpleIoc.Default.GetInstance<DetailPageViewModel>();
    }
}
