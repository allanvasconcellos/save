using System.Collections.Generic;
using System.ComponentModel;

namespace INetSales.OfflineInterface
{
    public class FluentParameter
    {
        private Dictionary<string, object> Map { get; set; }
        private string _idKey;

        private FluentParameter()
        {
            Map = new Dictionary<string, object>();
            _idKey = string.Empty;
        }

        public static FluentParameter Create()
        {
            return new FluentParameter();
        }

        public static FluentParameter Open(Dictionary<string, object> from, string idKey)
        {
            var p = new FluentParameter();
            p._idKey = idKey;
            foreach (var o in from)
            {
                p.Add(o.Key, o.Value);
            }
            return p;
        }

        public Dictionary<string, object> Mapper()
        {
            return Map;
        }

        public FluentParameter Add(string key, object value)
        {
            Map.Add(SqlReader.GetParamText(key), value);
            return this;
        }

        public int GetId()
        {
            if (!string.IsNullOrEmpty(_idKey))
            {
                return (int)Map[SqlReader.GetParamText(_idKey)];
            }
            return 0;
        }
    }
}