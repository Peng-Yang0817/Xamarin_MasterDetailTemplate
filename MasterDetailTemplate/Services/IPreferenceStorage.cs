using System;
using System.Collections.Generic;
using System.Text;

namespace MasterDetailTemplate.Services
{
    /// <summary>
    /// 偏好存储接口
    /// </summary>
    public interface IPreferenceStorage
    {
        void Set(string key, string value);
        string Get(string key,string defaultValue);


        void Set(string key, int value);
        int Get(string key, int defaultValue);
    }
}
