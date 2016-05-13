using System;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;
using System.Collections.Generic;
using INetSales.Objects;

namespace Save.LocalData.Repositories
{
	public class ProdutoRepository : BaseRepository<ProdutoDto>, IOfflineProdutoDb
	{
		public ProdutoRepository (IData<ProdutoDto> data)
			: base(data)
		{
		}

		public override IEnumerable<ProdutoDto> GetAll (UsuarioDto usuario)
		{
			var dataSaldo = FactoryOffline.GetData<ProdutoSaldoDto> ();
			IEnumerable<ProdutoSaldoDto> saldos;
			if (usuario != null) {
				saldos = dataSaldo.Find (s => s.UsuarioId == usuario.Id);
			} else {
				saldos = dataSaldo.All ();
			}
			var produtos = new List<ProdutoDto> ();
			foreach(var saldo in saldos) {
				var produto = Find (saldo.ProdutoId);
				produto.QuantidadeDisponivel = saldo.Saldo;
				produto.SaldoAtual = saldo.Saldo;
				produtos.Add (produto);
			}
			return produtos;
		}

		public IEnumerable<ProdutoDto> GetAllProdutos ()
		{
			var all = data.All ();
			foreach (var d in all) {
				Map (d);
			}
			return all;
		}

		public IEnumerable<ProdutoDto> GetProdutosEstocados (UsuarioDto usuario, GrupoDto grupo = null)
		{
			var dataSaldo = FactoryOffline.GetData<ProdutoSaldoDto> ();
			var saldos = dataSaldo.Find (s => s.UsuarioId == usuario.Id && s.Saldo > 0);
			List<ProdutoDto> produtos = new List<ProdutoDto> ();
			foreach(var saldo in saldos) {
				var produto = Find (saldo.ProdutoId);
				if(grupo == null || produto.Grupo.Equals(grupo))
				{
					produto.QuantidadeDisponivel = saldo.Saldo;
					produto.SaldoAtual = saldo.Saldo;
					produtos.Add (produto);
				}
			}
			return produtos;
		}

		public IEnumerable<ProdutoDto> GetProdutos (PedidoDto pedido)
		{
			var dataPedido = FactoryOffline.GetData<ProdutoPedidoDto> ();
			var dataSaldo = FactoryOffline.GetData<ProdutoSaldoDto> ();
			var produtoPedidos = dataPedido.Find (p => p.PedidoId == pedido.Id);
			List<ProdutoDto> produtos = new List<ProdutoDto> ();
			foreach(var produtoPedido in produtoPedidos)
			{
				var produto = Find (produtoPedido.ProdutoId);
				var saldo = dataSaldo.Get (s => s.ProdutoId == produtoPedido.ProdutoId);
				produto.QuantidadePedido = produtoPedido.QuantidadePedido;
				produto.Desconto = produtoPedido.Desconto;
				if (saldo != null) {
					produto.QuantidadeDisponivel = saldo.Saldo;
					produto.SaldoAtual = saldo.Saldo;
				}
				produtos.Add (produto);
			}
			return produtos;
		}

		public bool InserirHistorico (ProdutoDto produto, UsuarioDto usuario, decimal quantidadeAntiga, decimal quantidadeNova, double valor, string motivo)
		{
			var dataHistorico = FactoryOffline.GetData<ProdutoHistoricoDto> ();
			var historico = new ProdutoHistoricoDto () {
				ProdutoId = produto.Id,
				UsuarioId = usuario.Id,
				QuantidadeAntiga = quantidadeAntiga,
				QuantidadeNova = quantidadeNova,
				Valor = valor,
				Motivo = motivo,
			};
			return dataHistorico.Add (historico) > 0;
		}

		public void AtualizarSaldo (ProdutoDto produto, UsuarioDto usuario, decimal saldoAtualizado)
		{
			var dataSaldo = FactoryOffline.GetData<ProdutoSaldoDto> ();
			var saldoItem = dataSaldo.Get (s => s.ProdutoId == produto.Id && s.UsuarioId == usuario.Id);
			if (saldoItem != null) { // Atualiza o saldo
				saldoItem.Saldo = saldoAtualizado;
				dataSaldo.Update (saldoItem);
			} else { // Inseri o novo saldo
				saldoItem = new ProdutoSaldoDto() { ProdutoId = produto.Id, UsuarioId = usuario.Id, Saldo = saldoAtualizado };
				dataSaldo.Add(saldoItem);
			}
		}

		#region implemented abstract members of BaseRepository

		protected override void Map (ProdutoDto dto)
		{
			var grupoRepository = new GrupoRepository(FactoryOffline.GetData<GrupoDto> ());
			dto.Grupo = grupoRepository.Find (dto.GrupoId);
		}

		protected override void PreInsert (ProdutoDto dto)
		{
			dto.GrupoId = dto.Grupo.Id;
		}

		protected override void PreUpdate (ProdutoDto dto)
		{
			dto.GrupoId = dto.Grupo.Id;
		}

		#endregion
	}
}

