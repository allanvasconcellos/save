using System;
using INetSales.Objects.Dtos;
using System.Collections.Generic;

namespace INetSales.OfflineInterface.AndroidDb.Maps
{
	public class CondicaoPagamentoMap : DtoMap<CondicaoPagamentoDto>
	{
		public const string ID_COLUMN = "CondicaoId";
		public const string DESCRICAO_COLUMN = "Descricao";
		public const string ISDEFAULT_COLUMN = "IsDefault";
		public const string ISBOLETO_COLUMN = "IsBoleto";
		public const string ISCHEQUE_COLUMN = "IsCheque";

		public CondicaoPagamentoMap()
			: base(ID_COLUMN, DtoMap.CODIGO_COLUMN, DtoMap.DATA_CRIACAO_COLUMN, DtoMap.DATA_ALTERACAO_COLUMN,
				DESCRICAO_COLUMN, ISDEFAULT_COLUMN, ISBOLETO_COLUMN, ISCHEQUE_COLUMN)
		{
			TableName = "TCondicaoPagamento";
			PrimaryKey = ID_COLUMN;
		}

		protected override IDictionary<string, object> GetInsertMap(CondicaoPagamentoDto dto, out int id)
		{
			var map = FluentParameter.Open(GetCommonInsertMap(dto), ID_COLUMN)
				.Add(DESCRICAO_COLUMN, dto.Descricao)
				.Add(ISDEFAULT_COLUMN, dto.IsDefault)
				.Add(ISBOLETO_COLUMN, dto.IsBoleto)
				.Add(ISCHEQUE_COLUMN, dto.IsCheque);
			id = map.GetId();
			return map;
		}

		protected override IDictionary<string, object> GetUpdateMap(CondicaoPagamentoDto dto)
		{
			var mapUpdate = GetCommonUpdateMap(dto);
			mapUpdate.Add(DESCRICAO_COLUMN, dto.Descricao);
			mapUpdate.Add(ISDEFAULT_COLUMN, dto.IsDefault);
			mapUpdate.Add(ISBOLETO_COLUMN, dto.IsBoleto);
			mapUpdate.Add(ISCHEQUE_COLUMN, dto.IsCheque);
			return mapUpdate;
		}

		protected override bool DoEspecificMap(SqlReader reader, CondicaoPagamentoDto dto)
		{
			dto.Descricao = reader.GetString(reader.GetColumnIndex(DESCRICAO_COLUMN));
			dto.IsDefault = reader.GetBool(reader.GetColumnIndex(ISDEFAULT_COLUMN));
			dto.IsBoleto = reader.GetBool(reader.GetColumnIndex(ISBOLETO_COLUMN));
			dto.IsCheque = reader.GetBool(reader.GetColumnIndex(ISCHEQUE_COLUMN));
			return true;
		}
	}
}

