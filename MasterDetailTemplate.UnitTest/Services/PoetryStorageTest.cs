using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDetailTemplate.Services;
using MasterDetailTemplate.Models;
using Moq;
using MasterDetailTemplate.UnitTest.Helpers;
using System.Linq.Expressions;

namespace MasterDetailTemplate.UnitTest.Services
{
    /// <summary>
    /// 詩詞存儲測試
    /// </summary>
    public class PoetryStorageTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile() =>
            PoetryStorageHelper.RemoveDatabaseFile();

        /// <summary>
        /// 測試初始化數據庫
        /// </summary>
        [Test]
        public async Task TestInitializeAsync()
        {
            // 看這個路徑是否存在，若存在就不對了! 這就是斷言
            // 斷言這個路徑不存在，所以會返回False
            Assert.IsFalse(File.Exists(PoetryStorage.PoetryDbPath));

            var preferenceStorageMock = new Mock<IPreferenceStorage>();
            var mockPreferenceStorage = preferenceStorageMock.Object;

            var poetryStorage = new PoetryStorage(mockPreferenceStorage);
            await poetryStorage.InitializeAsync();

            // 初始化完成後，文件就應該存在，所以是IsTrue
            Assert.IsTrue(File.Exists(PoetryStorage.PoetryDbPath));

            // 這樣可以確認這個Mock是否有調用過這個參數，並傳入這幾個值，一次
            // 這樣就可以驗證這個函數是否被調用
            preferenceStorageMock.Verify(x => x.Set(
                    PoetryStorageConstants.VersionKey,
                    PoetryStorageConstants.Version),
                    Times.Once
            );
        }

        /// <summary>
        /// 測試是否已被初始化數據庫
        /// </summary>
        [Test]
        public void TestIsInitialized()
        {
            var preferenceStorageMock = new Mock<IPreferenceStorage>();

            // 只要你調用這個函數，我就把預設版本回傳給你。
            preferenceStorageMock.Setup(x => x.Get(
                PoetryStorageConstants.VersionKey,
                0))
                .Returns(PoetryStorageConstants.Version);

            var mockPreferenceStorage = preferenceStorageMock.Object;

            var poetryStorage = new PoetryStorage(mockPreferenceStorage);
            Assert.IsTrue(
                    poetryStorage.IsInitialized()
                );

        }

        /// <summary>
        /// 測試沒有被初始化數據庫
        /// </summary>
        [Test]
        public void TestIsNotInitialized()
        {
            var preferenceStorageMock = new Mock<IPreferenceStorage>();

            // 只要你調用這個函數，我就把預設版本回傳給你。
            preferenceStorageMock.Setup(x => x.Get(
                PoetryStorageConstants.VersionKey,
                0))
                .Returns(PoetryStorageConstants.Version - 1);

            var mockPreferenceStorage = preferenceStorageMock.Object;

            var poetryStorage = new PoetryStorage(mockPreferenceStorage);
            Assert.IsFalse(
                    poetryStorage.IsInitialized()
                );

        }
        /// <summary>
        /// 獲得一個詩詞
        /// </summary>
        [Test]
        public async Task TestGetPoetryAsync()
        {

            PoetryStorage poetrystorage = 
                await PoetryStorageHelper.GetInitializedPoetryStorageAsync();

            var poetry = await poetrystorage.GetPoetryAsync(10001);

            await poetrystorage.CloseAsync();

            Assert.AreEqual("临江仙 · 夜归临皋",poetry.Name);
        }

        /// <summary>
        /// 獲得滿足條件的詩詞
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestGetPoetrysAsync() {
            PoetryStorage poetrystorage =
                await PoetryStorageHelper.GetInitializedPoetryStorageAsync();

            var where = Expression.Lambda<Func<Poetry, bool>>(
                    Expression.Constant(true),
                    Expression.Parameter(typeof(Poetry), "p"));

            var poetrys = await poetrystorage.GetPoetrysAsync(where, 0, int.MaxValue);

            await poetrystorage.CloseAsync();
        }
    }
}
