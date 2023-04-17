using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailTemplate.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        Test,
        LoginAndLogout,
        AquariumWaterSituation,
        HistoricalRecordSearch,
        BindAndEdit,
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
