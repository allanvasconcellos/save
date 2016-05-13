using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects;
using Mono.Data.Sqlite;
using System.Data;

namespace INetSales.OfflineInterface
{
    public partial class BaseDal2
    {
        protected TValue GetValueOrNull<TValue>(SqliteDataReader reader, int index)
        {
            if (!reader.IsDBNull(index))
            {
                return (TValue)Convert.ChangeType(reader.GetValue(index), typeof(TValue));
            }
            return default(TValue);
        }

        protected TDto GetObject<TDto>(string query, IEnumerable<SqliteParameter> parameters, Func<SqliteDataReader, TDto> map)
            where TDto : IDto, new()
        {
            var conn = GetConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.AddRange(parameters.ToArray());
                var readerDb = command.ExecuteReader();
                if (readerDb.Read())
                {
                    return map(readerDb);
                }
            }
            return default(TDto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="map"></param>
        /// <param name="bindingDataBound">dto: objeto atual da interação - reader: reader utilizado para obter as informações - lista: lista processada até o momento.</param>
        /// <param name="finalize">Action com a lista finalizada</param>
        /// <returns></returns>
        protected List<TDto> GetList<TDto>(string query, IEnumerable<SqliteParameter> parameters, Func<SqliteDataReader, TDto> map,
                                     Func<TDto, SqliteDataReader, IEnumerable<TDto>, bool> bindingDataBound = null, Action<IEnumerable<TDto>> finalize = null)
            where TDto : IDto, new()
        {
            var list = new List<TDto>();
            var conn = GetConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = query;
                if (parameters != null && parameters.Count() > 0)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                var readerDb = command.ExecuteReader(CommandBehavior.SingleResult);
                while (readerDb.Read())
                {
                    var dto = map(readerDb);
                    if (bindingDataBound == null || bindingDataBound(dto, readerDb, list))
                    {
                        list.Add(dto);
                    }
                }
                if (finalize != null)
                {
                    finalize(list);
                }
            }
            return list;
        }

        protected TObject GetScalar<TObject>(SqliteConnection conn, string query, IEnumerable<SqliteParameter> parameters)
        {
            using (var command = conn.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.AddRange(parameters.ToArray());
                var obj = command.ExecuteScalar();
                if (obj != null)
                {
                    return !Convert.IsDBNull(obj) ? (TObject)Convert.ChangeType(obj, typeof(TObject))
                                                : default(TObject);
                }
                return default(TObject);
            }
        }
    }
}
