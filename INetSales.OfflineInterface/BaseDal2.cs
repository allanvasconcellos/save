using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using Android.App;
using INetSales.Objects;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface
{
    public abstract partial class BaseDal2
    {
        private readonly SqliteConnection _conn;
        protected DatabaseHelper Helper { get; private set; }

        protected const string FIELD_CODIGO = "Codigo";
        protected const string FIELD_DATA_CRIACAO = "DataCriacao";
        protected const string FIELD_DATA_ALTERACAO = "DataAlteracao";
        protected const string FIELD_IS_DESABILITADO = "IsDesabilitado";
        protected const string FIELD_IS_PENDING_UPLOAD = "IsPendingUpload";
        protected const string FIELD_DATA_LAST_UPLOAD = "DataLastUpload";
        protected const string PARAM_QUERY_ID = "@ID";
        protected const string PARAM_QUERY_CODIGO = "@" + FIELD_CODIGO;
        protected const string PARAM_QUERY_DATA_CRIACAO = "@" + FIELD_DATA_CRIACAO;
        protected const string PARAM_QUERY_DATA_ALTERACAO = "@" + FIELD_DATA_ALTERACAO;
        protected const string PARAM_QUERY_IS_DESABILITADO = "@" + FIELD_IS_DESABILITADO;
        protected const string PARAM_QUERY_IS_PENDING_UPLOAD = "@" + FIELD_IS_PENDING_UPLOAD;
        protected const string PARAM_QUERY_DATA_LAST_UPLOAD = "@" + FIELD_DATA_LAST_UPLOAD;

        protected BaseDal2(SqliteConnection conn)
        {
           _conn = conn;
        }

        protected SqliteConnection Connection
        {
            get { return _conn; }
        }

        protected TDto Find<TDto>(int id, string primaryKey, string queryField, string tableName, Func<SqliteDataReader, TDto> map)
            where TDto : IDto, new()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", queryField);
            query.AppendFormat("FROM {0} ", tableName);
            query.AppendFormat("WHERE {0} = @ID ", primaryKey);

            var parameters = new[]
                                 {
                                     new SqliteParameter("@ID", id),
                                 };
            return GetObject(query.ToString(), parameters, map);
        }

        protected TDto FindByCodigo<TDto>(string codigo, string queryField, string tableName, Func<SqliteDataReader, TDto> map)
            where TDto : IDto, new()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", queryField);
            query.AppendFormat("FROM {0} ", tableName);
            query.Append("WHERE Codigo = @CODIGO");

            var parameters = new[]
                                 {
                                     new SqliteParameter("@CODIGO", codigo),
                                 };
            return GetObject(query.ToString(), parameters, map);
        }

        protected int GetNextPkValue(string primaryKey, string tableName)
        {
            var conn = _conn;
            var query = new StringBuilder();
            query.AppendFormat("SELECT Max({0}) ", primaryKey);
            query.AppendFormat("FROM {0} ", tableName);

            var currentValue = GetScalar<int>(conn, query.ToString(), new SqliteParameter[] { });
            return currentValue + 1;
        }

        protected SqliteConnection GetConnection()
        {
            return ConnectionHelper.GetSingleConnection();
        }

        protected bool ExecuteNonQuery(string commandText, IEnumerable<SqliteParameter> parameters)
        {
            var conn = _conn;
            using (var command = conn.CreateCommand())
            {
                command.CommandText = commandText;
                //var tr = ConnectionHelper.GetNonFakeTransaction();
                //if(tr is AndroidTransaction)
                //{
                //    command.Transaction = ((AndroidTransaction) tr).Transaction;
                //}
                command.Parameters.AddRange(parameters.ToArray());
                return command.ExecuteNonQuery() > 0;
            }
        }

        protected bool Exist(string query, IEnumerable<SqliteParameter> parameters)
        {
            var conn = _conn;
            using (var command = conn.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.AddRange(parameters.ToArray());
                var reader = command.ExecuteReader();
                return reader.HasRows;
            }
        }
    }
}