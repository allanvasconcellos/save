using System;
using INetSales.Objects.Dtos;
using System.Collections.Generic;

namespace Save.LocalData.Repositories
{
	public class PagamentoRepository : BaseRepository<PagamentoDto>
	{
		public PagamentoRepository (IData<PagamentoDto> data)
			: base(data)
		{
		}

		public IEnumerable<PagamentoDto> GetPagamentos(PedidoDto pedido)
		{
			return GetAll (p => p.PedidoId == pedido.Id);
		}

		private void InserirChequeInfo(PagamentoChequeDto chequeInfo)
		{
			var dataCheque = FactoryOffline.GetData<PagamentoChequeDto> ();
			dataCheque.Add (chequeInfo);
		}

		#region implemented abstract members of BaseRepository

		protected override void Map (PagamentoDto dto)
		{
			CondicaoPagamentoRepository condicaoRepository = new CondicaoPagamentoRepository (
				FactoryOffline.GetData<CondicaoPagamentoDto> ());
			dto.Condicao = condicaoRepository.Find (dto.CondicaoId);
		}

		protected override void PreInsert (PagamentoDto dto)
		{
			dto.CondicaoId = dto.Condicao.Id;
		}

		protected override void PosInsert (PagamentoDto dto)
		{
			if (dto is PagamentoChequeDto)
			{
				var chequeDto = (PagamentoChequeDto) dto;
				//InserirChequeInfo(chequeDto);
			}
		}

		protected override void PreUpdate (PagamentoDto dto)
		{
			dto.CondicaoId = dto.Condicao.Id;
		}

		#endregion
	}
}

