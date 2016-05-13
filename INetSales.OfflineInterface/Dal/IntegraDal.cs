using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface.Dal
{
    public class IntegraDal : BaseDal<IntegraDto>, IOfflineIntegraDb
    {
        public IntegraDal(SqliteConnection conn)
            : base(conn)
        {
            QueryField = "{0}DataInicio, {0}Intervalo, {0}DataUltimaIntegra";
            TableName = "TIntegra {0}";
            PrimaryKey = "IntegraId";
            FinalizeQueryField(FIELD_DATA_CRIACAO, FIELD_DATA_ALTERACAO, FIELD_IS_DESABILITADO);
        }

        protected override void DoMapDto(SqliteDataReader reader, IntegraDto dto, int nextIndex)
        {
            dto.DataInicio = reader.GetDateTime(nextIndex++);
            dto.Intervalo = TimeSpan.Parse(reader.GetString(nextIndex++));
            dto.DataUltimaIntegracao = reader.IsDBNull(nextIndex) ? null : (DateTime?) reader.GetDateTime(nextIndex);
        }

        protected override void Insert(IntegraDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
            commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
            commandText.Append("VALUES ");
            commandText.Append("(@ID, @DESCRICAO, @DATA_INICIO, @INTERVALO, @DATA_ULTIMA_INTEGRACAO) ");

            int newIntegraId = GetNextPkValue(PrimaryKey, GetTableName(String.Empty));

            var parameters = new[]
                                 {
                                     new SqliteParameter("@ID", newIntegraId),
                                     new SqliteParameter("@DESCRICAO", dto.Codigo),
                                     new SqliteParameter("@DATA_INICIO", dto.DataInicio),
                                     new SqliteParameter("@INTERVALO", dto.Intervalo.ToString()),
                                     new SqliteParameter("@DATA_ULTIMA_INTEGRACAO", dto.DataUltimaIntegracao),
                                 };

            if (ExecuteNonQuery(commandText.ToString(), parameters))
            {
                dto.Id = newIntegraId;
            }
        }

        protected override void Update(IntegraDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("UPDATE {0} SET ", GetTableName(String.Empty));
            commandText.Append("DataUltimaIntegra = @DATA_ULTIMA_INTEGRACAO ");
            commandText.Append("WHERE IntegraId = @ID");

            var parameters = new[]
                                 {
                                     new SqliteParameter("@ID", dto.Id),
                                     new SqliteParameter("@DATA_ULTIMA_INTEGRACAO", dto.DataUltimaIntegracao),
                                 };

            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        #region Implementation of IDb<IntegraDto>

        public IEnumerable<IntegraDto> GetAll(UsuarioDto usuario)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IntegraDto> GetAll()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}