using System;
using System.ComponentModel;
using Android.Database;
using System.Globalization;

namespace INetSales.OfflineInterface.AndroidDb
{
    internal abstract class SqlReader
    {
        public const string PREFIX_PARAM = "@";

        public const string PARAM_QUERY_ID = "@ID";
		public const string PARAM_QUERY_CODIGO = "@" + DtoMap.CODIGO_COLUMN;
		public const string PARAM_QUERY_DATA_CRIACAO = "@" + DtoMap.DATA_CRIACAO_COLUMN;
		public const string PARAM_QUERY_DATA_ALTERACAO = "@" + DtoMap.DATA_ALTERACAO_COLUMN;
		public const string PARAM_QUERY_IS_DESABILITADO = "@" + DtoMap.IS_DESABILITADO_COLUMN;
		public const string PARAM_QUERY_IS_PENDING_UPLOAD = "@" + DtoMap.IS_PENDING_UPLOAD_COLUMN;
		public const string PARAM_QUERY_DATA_LAST_UPLOAD = "@" + DtoMap.DATA_LAST_UPLOAD_COLUMN;

        public static string GetParamText(string text)
        {
            return String.Format("{0}{1}", PREFIX_PARAM, text);
        }

        public static TValue GetValueOrDefault<TValue>(string text)
        {
            var converter = TypeDescriptor.GetConverter(typeof(TValue));
            return converter.CanConvertFrom(typeof(string)) ?
                (TValue)converter.ConvertFrom(text) :
                default(TValue);
        }

		public static string GetDateTimeText(DateTime value)
		{
			//Logger.Debug("Obtendo o texto da data - Value: {0} - Texto: {1}", value, value.ToString("yyyy-MM-dd HH:mm:ss"));
			return value.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static string GetBoolText(bool value)
		{
			return value ? "1" : "0";
		}

		public static string GetTextValue(object value)
		{
			if (value != null)
			{
				if (value.GetType().Equals(typeof (DateTime)))
				{
					return GetDateTimeText((DateTime) value);
				}
				if (value.GetType().Equals(typeof (bool)))
				{
					return GetBoolText((bool) value);
				}
				if (value.GetType().Equals(typeof(double)))
				{
					var numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
					numberFormat.NumberDecimalSeparator = ".";
					return Convert.ToString(value, numberFormat);
				}
				return value.ToString();
			}
			return String.Empty;
		}

		private readonly ICursor _cursor;
		public SqlReader(ICursor cursor)
		{
			_cursor = cursor;
		}

		public string GetString(int index)
		{
			if (index > -1 && !IsNull(index))
			{
				return _cursor.GetString(index);
			}
			return String.Empty;
		}

		public  int GetInt(int index)
		{
			if (index > -1 && !IsNull(index))
			{
				return _cursor.GetInt(index);
			}
			return default(int);
		}

		public  bool GetBool(int index)
		{
			if (index > -1 && !IsNull(index))
			{
				string boolText = _cursor.GetString(index);
				return boolText.Equals("0") ? false : true;
			}
			return default(bool);
		}

		public  char GetChar(int index)
		{
			if (index > -1 && !IsNull(index))
			{
				string charText = _cursor.GetString(index);
				return Char.Parse(charText);
			}
			return default(char);
		}

		public  double GetDouble(int index)
		{
			if (index > -1 && !IsNull(index))
			{
				string doubleText = _cursor.GetString(index);
				var numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
				numberFormat.NumberDecimalSeparator = ".";
				return Double.Parse(doubleText, numberFormat);
			}
			return default(double);
		}

		public int GetColumnIndex(string column)
		{
			return _cursor.GetColumnIndex(column);
		}

		public bool IsNull(int index)
		{
			return _cursor.IsNull(index);
		}

        public TValue GetValueOrDefault<TValue>(int index)
        {
            if (index >= 0 && !IsNull(index))
            {
                string text = GetString(index);
                var value = GetValueOrDefault<TValue>(text);
                return value;
            }
            return default(TValue);
        }
    }
}