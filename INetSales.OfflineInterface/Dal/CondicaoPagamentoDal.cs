using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface.Dal
{
    public class CondicaoPagamentoDal : BaseDal<CondicaoPagamentoDto>, IOfflineCondicaoPagamentoDb
    {
        public CondicaoPagamentoDal(SqliteConnection conn)
            : base(conn)
        {
            QueryField = "{0}Descricao, {0}IsDefault, {0}IsCheque, {0}IsBoleto, {0}IsPublica ";
            TableName = "TCondicaoPagamento {0}";
            PrimaryKey = "CondicaoId";
            FinalizeQueryField(FIELD_IS_DESABILITADO, FIELD_IS_PENDING_UPLOAD, FIELD_DATA_LAST_UPLOAD);
        }

        #region Overrides of BaseDal<CondicaoPagamentoDto>

        protected override void DoMapDto(SqliteDataReader reader, CondicaoPagamentoDto dto, int nextIndex)
        {
            dto.Descricao = reader.GetString(nextIndex++);
            dto.IsDefault = reader.GetBoolean(nextIndex++);
            dto.IsCheque = reader.GetBoolean(nextIndex++);
            dto.IsBoleto = reader.GetBoolean(nextIndex++);
            dto.IsPublica = GetValueOrNull<bool>(reader, nextIndex++);
        }

        protected override void Insert(CondicaoPagamentoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
            commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
            commandText.Append("VALUES ");
            commandText.Append("(" + PrefixInsertValues + ", @DESCRICAO, @IS_DEFAULT, @IS_CHEQUE, @IS_BOLETO, @IS_PUBLICA) ");

            int newCondicaoId = GetNextPkValue(PrimaryKey, GetTableName(String.Empty));

            SqliteParameter[] parameters = GetParameters(dto, newCondicaoId,
                                                         new SqliteParameter("@DESCRICAO", dto.Descricao),
                                                         new SqliteParameter("@IS_DEFAULT", dto.IsDefault),
                                                         new SqliteParameter("@IS_CHEQUE", dto.IsCheque),
                                                         new SqliteParameter("@IS_BOLETO", dto.IsBoleto),
                                                         new SqliteParameter("@IS_PUBLICA", dto.IsPublica)
                );

            if (ExecuteNonQuery(commandText.ToString(), parameters))
            {
                dto.Id = newCondicaoId;
            }
        }

        protected override void Update(CondicaoPagamentoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("UPDATE {0} SET ", GetTableName(String.Empty));
            commandText.Append(PrefixUpdateValues);
            commandText.Append(",Descricao = @DESCRICAO");
            commandText.Append(",IsDefault = @IS_DEFAULT");
            commandText.Append(",IsCheque = @IS_CHEQUE");
            commandText.Append(",IsBoleto = @IS_BOLETO");
            commandText.Append(",IsPublica = @IS_PUBLICA");
            commandText.AppendFormat(" WHERE {0} = @ID", PrimaryKey);

            SqliteParameter[] parameters = GetParameters(dto, dto.Id,
                                                         new SqliteParameter("@DESCRICAO", dto.Descricao),
                                                         new SqliteParameter("@IS_DEFAULT", dto.IsDefault),
                                                         new SqliteParameter("@IS_CHEQUE", dto.IsCheque),
                                                         new SqliteParameter("@IS_BOLETO", dto.IsBoleto),
                                                         new SqliteParameter("@IS_PUBLICA", dto.IsPublica)
                );

            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        #endregion

        #region Implementation of IDb<CondicaoPagamentoDto>

        public IEnumerable<CondicaoPagamentoDto> GetAll(UsuarioDto usuario)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            return GetList(query.ToString());
        }

        public IEnumerable<CondicaoPagamentoDto> GetAll()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            return GetList(query.ToString());
        }

        #endregion
    }
}