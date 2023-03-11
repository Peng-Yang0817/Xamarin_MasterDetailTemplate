using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDetailTemplate.Services;
using MasterDetailTemplate.Models;
using Moq;

namespace MasterDetailTemplate.UnitTest.Helpers
{
    /// <summary>
    /// 詩詞存储幫助類
    /// </summary>
    
    public static class PoetryStorageHelper
    {
        /// <summary>
        /// TABLE中詩詞總數量
        /// </summary>
        public const int NumberPoetry = 139;

        public static async Task<PoetryStorage>
            GetInitializedPoetryStorageAsync()
        {
            var poetryStorage =
                new PoetryStorage(new Mock<IPreferenceStorage>().Object);

            // 數據庫被初始化了!
            await poetryStorage.InitializeAsync();
            return poetryStorage;
        }

        public static void RemoveDatabaseFile()
        {
            File.Delete(PoetryStorage.PoetryDbPath);
        }

    }
}
