using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace XnaCraft.Engine.Diagnostics
{
    class DiagnosticsService
    {
        ConcurrentDictionary<string, object> _infoValues = new ConcurrentDictionary<string, object>();

        public IDictionary<string, object> GetInfoValues()
        {
            return _infoValues;
        }

        public void SetInfoValue<T>(string key, T value)
        {
            _infoValues[key] = value;
        }
    }
}
