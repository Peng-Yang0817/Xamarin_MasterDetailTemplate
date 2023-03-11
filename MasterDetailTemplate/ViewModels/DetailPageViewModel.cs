using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using MasterDetailTemplate.Models;

namespace MasterDetailTemplate.ViewModels
{
    /// <summary>
    /// 詩詞詳情頁
    /// </summary>
    public class DetailPageViewModel : ViewModelBase
    {
        /// <summary>
        ///  詩詞
        /// </summary>
        public Poetry Poetry {
            get => _poetry;
            set => Set(nameof(Poetry), ref _poetry, value);
        }
        /// <summary>
        ///  詩詞
        /// </summary>
        private Poetry _poetry;
    }
}
