using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using Xamarin.Forms.Extended;
using MasterDetailTemplate.Models;
using MasterDetailTemplate.Services;
using System.Linq.Expressions;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;

namespace MasterDetailTemplate.ViewModels
{
    /// <summary>
    /// 搜索結果類
    /// </summary>
    public class ResultPageViewModel : ViewModelBase
    {
        // TODO
        private IPoetryStorage _poetryStorage;

        /// <summary>
        /// 內容導航服務
        /// </summary>
        private IContentNavigationService _contentNavigationService;

        // 構造函數 ******************************
        // 因為要讀詩詞數據，所以一定需要一個IPoetryStorage
        public ResultPageViewModel(IPoetryStorage poetryStorage,IContentNavigationService contentNavigationService)
        {
            _poetryStorage = poetryStorage;
            _contentNavigationService = contentNavigationService;

            PoetryCollection = new InfiniteScrollCollection<Poetry>
            {
                // 現在還能不能繼續家載
                OnCanLoadMore = () => _canLoadMore,
                // 如果OnCanLoadMore為true，那這裡就會繼續加載
                OnLoadMore = async () =>
                {
                    Status = Loading;
                    var poetrys =
                        await poetryStorage.GetPoetrysAsync(Where, PoetryCollection.Count, PageSize);

                    Status = "";

                    // 如果這次返回的結果數量已經不滿足PageSize設定的20條了!
                    // 那代表已經沒有資料可以繼續返回，因次必須將OnConLoadMore設置為False
                    if (poetrys.Count < PageSize)
                    {
                        Status = NoMoreResult;
                        _canLoadMore = false;
                    }
                    // 當返回的poetrys長度為0，並且目前Collection內也沒有東西，那代表沒有查到對應結果
                    if (poetrys.Count == 0 && PoetryCollection.Count == 0)
                    {
                        Status = NoResult;
                    }

                    return poetrys;
                }
            };
        }

        // 綁定屬性 ******************************
        // 無限滾動屬性需要載額外套件 Xamarin.Forms.Extended.InfiniteScrolling
        /// <summary>
        /// 詩詞集合，這裡的名稱必須跟 View 中 ViewList Binding 的 一樣才行
        /// </summary>
        public InfiniteScrollCollection<Poetry> PoetryCollection { get; }

        // 綁定命令 ******************************
        // 當按下查詢後，會跳轉到結果頁面，我們必須將結果頁面的內容清乾淨
        private RelayCommand _pageAppearingCommand;
        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ?? (_pageAppearingCommand = new RelayCommand(
                async () => await PageAppearingCommandFunction()
            ));

        public async Task PageAppearingCommandFunction()
        {
            // TODO 共演示使用，之後要刪除
            Where = Expression.Lambda<Func<Poetry, bool>>(
                    Expression.Constant(true),
                    Expression.Parameter(typeof(Poetry), "p"));

            await _poetryStorage.InitializeAsync();

            if (!_isNewQuery)
            {
                return;
            }
            _isNewQuery = false;
            PoetryCollection.Clear();
            _canLoadMore = true;
            await PoetryCollection.LoadMoreAsync();
        }

        /// <summary>
        /// 詩詞點擊命令
        /// </summary>
        private RelayCommand<Poetry> _poetryTappedCommand;

        public RelayCommand<Poetry> PoetryTappedCommand =>
            _poetryTappedCommand ??
                (_poetryTappedCommand = new RelayCommand<Poetry>(
                        async p => await PoetryTappedCommmandFunction(p)
                    ));

        public async Task PoetryTappedCommmandFunction(Poetry poetry)
        {
            await _contentNavigationService.NavigateToAsync(ContentNavigationConstants.DetailPage, poetry);
        }

        // 公開變量_(常量) ******************************
        public const string Loading = "正在載入";
        public const string NoResult = "沒有滿足條件的結果";
        public const string NoMoreResult = "沒有更多結果";
        // 一次加載多少個
        public const int PageSize = 20;


        // 公開變量_(變量) ******************************
        // 這裡set的寫法表示，Status在賦值時，當值發生變化，就會廣播，這樣前端就會知道
        public string Status
        {
            get => _status;
            set => Set(nameof(Status), ref _status, value);
        }

        // 私有變量_(變量) ******************************
        private string _status;


        // 公開變量_(變量) ******************************
        // 這裡set的寫法表示，Where在賦值時，當值發生變化，就會廣播，這樣前端就會知道
        public Expression<Func<Poetry, bool>> Where
        {
            get => _where;
            // 當有新的查詢條件過來就自動設為true
            set
            {
                Set(nameof(Where), ref _where, value);
                _isNewQuery = true;
            }
        }

        // 私有變量_(變量) ******************************
        private Expression<Func<Poetry, bool>> _where;

        // 私有變量_(變量) ******************************
        private bool _canLoadMore;

        /// <summary>
        /// 是否為新查詢
        /// </summary>
        private bool _isNewQuery;
    }
}
