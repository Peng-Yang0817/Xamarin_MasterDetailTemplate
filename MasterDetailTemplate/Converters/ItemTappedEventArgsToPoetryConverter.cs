using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using MasterDetailTemplate.Utils;
using MasterDetailTemplate.Models;

namespace MasterDetailTemplate.Converters
{
    /// <summary>
    /// ItemTappedEventArgs到詩詞轉換器
    /// </summary>
    public class ItemTappedEventArgsToPoetryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果轉換成功就調用Item
            return (value as ItemTappedEventArgs)?.Item as Poetry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new DoNotCallMeException();
        }
    }
}
