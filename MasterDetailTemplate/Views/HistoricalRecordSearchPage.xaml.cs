using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MasterDetailTemplate.Models.ServerAccessSet;
using System.Net;
using System.Collections.Specialized;
using System.Threading;
using Newtonsoft.Json;

namespace MasterDetailTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoricalRecordSearchPage : ContentPage
    {
        // 用於紀錄當前輸入框的文字長度
        public int EntryLength = 0;

        public App app = Application.Current as App;
        public string AquariumUnitNum = "AquariumUnitNum";
        public string Auth001Id = "Auth001Id";

        private serverAccessSet server = new serverAccessSet();
        public HistoricalRecordSearchPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 覆寫重新進到該頁面後的刷新。
        /// </summary>
        protected override async void OnAppearing()
        {
            Appearing_RefreshView.IsEnabled = true;
            Appearing_RefreshView.IsRefreshing = true;
            await Task.Delay(200);

            txtFishTankNum.Text = "";

            // 清空StackLayout
            ClearStackLayoutData();

            Appearing_RefreshView.IsEnabled = false;
            Appearing_RefreshView.IsRefreshing = false;
        }

        /// <summary>
        /// 覆寫離開頁面後的刷新。
        /// </summary>
        protected override async void OnDisappearing()
        {
            Appearing_RefreshView.IsEnabled = true;
            Appearing_RefreshView.IsRefreshing = true;
            await Task.Delay(200);

            txtFishTankNum.Text = "";

            // 清空StackLayout
            ClearStackLayoutData();

            Appearing_RefreshView.IsEnabled = false;
            Appearing_RefreshView.IsRefreshing = false;
        }

        /// <summary>
        /// 看文字長度更變厚，是否要更換顏色
        /// </summary>
        private void TxtFishTankNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            EntryLength = e.NewTextValue.Length;

            if (e.NewTextValue.Length != 16)
            {
                btnSearch.BackgroundColor = Color.FromHex("#74bbf3");
                btnSearch.TextColor = Color.FromHex("#ffffff");

            }
            else
            {
                btnSearch.BackgroundColor = Color.FromHex("#2196F3");
                btnSearch.TextColor = Color.FromHex("#ffffff");
            }
        }

        /// <summary>
        /// 按鈕按下去後，查看是否要發送請求。
        /// </summary>
        private async void BtnSearch_Clicked(object sender, EventArgs e)
        {
            // 清空StackLayout
            ClearStackLayoutData();
            if (!server.IsNetworkAvailable())
            {
                await DisplayAlert("警告", "請先連接至網際網路。", "確定");
            }
            else
            {
                if (EntryLength != 16)
                {
                    await DisplayAlert("警告", "請輸入完整的16位元組。", "確定");
                }
                else
                {
                    Appearing_RefreshView.IsEnabled = true;
                    Appearing_RefreshView.IsRefreshing = true;
                    await Task.Delay(300);

                    // 將用戶填入的魚缸編號取出
                    Entry Entry_txtFishTankNum = (Entry)FindByName("txtFishTankNum");
                    string string_txtFishTankNum = Entry_txtFishTankNum.Text;

                    // 向Server抓取資料
                    List<AquaruimNumBindHistory> DataList = await GetMyChartNeedData(string_txtFishTankNum);

                    if (DataList != null)
                    {
                        // 紀錄資料長度
                        int DataLength = DataList.Count;

                        if (DataLength != 0)
                        {
                            // 初始化一個Grid
                            Grid grid_content = InitGridSet();

                            // 將抓到的資料放入Grid中
                            for (int i = 0; i < DataLength; i++)
                            {
                                ModifyGridToAddData(grid_content, i, DataList[i]);
                            }

                            AddGridInToStackLayout(grid_content);
                        }
                        else
                        {
                            await DisplayAlert("提示", "查無該魚缸的綁定紀錄。", "確定");
                        }
                    }

                    Appearing_RefreshView.IsRefreshing = false;
                    Appearing_RefreshView.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 初始化要放資料的Grid
        /// </summary>
        /// <returns></returns>
        public Grid InitGridSet()
        {
            var grid_content_temp = new Grid
            {
                ColumnSpacing = 2,
                RowSpacing = 1,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                },
                ColumnDefinitions =
                 {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            return grid_content_temp;
        }

        /// <summary>
        /// 編輯單行資訊，塞入後再將Grid返回
        /// </summary>
        /// <param name="grid_content">編輯中的Grid</param>
        /// <param name="TargetRow">目前該資料要放在的Row</param>
        /// <param name="data">要塞入的該筆資料</param>
        /// <returns></returns>
        public Grid ModifyGridToAddData(Grid grid_content, int TargetRow, AquaruimNumBindHistory data)
        {
            // 序列 (欄0)
            var stackLayout_RowNum = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = (TargetRow+1).ToString(),
                        TextColor = Color.Black,
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                    }
                }
            };

            // 綁定日期 (欄1)
            // 22/10/24-19:20
            string Day = data.createTime.Substring(0, 8);
            string Time = data.createTime.Substring(9, 5);

            var stackLayout_BindDate = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = Day,
                        TextColor = Color.Black,
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                    },
                    new Label
                    {
                        Text = Time,
                        TextColor = Color.Black,
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                    },
                }
            };

            // 編輯日期 (欄1)
            // 22/10/24-19:20
            Day = data.modifyTime.Substring(0, 8);
            Time = data.modifyTime.Substring(9, 5);
            var stackLayout_modifyDate = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = Day,
                        TextColor = Color.Black,
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                    },
                    new Label
                    {
                        Text = Time,
                        TextColor = Color.Black,
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                    }
                }
            };

            // 狀態 (欄3)
            var stackLayout_State = new StackLayout
            {
                Style = (Style)Application.Current.Resources["Table_Content_Style"],
                Children =
                {
                    new Label
                    {
                        Text = data.bindtag,
                        TextColor = Color.Black,
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                    }
                }
            };

            grid_content.Children.Add(stackLayout_RowNum);
            Xamarin.Forms.Grid.SetRow(stackLayout_RowNum, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_RowNum, 0);

            grid_content.Children.Add(stackLayout_BindDate);
            Xamarin.Forms.Grid.SetRow(stackLayout_BindDate, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_BindDate, 1);

            grid_content.Children.Add(stackLayout_modifyDate);
            Xamarin.Forms.Grid.SetRow(stackLayout_modifyDate, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_modifyDate, 2);

            grid_content.Children.Add(stackLayout_State);
            Xamarin.Forms.Grid.SetRow(stackLayout_State, TargetRow);
            Xamarin.Forms.Grid.SetColumn(stackLayout_State, 3);

            return grid_content;
        }

        /// <summary>
        /// 清空塞資料的StackLayout
        /// </summary>
        public void ClearStackLayoutData()
        {
            stackLayout_DataList.Children.Clear();
        }

        /// <summary>
        /// 將編輯完成的Grid放入 stackLayout_DataList
        /// </summary>
        /// <param name="grid_content"></param>
        public void AddGridInToStackLayout(Grid grid_content)
        {
            stackLayout_DataList.Children.Add(grid_content);
        }

        // ========================================================================== 取得伺服器回傳魚缸綁定紀錄
        /// <summary>
        /// 取得伺服器回傳的魚缸綁定紀錄
        /// </summary>
        public async Task<List<AquaruimNumBindHistory>> GetMyChartNeedData(string AquariumNum)
        {
            // 準備裝曲線圖的資料集合
            List<AquaruimNumBindHistory> DataList = new List<AquaruimNumBindHistory>();

            using (var wb = new WebClient())
            {
                var dataSendUse = new NameValueCollection();

                string urlSendUse = server.ServerIP + "/MobileService/GetAquaruimNumBindHistory";

                string Bearer = "Bearer " + "jpymJUKgpjPp49GbC6onVCBlNYZfIDHfi5hypNrPXh1";
                wb.Headers.Add("Authorization", Bearer);

                // 準備魚缸編號，用於查詢
                dataSendUse["AquariumNum"] = AquariumNum;

                // 使用 CancellationTokenSource 取消等待
                var cts = new CancellationTokenSource();

                // 建立 timeoutTask 來等待 WaitTimeToServerResponese 的時間
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(server.AccessTimeOut), cts.Token);

                // 建立 responseTask 來執行伺服器回應
                var responseTask = wb.UploadValuesTaskAsync(urlSendUse, "POST", dataSendUse);

                // 等待 timeoutTask 與 responseTask 中的其中一個 Task 完成
                Task completedTask = await Task.WhenAny(timeoutTask, responseTask);

                cts.Cancel();

                if (completedTask == timeoutTask)
                {
                    await DisplayAlert("警告", "伺服器回應超時，請稍後再試。", "確定");
                    return null;
                }

                // 如果 responseTask 完成，則繼續執行下面的程式碼
                var responseSendUse = await responseTask;

                string str = Encoding.UTF8.GetString(responseSendUse);

                // 解析JSON string
                DataList = JsonConvert.DeserializeObject<List<AquaruimNumBindHistory>>(str);

                return DataList;

            }
        }
    }

    // 自定義魚缸綁定歷史紀錄類別
    public class AquaruimNumBindHistory
    {
        public string createTime { get; set; }
        public string modifyTime { get; set; }
        public string bindtag { get; set; }
    }
}