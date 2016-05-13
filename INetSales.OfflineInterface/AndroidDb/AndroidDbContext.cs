using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using INetSales.Objects;
using INetSales.OfflineInterface.Dal;

namespace INetSales.OfflineInterface.AndroidDb
{
	internal class AndroidDbContext<TDto> : IDbContext<TDto>
		where TDto : IDto, new()
    {
        public const string DATABASE_NAME = "inetsales.db";
        public const int DATABASE_VERSION = 13;

		DtoMap<TDto> map;

		public AndroidDbContext(DtoMap<TDto> map)
		{
			this.map = map;
		}

        #region Static Member

        private static DatabaseHelper _helper;
        private static readonly object _lockHelper = new object();
        private static DatabaseHelper Helper
        {
            get
            {
                if(_helper == null)
                {
                    lock (_lockHelper)
                    {
                        if (_helper == null)
                        {
                            _helper = new DatabaseHelper(Application.Context, DATABASE_NAME, DATABASE_VERSION);
                        }
                    }
                }
                return _helper;
            }
        }

        public static void CloseAll()
        {
            Helper.Close();
        }

        #endregion

        #region Implementation of IDbContext

        #region Query

        public TDto Find<TDto>(int id, Action<TDto> dataBound = null) 
        {
            var db = Helper.ReadableDatabase;
            var dto = default(TDto);
            using (var cursor = db.Query(true, map.TableName, map.Fields, 
                        String.Format("{0} = ?", map.PrimaryKey), new[] { id.ToString() }, null, null, null, null))
            {
                if (cursor.MoveToNext())
                {
                    var reader = new SqlReader(cursor);
                    dto = new TDto();
					if (map.Map (reader, dto)) {
						if(dataBound != null) dataBound (dto);
					}
                }
                return dto;
            }
        }

        public TDto FindByCodigo<TDto>(string codigo, Action<TDto> dataBound = null)
        {
            var db = Helper.ReadableDatabase;
            var dto = default(TDto);
            using (var cursor = db.Query(true, map.TableName, map.Fields,
				String.Format("Upper({0}) = ?", DtoMap<TDto>.CODIGO_COLUMN), new[] { codigo.ToUpper() }, null, null, null, null))
            {
                if (cursor.MoveToNext())
                {
                    var reader = new SqlReader(cursor);
					dto = new TDto ();
					if (map.Map (reader, dto)) {
						if(dataBound != null) dataBound (dto);
					}
                }
                return dto;
            }
        }

        public bool Exist(IDictionary<string, object> where)
        {
			var query = FluentQuery.Create ()
				.AddSelect ("1")
				.AddFrom (map.GetTableName(String.Empty));
			foreach (var keyPair in where) {
				query.AddWhere (keyPair.Key + " = {0}", FluentParameter.Create ()
					.Add (keyPair.Key, keyPair.Value)
					.Mapper ());
			}
            var o = GetScalar<int>(query.CommandText, query.Parameters);
            return o > 0;
        }

        public int GetNextPkValue()
        {
            var db = Helper.ReadableDatabase;
            using (var cursor = db.Query(true, map.TableName, new[] {"Max(" + map.PrimaryKey + ")"}, null, null, null, null, null, null))
            {
                if (cursor.MoveToNext())
                {
                    var reader = new SqlReader(cursor);
                    return reader.GetInt(0);
                }
                return -1;
            }
        }

		public TDto GetObject(IDictionary<string, object> where, Action<TDto> dataBound = null)
		{
			var query = FluentQuery.Create ()
				.AddSelect (map.GetQueryField(String.Empty))
				.AddFrom (map.GetTableName(String.Empty));
			foreach (var keyPair in where) {
				query.AddWhere (keyPair.Key + " = {0}", FluentParameter.Create ()
					.Add (keyPair.Key, keyPair.Value)
					.Mapper ());
			}
			return GetObject(query.CommandText, query.Parameters, dataBound);
		}

        private TDto GetObject(string query, IDictionary<string, object> parameters, Action<TDto> dataBound = null) 
        {
            var db = Helper.ReadableDatabase;
            var dto = default(TDto);
            string[] parametersOrdained = parameters != null && parameters.Count > 0 ? GetParametersInOrder(query, parameters) : null;
            string queryPrepared = parameters != null && parameters.Count > 0
                                       ? PrepareQuery(query, parameters.Keys)
                                       : query;
            using (var cursor = db.RawQuery(queryPrepared, parametersOrdained))
            {
                if (cursor.MoveToNext())
                {
                    var reader = new SqlReader(cursor);
                    dto = new TDto();
					if (map.Map (reader, dto)) {
						if(dataBound != null) dataBound (dto);
					}
                }
                return dto;
            }
        }

		public List<TDto> GetList(IDictionary<string, object> where, Action<TDto> dataBound = null)
		{
			var query = FluentQuery.Create ()
				.AddSelect (map.GetQueryField(String.Empty))
				.AddFrom (map.GetTableName(String.Empty));
			foreach (var keyPair in where) {
				query.AddWhere (keyPair.Key + " = {0}", FluentParameter.Create ()
					.Add (keyPair.Key, keyPair.Value)
					.Mapper ());
			}
			return GetList(query.CommandText, query.Parameters, dataBound);
		}

		private List<TDto> GetList(string query, IDictionary<string, object> where, Action<TDto> dataBound = null)
        {
            var db = Helper.ReadableDatabase;
            var list = new List<TDto>();
            string[] parametersOrdained = where != null && where.Count > 0 ? GetParametersInOrder(query, where) : null;
            string queryPrepared = where != null && where.Count > 0 ? PrepareQuery(query, where.Keys) : query;
            using (var cursor = db.RawQuery(queryPrepared, parametersOrdained))
            {
                while (cursor.MoveToNext())
                {
                    var reader = new SqlReader(cursor);
                    var dto = new TDto();
					if (map.Map(reader, dto))
                    {
						if(dataBound != null) dataBound (dto);
                        //Logger.Debug("Inserindo na lista {0} - Codigo: {1} ID: {2}", dal.TableName, dto.Codigo, dto.Id);
                        list.Add(dto);
                    }
                }
                return list;
            }
        }

		public TObject GetScalar<TObject>(string scalarField, IDictionary<string, object> where)
		{
			var query = FluentQuery.Create ()
				.AddSelect (scalarField)
				.AddFrom (map.TableName);
			foreach (var keyPair in where) {
				query.AddWhere (keyPair.Key + " = {0}", FluentParameter.Create ()
					.Add (keyPair.Key, keyPair.Value)
					.Mapper ());
			}
			var o = GetScalar<int>(query.CommandText, query.Parameters);
			return o > 0;
		}

		private TObject GetScalar<TObject>(string query, IDictionary<string, object> where)
        {
            var db = Helper.ReadableDatabase;
            string[] parametersOrdained = GetParametersInOrder(query, where);
            using (var cursor = db.RawQuery(PrepareQuery(query, where != null ? where.Keys : null), parametersOrdained))
            {
                if (cursor.MoveToNext())
                {
                    var reader = new SqlReader(cursor);
                    return reader.GetValueOrDefault<TObject>(0);
                }
                return default(TObject);
            }
        }

        #endregion

        public long Insert(TDto dto)
        {
            var db = Helper.WritableDatabase;
            var content = new ContentValues();
			dto.Id = GetNextPkValue ();
			var mapInsert = map.GetInsertMap (dto);
            foreach (var o in mapInsert)
            {
                //Logger.Debug("parametro - Tablename: {0} Key: {1} Value: {2}", tableName, o.Key.Replace(SqlReader.PREFIX_PARAM, String.Empty), AndroidSqlReader.GetTextValue(o.Value));
                content.Put(o.Key.Replace(SqlReader.PREFIX_PARAM, String.Empty), SqlReader.GetTextValue(o.Value));
            }
            return db.Insert(map.TableName, null, content);
        }

		public long Update(TDto dto)
		{
			var mapUpdate = map.GetUpdateMap (dto);

			var where = map.GetKeyMap();
			FluentQuery condition = FluentQuery.Create();
			foreach (var keyPair in where) {
				condition.AddWhere (keyPair.Key + " = {0}", FluentParameter.Create ()
					.Add (keyPair.Key, keyPair.Value)
					.Mapper ());
			}
			//condition.Add (SqlReader.GetParamText (map.PrimaryKey), dto.Id);
			//var where = String.Format("{0} = {1}", map.PrimaryKey, SqlReader.GetParamText(map.PrimaryKey));
			return Update (mapUpdate, where, condition.CommandText);
		}

		private long Update(IDictionary<string, object> mapUpdate, string condition, IDictionary<string, object> where)
        {
            var db = Helper.WritableDatabase;
            var content = new ContentValues();
            string[] parametersOrdained = where != null && where.Count > 0 ? GetParametersInOrder(condition, where) : null;
            string queryPrepared = where != null && where.Count > 0 ? PrepareQuery(condition, where.Keys) : condition;
            foreach (var o in mapUpdate)
            {
                content.Put(o.Key.Replace(SqlReader.PREFIX_PARAM, String.Empty), SqlReader.GetTextValue(o.Value));
            }
			return db.Update(map.TableName, content, queryPrepared, parametersOrdained);
        }

        public long Delete(int id)
        {
            var db = Helper.WritableDatabase;
            return db.Delete(map.TableName, String.Format("{0} = ?", map.PrimaryKey), new[] {id.ToString()});
        }

		public void ExecuteNonQuery(string sql)
		{
			Helper.WritableDatabase.ExecSQL (sql);
		}

        #endregion

        private string PrepareQuery(string query, IEnumerable<string> parametersKey)
        {
            if (parametersKey != null)
            {
                string result = parametersKey.Aggregate(query, (current, key) => current.Replace(key, "?"));
                return result;
            }
            return query;
        }

        private string[] GetParametersInOrder(string query, IDictionary<string, object> parameters)
        {
            var ordered = new List<string>();
            int nextStartIndex = -1;
            while (true)
            {
                nextStartIndex = query.IndexOf(SqlReader.PREFIX_PARAM, nextStartIndex + 1);
                if(nextStartIndex < 0)
                {
                    break;
                }
                int finishIndex = query.IndexOf(" ", nextStartIndex + 1);
                if(finishIndex < nextStartIndex)
                {
                    finishIndex = query.Length;
                }
                string parameterKey = query.Substring(nextStartIndex, finishIndex - nextStartIndex);
                ordered.Add(SqlReader.GetTextValue(parameters[parameterKey]));
            }
            return ordered.ToArray();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}
