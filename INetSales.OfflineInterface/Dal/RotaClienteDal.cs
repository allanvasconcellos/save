using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects;
using INetSales.Objects.Dtos;

namespace INetSales.OfflineInterface.Dal
{
	internal class RotaClienteDal : DtoDal
	{
		public const string CLIENTE_COLUMN = "ClienteId";
		public const string ROTA_COLUMN = "RotaId";
		public const string ORDEM_COLUMN = "Ordem";
		public const string ISATIVOROTEIRO_COLUMN = "IsAtivoRoteiro";
		public const string ISPERMITIDOFORADIA_COLUMN = "IsPermitidoForaDia";
		public const string HASPEDIDO_COLUMN = "HasPedido";

		public RotaClienteDal(IDbContext context) : base(
			context,
			DATA_CRIACAO_COLUMN, CLIENTE_COLUMN, ROTA_COLUMN, ORDEM_COLUMN, ISATIVOROTEIRO_COLUMN, ISPERMITIDOFORADIA_COLUMN, HASPEDIDO_COLUMN)
		{
			TableName = "TRotaCliente";
			PrimaryKey = CLIENTE_COLUMN + "," + ROTA_COLUMN;
		}

		#region Overrides of DtoDal<ClienteDto>

		public void IndicarPedidoCliente(RotaDto rota, ClienteDto cliente)
		{
			var mapUpdate = new Dictionary<string, object>();
			var condition = new Dictionary<string, object>();
			mapUpdate.Add(SqlReader.GetParamText(HASPEDIDO_COLUMN), true);
			condition.Add(SqlReader.GetParamText(ROTA_COLUMN), rota.Id);
			condition.Add(SqlReader.GetParamText(CLIENTE_COLUMN), cliente.Id);
			Context.Update(this, mapUpdate, String.Format("{0} = {1} AND {2} = {3}", 
				CLIENTE_COLUMN, SqlReader.GetParamText(CLIENTE_COLUMN),
				ROTA_COLUMN, SqlReader.GetParamText(ROTA_COLUMN)), condition);
		}

		public void InserirClienteRota(ClienteDto cliente, RotaDto rota)
		{
			var mapInsert = new Dictionary<string, object>();
			mapInsert.Add(SqlReader.GetParamText(CLIENTE_COLUMN), cliente.Id);
			mapInsert.Add(SqlReader.GetParamText(ROTA_COLUMN), rota.Id);
			mapInsert.Add(SqlReader.GetParamText(ORDEM_COLUMN), cliente.OrdemRoteiro);
			mapInsert.Add(SqlReader.GetParamText(ISATIVOROTEIRO_COLUMN), cliente.IsAtivoRoteiro);
			mapInsert.Add(SqlReader.GetParamText(ISPERMITIDOFORADIA_COLUMN), cliente.IsPermitidoForaDia);
			mapInsert.Add(SqlReader.GetParamText(DATA_CRIACAO_COLUMN), DateTime.Now);
			if(Context.Insert(this, mapInsert) > 0)
			{
				Logger.Info(true, false, "Inserido o cliente (Codigo: {0} - Ordem Roteiro: {1} - Permitido fora do dia: {2}) na rota (Dia: {3} - Pasta: {4})",
					cliente.Codigo, cliente.OrdemRoteiro, cliente.IsPermitidoForaDia, rota.Dia, rota.IndicePasta);
			}
		}

		public void AtualizarClienteRota(ClienteDto cliente, RotaDto rota)
		{
			var mapUpdate = new Dictionary<string, object>();
			var condition = new Dictionary<string, object>();
			mapUpdate.Add(SqlReader.GetParamText(ORDEM_COLUMN), cliente.OrdemRoteiro);
			mapUpdate.Add(SqlReader.GetParamText(ISATIVOROTEIRO_COLUMN), cliente.IsAtivoRoteiro);
			mapUpdate.Add(SqlReader.GetParamText(ISPERMITIDOFORADIA_COLUMN), cliente.IsPermitidoForaDia);
			mapUpdate.Add(SqlReader.GetParamText(HASPEDIDO_COLUMN), true);
			condition.Add(SqlReader.GetParamText(ROTA_COLUMN), rota.Id);
			condition.Add(SqlReader.GetParamText(CLIENTE_COLUMN), cliente.Id);

			if(Context.Update(this, mapUpdate, String.Format("{0} = {1} AND {2} = {3}",
				CLIENTE_COLUMN, SqlReader.GetParamText(CLIENTE_COLUMN),
				ROTA_COLUMN, SqlReader.GetParamText(ROTA_COLUMN)), condition) > 0)
			{
				Logger.Info(true, false, "Atualizado o cliente (Codigo: {0} - Ordem Roteiro: {1} - Permitido fora do dia: {2}) na rota (Dia: {3} - Pasta: {4})", 
					cliente.Codigo, cliente.OrdemRoteiro, cliente.IsPermitidoForaDia, rota.Dia, rota.IndicePasta);
			}
		}

