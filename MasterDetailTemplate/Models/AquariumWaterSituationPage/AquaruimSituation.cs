using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailTemplate.Models.AquariumWaterSituationPage
{
    public class AquaruimSituation
    {
        public int Auth001Id { get; set; }
        public string AquariumUnitNum { get; set; }
        public string WaterType { get; set; }
        public string BindTag { get; set; }
        public int AquariumId { get; set; }
        public string temperature { get; set; }
        public string Turbidity { get; set; }
        public string PH { get; set; }
        public string TDS { get; set; }
        public string WaterLevel { get; set; }
        public DateTime createTime { get; set; }
        public string WaterLevelNum { get; set; }
    }
}
