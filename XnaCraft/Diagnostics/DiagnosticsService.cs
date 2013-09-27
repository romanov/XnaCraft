using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Diagnostics
{
    class DiagnosticsService
    {
        Dictionary<string, object> _infoValues = new Dictionary<string, object>();

        public IDictionary<string, object> GetInfoValues()
        {
            return _infoValues;
        }

        public void SetInfoValue<T>(string key, T value)
        {
            if (!_infoValues.ContainsKey(key))
            {
                _infoValues.Add(key, value);
            }
            else
            {
                _infoValues[key] = value;
            }
        }
    }
}