		public bool ExisteClienteRota(ClienteDto cliente, RotaDto rota)
		{
			var query = new StringBuilder();
			var parameters = new Dictionary<string, object>();
			query.Append("SELECT 1 ");
			query.AppendFormat("FROM {0} ", GetTableName());
			query.AppendFormat("WHERE {0} = {1} AND {2} = {3}",
				CLIENTE_COLUMN, SqlReader.GetParamText(CLIENTE_COLUMN),
				ROTA_COLUMN, SqlReader.GetParamText(ROTA_COLUMN));
			parameters.Add(SqlReader.GetParamText(CLIENTE_COLUMN), cliente.Id);
			parameters.Add(SqlReader.GetParamText(ROTA_COLUMN), rota.Id);
			return Context.Exist(query.ToString(), parameters);
		}

		public bool VerificarDesabilitadoNaRota(RotaDto rota, ClienteDto cliente)
		{
			var query = new StringBuilder();
			var parameters = new Dictionary<string, object>();
			query.Append("SELECT 1 ");
			query.AppendFormat("FROM {0} ", GetTableName());
			query.AppendFormat("WHERE {0} = {1} And {2} = {3} And {4} = {5} ",
				ISATIVOROTEIRO_COLUMN, SqlReader.GetParamText(ISATIVOROTEIRO_COLUMN),
				CLIENTE_COLUMN, SqlReader.GetParamText(CLIENTE_COLUMN),
				ROTA_COLUMN, SqlReader.GetParamText(ROTA_COLUMN));
			parameters.Add(SqlReader.GetParamText(ISATIVOROTEIRO_COLUMN), false);
			parameters.Add(SqlReader.GetParamText(CLIENTE_COLUMN), cliente.Id);
			parameters.Add(SqlReader.GetParamText(ROTA_COLUMN), rota.Id);
			return Context.Exist(query.ToString(), parameters);
		}

		public bool ExisteInfoClienteNaRota(RotaDto rota, ClienteDto cliente)
		{
			var query = FluentQuery.Create();
			query.AddSelect("1");
			query.AddFrom(GetTableName());
			query.AddWhere("ClienteId = {0} And RotaId = {1} And Ordem = {2} And IsPermitidoForaDia = {3} ", FluentParameter.Create()
				.Add(CLIENTE_COLUMN, cliente.Id)
				.Add(ROTA_COLUMN, rota.Id)
				.Add(ORDEM_COLUMN, cliente.OrdemRoteiro)
				.Add(ISPERMITIDOFORADIA_COLUMN, cliente.IsPermitidoForaDia)
				.Mapper());
			return Context.Exist(query.CommandText, query.Parameters);
		}

		public void DesativarClienteNaRota(RotaDto rota, ClienteDto cliente)
		{
			var mapUpdate = new Dictionary<string, object>();
			var condition = new Dictionary<string, object>();
			mapUpdate.Add(SqlReader.GetParamText(ISATIVOROTEIRO_COLUMN), false);
			condition.Add(SqlReader.GetParamText(ROTA_COLUMN), rota.Id);
			condition.Add(SqlReader.GetParamText(CLIENTE_COLUMN), cliente.Id);
			if(Context.Update(this, mapUpdate, String.Format("{0} = {1} AND {2} = {3}",
				CLIENTE_COLUMN, SqlReader.GetParamText(CLIENTE_COLUMN),
				ROTA_COLUMN, SqlReader.GetParamText(ROTA_COLUMN)), condition) > 0)
			{
				Logger.Info(true, false, "Desativado o cliente (Codigo: {0} - Ordem Roteiro: {1} - Permitido fora do dia: {2}) na rota (Dia: {3} - Pasta: {4})",
					cliente.Codigo, cliente.OrdemRoteiro, cliente.IsPermitidoForaDia, rota.Dia, rota.IndicePasta);
			}
		}

		public bool MapRota(SqlReader reader, ClienteDto dto)
		{
			int ordemIndex = reader.GetColumnIndex(ORDEM_COLUMN);
			int isAtivoRoteiroIndex = reader.GetColumnIndex(ISATIVOROTEIRO_COLUMN);
			int isPermitidoForaDiaIndex = reader.GetColumnIndex(ISPERMITIDOFORADIA_COLUMN);
			int hasPedidoIndex = reader.GetColumnIndex(HASPEDIDO_COLUMN);

			dto.OrdemRoteiro = reader.GetInt(ordemIndex);
			dto.IsAtivoRoteiro = reader.GetBool(isAtivoRoteiroIndex);
			dto.IsPermitidoForaDia = reader.GetBool(isPermitidoForaDiaIndex);
			dto.HasPedidoRoteiro = reader.GetBool(hasPedidoIndex);
			return true;
		}

		#endregion
	}
}

