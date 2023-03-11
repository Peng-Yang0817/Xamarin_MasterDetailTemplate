using MasterDetailTemplate.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MasterDetailTemplate.Services
{
    public class PoetryStorage : IPoetryStorage
    {
        // ******* 公開變量
        /// <summary>
        /// 數據庫文件路徑
        /// </summary>
        public static readonly string PoetryDbPath =
                Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData),
                            DbName
                    );

        // ******* 私有變量
        private const string DbName = "poetrydb.sqlite3";
        

        // ******* 繼承方法

        /// <summary>
        /// 初始化數據庫的
        /// </summary>
        public async Task InitializeAsync()
        {
            // 為了把把目標文件部屬到行動端，我需要先找到行動端存放資源的特殊路徑
            // 若文件存在就覆蓋，這裡也就是覆蓋PoetryDbPath下的文件，沒有就創建
            using (var dbFileStream =
                    new FileStream(PoetryDbPath, FileMode.Create))
            {
                // 打開資料來源的db
                using (var dbAssertStream =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(DbName))
                {
                    // 拷貝流，把dbAssertStream的流拷貝到dbFileStream
                    await dbAssertStream.CopyToAsync(dbFileStream);
                }
            }
            // 在Essentials存放版本好，以便確認是否有初始化
            _preferenceStorage.Set(PoetryStorageConstants.VersionKey,
                (int)PoetryStorageConstants.Version);
        }

        /// <summary>
        /// 判斷數據庫是否存在
        /// </summary>
        public bool IsInitialized()
        {
            int mobileDBVersionNum = 
                _preferenceStorage.Get(PoetryStorageConstants.VersionKey, 0);

            // 如果偏好存取的本版號與目前接口的版本號一樣，那就代表已經初始化過
            if (mobileDBVersionNum == PoetryStorageConstants.Version)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// 數據庫連接，如果為空就直接連接預設好的路徑
        /// </summary>
        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Connection =>
            _connection ?? (_connection = new SQLiteAsyncConnection(PoetryDbPath));

        /// <summary>
        /// 偏好存储
        /// </summary>
        private IPreferenceStorage _preferenceStorage;

        /// <summary>
        /// 獲得一個詩詞
        /// </summary>
        /// <param name="id">詩詞id</param>
        /// <returns></returns>
        public async Task<Poetry> GetPoetryAsync(int id)
        {
            // 將表中id對應的資料抓出
            return await Connection.Table<Poetry>().FirstOrDefaultAsync(x=>x.Id == id);
        }

        /// <summary>
        /// 獲取滿足給訂條件的詩持集合。
        /// 這寫法就是寫一個查詢條件，滿足就返回，
        /// 不滿足就不返回。
        /// 從skip往後開始數take條資料。
        /// </summary>
        /// <param name="where">where 條件</param>
        /// <param name="skip">跳過結果的數量</param>
        /// <param name="take">返回結果的數量</param>
        /// <returns></returns>
        public async Task<IList<Poetry>> GetPoetrysAsync
            (Expression<Func<Poetry, bool>> where, int skip, int take)
        {
            return await Connection.Table<Poetry>().Where(where).Skip(skip).Take(take).ToListAsync();
        }

        // ******** 建構方法
        public PoetryStorage(IPreferenceStorage preferenceStorage) {
            _preferenceStorage = preferenceStorage;
        }

        /// <summary>
        /// 關閉數據庫
        /// </summary>
        public async Task CloseAsync() { 
            await Connection.CloseAsync();
        }
    }
}
