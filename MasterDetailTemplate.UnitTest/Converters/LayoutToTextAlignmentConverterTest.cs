using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDetailTemplate.Converters;
using MasterDetailTemplate.Utils;
using MasterDetailTemplate.Models;
using Xamarin.Forms;

namespace MasterDetailTemplate.UnitTest.Converters
{
    /// <summary>
    /// 布局到文本轉換器測試
    /// </summary>
    public class LayoutToTextAlignmentConverterTest
    {
        [Test]
        public void TestConvertBack()
        {
            var layoutToTextAlignmentConverter =
                new LayoutToTextAlignmentConverter();

            Assert.Catch<DoNotCallMeException>(() =>
                layoutToTextAlignmentConverter.ConvertBack(null, null, null, null));
        }

        [Test]
        public void TestConvert()
        {
            var layoutToTextAlignmentConverter =
                new LayoutToTextAlignmentConverter();

            Assert.AreEqual(
                        TextAlignment.Center,
                        layoutToTextAlignmentConverter.Convert(Poetry.CenterLayout, null, null, null)
                    );

            Assert.AreEqual(
                        TextAlignment.Start,
                        layoutToTextAlignmentConverter.Convert(Poetry.IndentLayout, null, null, null)
                    );

            Assert.AreEqual(
                        null,
                        layoutToTextAlignmentConverter.Convert("XDXD", null, null, null)
                    );

            Assert.AreEqual(
                        null,
                        layoutToTextAlignmentConverter.Convert(null, null, null, null)
                    );
        }
    }
}
