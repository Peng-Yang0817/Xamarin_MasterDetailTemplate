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
    /// 布局到文本對齊轉換器
    /// </summary>
    public class LayoutToTextAlignmentConverter : IValueConverter
    {
        /// <summary>
        /// 將值轉換成顯示
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //  string stringValue = null;
            //  try { 
            //      stringValue = (string)value; 
            //  } catch (Exception e) { }

            // 同等於這段話 : value as string

            // 判斷傳過來的值是否是字串，若是就坐下面判斷
            switch (value as string)
            {
                case Poetry.CenterLayout:
                    return TextAlignment.Center;

                case Poetry.IndentLayout:
                    return TextAlignment.Start;

                default:
                    return null;
            }

        }


        /// <summary>
        /// 將顯示轉換成值，要是有人調用他，鐵定就不對!!
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 強行報異常
            throw new DoNotCallMeException();
        }
    }
}
