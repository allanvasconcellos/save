
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using INetSales.Objects;
using Mono.Data.Sqlite;
using INetSales.Objects.Dtos;

namespace INetSales.OfflineInterface.Dal
{
	public class PendenciaDal : BaseDal<PendenciaDto>
	{
		public PendenciaDal(SqliteConnection conn)
			: base(conn)
		{
			QueryField =
                "{0}ClienteId, {0}Documento, {0}ValorTotal, {0}ValorEmAberto, {0}DataEmissao, {0}DataVencimento, {0}LinkPagamento";
			TableName = "TPendencia {0}";
			PrimaryKey = "PendenciaId";
            FinalizeQueryField(FIELD_IS_DESABILITADO);
		}

		#region implemented abstract members of BaseDal
		protected override void Insert (PendenciaDto dto)
		{
			var commandText = new StringBuilder();
			commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
			commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
			commandText.Append("VALUES ");
			commandText.Append("(" + PrefixInsertValues +
				", @CLIENTE, @DOCUMENTO, @VALORTOTAL, @VALOREMABERTO, @DATAEMISSAO, @DATAVENCIMENTO, @LINKPAGAMENTO) ");

			int newPendenciaId = GetNextPkValue(PrimaryKey, GetTableName(String.Empty));

			SqliteParameter[] parameters = GetParameters(dto, newPendenciaId,
                new SqliteParameter("@CLIENTE", dto.ClienteId),
				new SqliteParameter("@DOCUMENTO", dto.Documento),
				new SqliteParameter("@VALORTOTAL", dto.ValorTotal),
				new SqliteParameter("@VALOREMABERTO", dto.ValorEmAberto),
				new SqliteParameter("@DATAEMISSAO", dto.DataEmissao),
				new SqliteParameter("@DATAVENCIMENTO", dto.DataVencimento),
				new SqliteParameter("@LINKPAGAMENTO", dto.LinkPagamento)
			);

			if (ExecuteNonQuery(commandText.ToString(), parameters))
			{
				dto.Id = newPendenciaId;
			}
		}
		protected override void Update (PendenciaDto dto)
		{
			var commandText = new StringBuilder();
			commandText.AppendFormat("UPDATE {0} SET ", GetTableName(String.Empty));
			commandText.Append(PrefixUpdateValues);
			commandText.Append(",Documento = @DOCUMENTO");
			commandText.Append(",ValorTotal = @VALORTOTAL");
			commandText.Append(",ValorEmAberto = @VALOREMABERTO");
			commandText.Append(",DataEmissao = @DATAEMISSAO");
			commandText.Append(",DataVencimento = @DATAVENCIMENTO");
			commandText.Append(",LinkPagamento = @LINKPAGAMENTO");
			commandText.AppendFormat(" WHERE {0} = @ID", PrimaryKey);

			SqliteParameter[] parameters = GetParameters(dto, dto.Id,
				new SqliteParameter("@DOCUMENTO", dto.Documento),
				new SqliteParameter("@VALORTOTAL", dto.ValorTotal),
				new SqliteParameter("@VALOREMABERTO", dto.ValorEmAberto),
				new SqliteParameter("@DATAEMISSAO", dto.DataEmissao),
				new SqliteParameter("@DATAVENCIMENTO", dto.DataVencimento),
				new SqliteParameter("@LINKPAGAMENTO", dto.LinkPagamento)
			);

			ExecuteNonQuery(commandText.ToString(), parameters);
		}
		protected override void DoMapDto (Mono.Data.Sqlite.SqliteDataReader reader, PendenciaDto dto, int nextIndex)
		{
            ClienteDal clienteDal = new ClienteDal(this.Connection);
            int clienteIndex = reader.GetOrdinal("ClienteId");
			int documentoIndex = reader.GetOrdinal("Documento");
			int valorTotalIndex = reader.GetOrdinal("ValorTotal");
			int valorEmAbertoIndex = reader.GetOrdinal("ValorEmAberto");
			int dataEmissaoIndex = reader.GetOrdinal("DataEmissao");
			int dataVencimentoIndex = reader.GetOrdinal("DataVencimento");
			int linkPagamentoIndex = reader.GetOrdinal("LinkPagamento");
            dto.ClienteId = GetValueOrNull<int>(reader, clienteIndex);
			dto.Documento = reader.GetString(documentoIndex);
			dto.ValorTotal = GetValueOrNull<double>(reader, valorTotalIndex);
			dto.ValorEmAberto = GetValueOrNull<double>(reader, valorEmAbertoIndex);
			dto.DataEmissao = GetValueOrNull<DateTime>(reader, dataEmissaoIndex);
			if (dto.DataEmissao.Value.Equals (default(DateTime))) {
				dto.DataEmissao = null;
			}
			dto.DataVencimento = GetValueOrNull<DateTime>(reader, dataVencimentoIndex);
			if (dto.DataVencimento.Value.Equals (default(DateTime))) {
				dto.DataVencimento = null;
			}
			dto.LinkPagamento = GetValueOrNull<string>(reader, linkPagamentoIndex);
		}
		#endregion

        public List<PendenciaDto> GetPendencias(ClienteDto cliente)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE ClienteId = @CLIENTE ");
            var parameters = new[]
                {
                    new SqliteParameter("@CLIENTE", cliente.Id),
                };
            return GetList(query.ToString(), parameters);
        }
	}
}

