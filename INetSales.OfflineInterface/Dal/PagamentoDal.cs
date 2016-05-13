using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface.Dal
{
    public class PagamentoDal : BaseDal<PagamentoDto>
    {
        public PagamentoDal(SqliteConnection conn)
            : base(conn)
        {
            QueryField = "{0}PedidoId, {0}CondicaoPagamentoId, {0}Valor ";
            TableName = "TPedidoPagamento {0}";
            PrimaryKey = "PagamentoId";
            FinalizeQueryField(FIELD_CODIGO, FIELD_DATA_CRIACAO, FIELD_DATA_ALTERACAO, FIELD_IS_DESABILITADO,
                               FIELD_IS_PENDING_UPLOAD, FIELD_DATA_LAST_UPLOAD);
        }

        public IEnumerable<PagamentoDto> GetPagamentos(PedidoDto pedido)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE PedidoId = @PEDIDO ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@PEDIDO", pedido.Id),
                                 };
            return GetList(query.ToString(), parameters);
        }

        #region Overrides of BaseDal<PagamentoDto>

        protected override void DoMapDto(SqliteDataReader reader, PagamentoDto dto, int nextIndex)
        {
            var condicaoDal = new CondicaoPagamentoDal(Connection);
            dto.PedidoId = reader.GetInt32(nextIndex++);
            dto.Condicao = condicaoDal.Find(reader.GetInt32(nextIndex++));
            dto.ValorFinal = reader.GetDouble(nextIndex++);
        }

        protected override void Insert(PagamentoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
            commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
            commandText.Append("VALUES ");
            commandText.Append("(" + PrefixInsertValues + ", @PEDIDO, @CONDICAO, @VALOR) ");

            int newPagamentoId = GetNextPkValue(PrimaryKey, GetTableName(String.Empty));

            SqliteParameter[] parameters = GetParameters(dto, newPagamentoId,
                                                         new SqliteParameter("@PEDIDO", dto.PedidoId),
                                                         new SqliteParameter("@CONDICAO", dto.Condicao.Id),
                                                         new SqliteParameter("@VALOR", dto.ValorFinal)
                );

            if (ExecuteNonQuery(commandText.ToString(), parameters))
            {
                dto.Id = newPagamentoId;

                if (dto is PagamentoChequeDto)
                {
                    var chequeDto = (PagamentoChequeDto) dto;
                    InserirChequeInfo(chequeDto);
                }
            }

            // TODO: Verificar se o pagamento é cheque, e exige que seja inserido na tabela de info.
        }

        private void InserirChequeInfo(PagamentoChequeDto chequeInfo)
        {
            var commandText = new StringBuilder();
            commandText.Append("INSERT INTO TPedidoPagamentoChequeInfo ");
            commandText.Append("(PagamentoId, Numero, Agencia, Banco) ");
            commandText.Append("VALUES ");
            commandText.Append("(@PAGAMENTO, @NUMERO, @AGENCIA, @BANCO)");

            var parameters = new[]
                                 {
                                     new SqliteParameter("@PAGAMENTO", chequeInfo.Id),
                                     new SqliteParameter("@NUMERO", chequeInfo.Numero),
                                     new SqliteParameter("@AGENCIA", chequeInfo.Agencia),
                                     new SqliteParameter("@BANCO", chequeInfo.Banco)
                                 };

            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        protected override void Update(PagamentoDto dto)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}