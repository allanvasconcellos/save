using System;
using INetSales.Objects.Dtos;
using System.Collections.Generic;

namespace INetSales.OfflineInterface.AndroidDb.Maps
{
	public class UsuarioMap : DtoMap<UsuarioDto>
	{
		public const string ID_COLUMN = "UsuarioId";
		public const string NOME_COLUMN = "Nome";
		public const string USERNAME_COLUMN = "Username";
		public const string SENHA_HASH_COLUMN = "SenhaHash";
		public const string PLACA_COLUMN = "PlacaVeiculo";
		public const string IS_ADM_COLUMN = "IsAdm";
		public const string CODIGO_SECUNDARIO_COLUMN = "CodigoSecundario";
		public const string IS_SYNC_PENDING = "IsSyncPending";

		public UsuarioMap()
			: base(ID_COLUMN, DtoMap.CODIGO_COLUMN, DtoMap.DATA_CRIACAO_COLUMN, DtoMap.DATA_ALTERACAO_COLUMN, DtoMap.IS_DESABILITADO_COLUMN,
				NOME_COLUMN, USERNAME_COLUMN, SENHA_HASH_COLUMN, PLACA_COLUMN, IS_ADM_COLUMN, CODIGO_SECUNDARIO_COLUMN, IS_SYNC_PENDING)
		{
			TableName = "TUsuario";
			PrimaryKey = ID_COLUMN;
		}

		protected override IDictionary<string, object> GetInsertMap(UsuarioDto dto, out int id)
		{
			var map = FluentParameter.Open(GetCommonInsertMap(dto), ID_COLUMN)
				.Add(NOME_COLUMN, dto.Nome)
				.Add(USERNAME_COLUMN, dto.Username)
				.Add(SENHA_HASH_COLUMN, dto.SenhaHash)
				.Add(PLACA_COLUMN, dto.PlacaVeiculo)
				.Add(CODIGO_SECUNDARIO_COLUMN, dto.CodigoSecundario)
				.Add(IS_ADM_COLUMN, dto.IsAdm)
				.Add(IS_SYNC_PENDING, dto.IsSyncPending);
			id = map.GetId();
			return map;
		}

		protected override IDictionary<string, object> GetUpdateMap(UsuarioDto dto)
		{
			var mapUpdate = GetCommonUpdateMap(dto);
			mapUpdate.Add(NOME_COLUMN, dto.Nome);
			mapUpdate.Add(USERNAME_COLUMN, dto.Username);
			mapUpdate.Add(SENHA_HASH_COLUMN, dto.SenhaHash);
			mapUpdate.Add(PLACA_COLUMN, dto.PlacaVeiculo);
			mapUpdate.Add(IS_ADM_COLUMN, dto.IsAdm);
			mapUpdate.Add(CODIGO_SECUNDARIO_COLUMN, dto.CodigoSecundario);
			mapUpdate.Add(IS_SYNC_PENDING, dto.IsSyncPending);
			return mapUpdate;
		}

		protected override bool DoEspecificMap(SqlReader reader, UsuarioDto dto)
		{
			int nomeIndex = reader.GetColumnIndex(NOME_COLUMN);
			int usernameIndex = reader.GetColumnIndex(USERNAME_COLUMN);
			int SenhaHashIndex = reader.GetColumnIndex(SENHA_HASH_COLUMN);
			int placaIndex = reader.GetColumnIndex(PLACA_COLUMN);
			int isAdmIndex = reader.GetColumnIndex(IS_ADM_COLUMN);
			int codigoSecundarioIndex = reader.GetColumnIndex(CODIGO_SECUNDARIO_COLUMN);
			dto.Nome = reader.GetString(nomeIndex);
			dto.Username = reader.GetString(usernameIndex);
			dto.SenhaHash = reader.GetString(SenhaHashIndex);
			dto.IsAdm = reader.GetBool(isAdmIndex);
			dto.PlacaVeiculo = reader.GetString(placaIndex);
			dto.CodigoSecundario = reader.GetString(codigoSecundarioIndex);
			dto.IsSyncPending = reader.GetBool(reader.GetColumnIndex(IS_SYNC_PENDING));
			return true;
		}
	}
}

