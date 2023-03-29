using MasterDetailTemplate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MasterDetailTemplate.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;

        public App app = Application.Current as App;
        public string LogStatus = "LogStatus";

        public MenuPage()
        {
            // 必須要重開才能刷新Menu! 需要找方法重新整理列表
            InitializeComponent();
            menuItems = new List<HomeMenuItem>
                {
                    // new HomeMenuItem {Id = MenuItemType.Browse, Title="Browse" },
                    new HomeMenuItem {Id = MenuItemType.About, Title="首頁" },
                    new HomeMenuItem { Id = MenuItemType.LoginAndLogout,Title="用戶介面"},
                    new HomeMenuItem {Id = MenuItemType.AquariumWaterSituation, Title="魚缸水質狀況" }
                };


            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                // 若點選的不是LoginOrLogout的頁面 或 首頁
                var selectedItem = e.SelectedItem as HomeMenuItem; // 將選擇的項目轉換為 HomeMenuItem 類型
                string title = selectedItem.Title; // 獲取選擇項目的標題
                if (title == "用戶介面" || title == "首頁")
                {
                    if (e.SelectedItem == null)
                        return;
                    var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                    await RootPage.NavigateFromMenu(id);
                }
                else
                {
                    if (app.Properties[LogStatus].ToString() == "false")
                    {
                        await RootPage.NavigateFromMenu(3);
                        ListViewMenu.SelectedItem = menuItems[1];

                        await DisplayAlert("請先登入",
                               "登入後才可進行其餘操作。",
                               "OK");
                    }
                    else
                    {
                        if (e.SelectedItem == null)
                            return;
                        var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                        await RootPage.NavigateFromMenu(id);
                    }
                }
            };
        }
    }
}