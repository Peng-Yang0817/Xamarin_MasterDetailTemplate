using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDetailTemplate.Models;
using MasterDetailTemplate.Converters;
using MasterDetailTemplate.Utils;
using Xamarin.Forms;

namespace MasterDetailTemplate.UnitTest.Converters
{
    public class ItemTappedEventArgsToPoetryConverterTest
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
            var itemTappedEventArgsToPoetryConverter =
                new ItemTappedEventArgsToPoetryConverter();

            Assert.IsNull(
                    itemTappedEventArgsToPoetryConverter.Convert(new object(), null, null, null)
                );

            var itemTappedEventArgsToConvert = new ItemTappedEventArgs(new object(), null, -1);

            Assert.IsNull(
                    itemTappedEventArgsToPoetryConverter.Convert(itemTappedEventArgsToConvert, null, null, null)
                );
        }

        [Test]
        public void TestConvertSucceded()
        {
            var itemTappedEventArgsToPoetryConverter =
                new ItemTappedEventArgsToPoetryConverter();

            var poetryToReturn = new Poetry();

            var itemTappedEventArgsToConvert =
                new ItemTappedEventArgs(null, poetryToReturn, -1);

            Assert.AreSame(poetryToReturn, 
                itemTappedEventArgsToPoetryConverter.Convert(itemTappedEventArgsToConvert, null, null, null)
                );
        }
    }
}
