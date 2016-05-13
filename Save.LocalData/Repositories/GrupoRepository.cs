using System;
using System.Linq;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;
using System.Collections.Generic;

namespace Save.LocalData.Repositories
{
	public class GrupoRepository : BaseRepository<GrupoDto>, IOfflineGrupoDb
	{
		public GrupoRepository (IData<GrupoDto> data)
			: base(data)
		{
		}

		public GrupoDto GetGrupoDefault ()
		{
			var d = data.First ();
			if (d != null) {
				Map (d);
			}
			return d;
		}

		public IEnumerable<GrupoDto> GetGruposEstocados (UsuarioDto usuario)
		{
			var produtoRepository = new ProdutoRepository (
				FactoryOffline.GetData<ProdutoDto>());
			return data.All (g => {
				g.Produtos = produtoRepository.GetProdutosEstocados(usuario, g);
				foreach (ProdutoDto produto in g.Produtos)
				{
					//produto.QuantidadeTotalPedido = pedidoDal.GetQuantidadeTotalPedido(produto,
					//	usuario);
				}
				Map(g);
				return g.Produtos.Count() > 0;
			});
		}

		#region implemented abstract members of BaseRepository

		protected override void Map (GrupoDto dto)
		{
			if (dto.GrupoPaiId > 0) {
				dto.GrupoPai = data.Get (dto.GrupoPaiId);
			}
		}

		protected override void PreInsert (GrupoDto dto)
		{
			if (dto.GrupoPai != null) {
				dto.GrupoPaiId = dto.GrupoPai.Id;
			}
		}

		protected override void PreUpdate (GrupoDto dto)
		{
			if (dto.GrupoPai != null) {
				dto.GrupoPaiId = dto.GrupoPai.Id;
			}
		}

		#endregion
	}
}

