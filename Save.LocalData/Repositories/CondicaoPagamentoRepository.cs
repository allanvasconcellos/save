using System;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;
using System.Collections.Generic;

namespace Save.LocalData.Repositories
{
	public class CondicaoPagamentoRepository : BaseRepository<CondicaoPagamentoDto>, IOfflineCondicaoPagamentoDb
	{
		public CondicaoPagamentoRepository (IData<CondicaoPagamentoDto> data)
			: base(data)
		{
		}

		#region implemented abstract members of BaseRepository

		protected override void Map (CondicaoPagamentoDto dto)
		{
		}

		#endregion
	}
}

