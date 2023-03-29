using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDetailTemplate.Models
{
    public class ByteArrayToDictionary
    {
        /// <summary>
        /// 將byte[] 轉成 Json的方法
        /// </summary>
        /// <param name="jsonSrting">要被轉換的 byte[]</param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(string jsonSrting)
        {
            var jsonObject = JObject.Parse(jsonSrting);
            var jTokens = jsonObject.Descendants().Where(p => !p.Any());
            var tmpKeys = jTokens.Aggregate(new Dictionary<string, string>(),
                (properties, jToken) =>
                {
                    properties.Add(jToken.Path, jToken.ToString());
                    return properties;
                });
            return tmpKeys;
        }
    }
}
