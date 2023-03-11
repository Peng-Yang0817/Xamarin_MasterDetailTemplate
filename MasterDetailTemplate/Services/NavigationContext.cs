using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MasterDetailTemplate.Services
{
    /// <summary>
    /// 導航上下文
    /// </summary>
    public static class NavigationContext
    {
        /// <summary>
        /// 導航參數屬性
        /// </summary>
        public static readonly BindableProperty NavigationParameterProperty =
            BindableProperty.CreateAttached("NavigationParameter", 
                                            typeof(object),
                                            typeof(NavigationContext),
                                            null,
                                            BindingMode.OneWayToSource);

        /// <summary>
        /// 設置導航參數
        /// </summary>
        /// <param name="bindableObject">設置導航參數需要的對象</param>
        /// <param name="value">導航參數</param>
        public static void SetParameter(BindableObject bindableObject,
                                        object value)
        {
            bindableObject.SetValue(NavigationParameterProperty, value);
        }
            
    }
}
