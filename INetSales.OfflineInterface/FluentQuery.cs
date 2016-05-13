using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace INetSales.OfflineInterface
{
    public class FluentQuery
    {
        private readonly StringBuilder _selectBuild;
        private readonly StringBuilder _fromBuild;
        private readonly StringBuilder _whereBuild;
		private readonly StringBuilder _orderBuild;
        public string CommandText
        {
            get { return _selectBuild + " " + _fromBuild + " " + _whereBuild + " " +  _orderBuild; }
        }

        public Dictionary<string, object> Parameters { get; private set; }

        private FluentQuery()
        {
            _selectBuild = new StringBuilder();
            _fromBuild = new StringBuilder();
            _whereBuild = new StringBuilder();
			_orderBuild = new StringBuilder();
        }

        public static FluentQuery Create()
        {
            return new FluentQuery();
        }

        public FluentQuery AddSelect(string fields)
        {
            _selectBuild.Clear();
            _selectBuild.AppendFormat("Select {0}", fields);
            return this;
        }

        public FluentQuery AddFrom(string table)
        {
            _fromBuild.Clear();
            _fromBuild.AppendFormat("From {0}", table);
            return this;
        }

        public FluentQuery AddWhere(string filter, Dictionary<string, object> parameters)
        {
            _whereBuild.Clear();
            _whereBuild.AppendFormat("Where " + filter, parameters.Keys
                .Select(c => c  + " ")
                .ToArray());
            Parameters = parameters;
            return this;
        }

		public FluentQuery AddOrderAscending(params string[] fields)
		{
			_orderBuild.Clear();
			_orderBuild.AppendFormat("Order {0} ASC", fields.Select(c => c + ","));
			return this;
		}

		public FluentQuery AddOrderDescending(params string[] fields)
		{
			_orderBuild.Clear();
			_orderBuild.AppendFormat("Order {0} DESC", fields.Select(c => c + ","));
			return this;
		}
    }
}