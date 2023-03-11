using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDetailTemplate.Converters;
using MasterDetailTemplate.Utils;
using MasterDetailTemplate.Models;

namespace MasterDetailTemplate.UnitTest.Converters
{
    public class TextIndentConverterTest
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
        public void TestConvertFailed()
        {
            var textIndentConverter = new TextIndentConverter();
            Assert.IsNull(textIndentConverter.Convert(new Poetry(), null, null, null));
        }

        [Test]
        public void TestConvertSuccess() { 
            var textIndentConverter = new TextIndentConverter();
            var stringToConvert = "A\nB\nC";
            var stringConverted =
                textIndentConverter.Convert(stringToConvert, null, null, null) as string;

            Assert.AreEqual(stringConverted, "\u3000\u3000A\n\u3000\u3000B\n\u3000\u3000C");
        }
    }
}
