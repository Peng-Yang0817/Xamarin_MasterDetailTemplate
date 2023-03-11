using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MasterDetailTemplate.Models;

namespace MasterDetailTemplate.Services
{
    /// <summary>
    /// 詩詞存储接口
    /// </summary>
    public interface IPoetryStorage
    {
        /// <summary>
        /// 初始化數據庫的
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// 判斷數據庫是否存在
        /// </summary>
        bool IsInitialized();

        /// <summary>
        /// 獲得一個詩詞
        /// </summary>
        /// <param name="id">詩詞id</param>
        /// <returns></returns>
        Task<Poetry> GetPoetryAsync(int id);

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
        Task<IList<Poetry>> GetPoetrysAsync(
                Expression<Func<Poetry, bool>> where, int skip, int take);

    }

    /// <summary>
    /// 詩詞存儲有關的常量
    /// </summary>
    public static class PoetryStorageConstants
    {
        /// <summary>
        /// 當前數據庫的版本號
        /// </summary>
        public const int Version = 1;
        /// <summary>
        /// 詩詞數據庫的版本號
        /// </summary>
        public const string VersionKey =
            nameof(PoetryStorageConstants) + "." + nameof(Version);
    }
}
