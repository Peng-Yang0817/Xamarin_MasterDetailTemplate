using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using MasterDetailTemplate.Utils;

namespace MasterDetailTemplate.Converters
{
    public class TextIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 
                (value as string)?.Insert(0, "\u3000\u3000")
                                  .Replace("\n", "\n\u3000\u3000");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 強行報異常
            throw new DoNotCallMeException();
        }
    }
}
