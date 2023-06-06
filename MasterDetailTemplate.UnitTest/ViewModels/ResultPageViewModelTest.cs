using MasterDetailTemplate.Models;
using MasterDetailTemplate.Services;
using MasterDetailTemplate.UnitTest.Helpers;
using MasterDetailTemplate.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MasterDetailTemplate.UnitTest.ViewModels
{
    /// <summary>
    /// 搜索結果頁ViewModel測試
    /// </summary>
    public class ResultPageViewModelTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile() =>
            PoetryStorageHelper.RemoveDatabaseFile();

        /// <summary>
        /// 測試詩詞集合
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestPoetryCollection()
        {
            PoetryStorage poetrystorage =
                await PoetryStorageHelper.GetInitializedPoetryStorageAsync();

            var where = Expression.Lambda<Func<Poetry, bool>>(
                    Expression.Constant(true),
                    Expression.Parameter(typeof(Poetry), "p"));

            var resultPageViewModel = new ResultPageViewModel(poetrystorage, null);
            resultPageViewModel.Where = where;

            var statusList = new List<string>();
            resultPageViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ResultPageViewModel.Status))
                {
                    statusList.Add(resultPageViewModel.Status);
                }
            };

            // 初始時沒有資料，所以長度是0
            Assert.AreEqual(0, resultPageViewModel.PoetryCollection.Count);
            // 頁面刷新，會初始化舊有資料，自動重新跑一次LoadMoreAsync，取得20筆資料
            // 第一次Status變化 = "載入中"
            // 第二次Status變化 = ""
            await resultPageViewModel.PageAppearingCommandFunction();
            // 比對是否是20筆資料
            Assert.AreEqual(20, resultPageViewModel.PoetryCollection.Count);

            Assert.AreEqual(2, statusList.Count);
            Assert.AreEqual(ResultPageViewModel.Loading, statusList[0]);
            Assert.AreEqual("", statusList[1]);

            var poetryCollectionChanged = false;

            // 若Where沒有重新附值，那就不會觸發頁面家載
            resultPageViewModel.PoetryCollection.CollectionChanged +=
                (sender, args) => poetryCollectionChanged = true;

            //resultPageViewModel.Where = where;
            //await resultPageViewModel.PageAppearingCommandFunction();

            Assert.IsFalse(poetryCollectionChanged);


            // 再次載入資料
            // 第三次Status變化 = "載入中"
            // 第四次Status變化 = ""
            await resultPageViewModel.PoetryCollection.LoadMoreAsync();

            // 當前資料筆數是否為40比
            Assert.AreEqual(40, resultPageViewModel.PoetryCollection.Count);

            // 當前數據庫中是否還有資料可以繼續加載?
            Assert.IsTrue(resultPageViewModel.PoetryCollection.CanLoadMore);

            Assert.AreEqual(4, statusList.Count);

            await poetrystorage.CloseAsync();
        }

        [Test]
        public async Task TestPoetryTappedCommand()
        {
            var contentNavigationServiceMock =
                new Mock<IContentNavigationService>();

            var mockContentNavigationService =
                contentNavigationServiceMock.Object;

            var poetryToTap = new Poetry();
            var resultPageViewModel =
                new ResultPageViewModel(null, mockContentNavigationService);
            await resultPageViewModel.PoetryTappedCommmandFunction(poetryToTap);

            contentNavigationServiceMock.Verify(p => p.NavigateToAsync(ContentNavigationConstants.DetailPage, poetryToTap), Times.Once);
        }

        [Test]
        public  void TestLoopRemoveListItem()
        {
            List<string> strings = new List<string>
            {
                "AA",
                "BB",
                "CC"
            };

            for (int i = 0; i < 3; i++)
            {
                if (strings[i] == "BB")
                {
                    strings[i] = "8787";
                }
            }
            Console.WriteLine(strings);
            GGDa(strings);
            int Xi = 0;
        }

        public static void GGDa(List<string> strings_1) 
        {
            strings_1.Remove(strings_1[1]);
            Console.WriteLine(strings_1);
        }
    }
}
