using System;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;
using System.Collections.Generic;
using System.Linq.Expressions;
using INetSales.Objects;
using System.Linq;

namespace Save.LocalData.Repositories
{
	public class PedidoRepository : BaseRepository<PedidoDto>, IOfflinePedidoDb, IUploadDb
	{
		public PedidoRepository (IData<PedidoDto> data)
			: base(data)
		{
		}

		public bool VerificarPedidoPendente ()
		{
			return data.Exist (p => p.IsPendingUpload == true);
		}

		public IEnumerable<PedidoDto> GetPedidosPorData (UsuarioDto usuario, DateTime dataInicio, DateTime dataFinal)
		{
			return GetAll (p => p.UsuarioId == usuario.Id && (p.DataCriacao >= dataInicio && p.DataCriacao <= dataFinal));
		}

		public decimal GetTotalValorPagoBoleto (ProdutoDto produtoDto)
		{
			var dataProdutoPedido = FactoryOffline.GetData<ProdutoPedidoDto> ();
			var produtosPedido = dataProdutoPedido.Find (p => p.ProdutoId == produtoDto.Id);
			decimal somatorioTotal = 0;
			foreach (var produtoPedido in produtosPedido) {
				var pagamentoRepository = new PagamentoRepository(FactoryOffline.GetData<PagamentoDto> ());
				var pagamentosProduto = pagamentoRepository.GetAll (p => p.PedidoId == produtoPedido.PedidoId);
				decimal somatorioPagamento = 0;
				foreach (var pagamento in pagamentosProduto) {
					if (pagamento.Condicao.IsBoleto) {
						somatorioPagamento += Convert.ToDecimal(pagamento.ValorFinal);
					}
				}
				somatorioTotal += (somatorioPagamento * produtoPedido.QuantidadePedido);
			}
			return somatorioTotal;
		}

		public decimal GetTotalValorPagoCheque (ProdutoDto produtoDto)
		{
			var dataProdutoPedido = FactoryOffline.GetData<ProdutoPedidoDto> ();
			var produtosPedido = dataProdutoPedido.Find (p => p.ProdutoId == produtoDto.Id);
			decimal somatorioTotal = 0;
			foreach (var produtoPedido in produtosPedido) {
				var pagamentoRepository = new PagamentoRepository(FactoryOffline.GetData<PagamentoDto> ());
				var pagamentosProduto = pagamentoRepository.GetAll (p => p.PedidoId == produtoPedido.PedidoId);
				decimal somatorioPagamento = 0;
				foreach (var pagamento in pagamentosProduto) {
					if (pagamento.Condicao.Codigo.Equals("430")) { // Código Cheque
						somatorioPagamento += Convert.ToDecimal(pagamento.ValorFinal);
					}
				}
				somatorioTotal += (somatorioPagamento * produtoPedido.QuantidadePedido);
			}
			return somatorioTotal;
		}

		public decimal GetTotalValorPagoDinheiro (ProdutoDto produtoDto)
		{
			var dataProdutoPedido = FactoryOffline.GetData<ProdutoPedidoDto> ();
			var produtosPedido = dataProdutoPedido.Find (p => p.ProdutoId == produtoDto.Id);
			decimal somatorioTotal = 0;
			foreach (var produtoPedido in produtosPedido) {
				var pagamentoRepository = new PagamentoRepository(FactoryOffline.GetData<PagamentoDto> ());
				var pagamentosProduto = pagamentoRepository.GetAll (p => p.PedidoId == produtoPedido.PedidoId);
				decimal somatorioPagamento = 0;
				foreach (var pagamento in pagamentosProduto) {
					if (pagamento.Condicao.Codigo.Equals("294")) { // Código Dinheiro
						somatorioPagamento += Convert.ToDecimal(pagamento.ValorFinal);
					}
				}
				somatorioTotal += (somatorioPagamento * produtoPedido.QuantidadePedido);
			}
			return somatorioTotal;
		}

		public void InserirProdutoPedido (PedidoDto pedido, ProdutoDto produto)
		{
			ProdutoPedidoDto produtoPedido = new ProdutoPedidoDto ();
			var dataProdutoPedido = FactoryOffline.GetData<ProdutoPedidoDto>();
			produtoPedido.PedidoId = pedido.Id;
			produtoPedido.ProdutoId = produto.Id;
			produtoPedido.ValorUnitarioVendido = produto.ValorUnitario;
			produtoPedido.Desconto = produto.Desconto;
			produtoPedido.QuantidadePedido = produto.QuantidadePedido;
			dataProdutoPedido.Add (produtoPedido);
		}

		public void InserirPagamentoPedido (PedidoDto pedido, PagamentoDto pagamento)
		{
			var repository = new PagamentoRepository (FactoryOffline.GetData<PagamentoDto> ());
			pagamento.PedidoId = pedido.Id;
			repository.Save (pagamento);
		}

		#region implemented abstract members of BaseRepository

		protected override void Map (PedidoDto dto)
		{
			var usuarioRepository = new UsuarioRepository(FactoryOffline.GetData<UsuarioDto>());
			var pagamentoRepository = new PagamentoRepository(FactoryOffline.GetData<PagamentoDto>());

			dto.Usuario = usuarioRepository.Find (dto.UsuarioId);
			dto.Pagamentos = pagamentoRepository.GetPagamentos (dto);
			if (dto.ClienteId > 0) {
				var clienteRepository = new ClienteRepository (FactoryOffline.GetData<ClienteDto> ());
				dto.Cliente = clienteRepository.Find (dto.ClienteId);
			}
			dto.HasCondicaoBoleto = dto.Pagamentos.Count() > 0 && dto.Pagamentos.Any(p => p.Condicao.IsBoleto);
		}

		protected override void PreInsert (PedidoDto dto)
		{
			if(dto.Cliente != null) dto.ClienteId = dto.Cliente.Id;
			dto.UsuarioId = dto.Usuario.Id;
			if (dto.Roteiro != null) {
				dto.RotaId = dto.Roteiro.Id;
			}
		}

		protected override void PreUpdate (PedidoDto dto)
		{
			if(dto.Cliente != null) dto.ClienteId = dto.Cliente.Id;
			dto.UsuarioId = dto.Usuario.Id;
			if (dto.Roteiro != null) {
				dto.RotaId = dto.Roteiro.Id;
			}
		}

		#endregion

		#region IUploadDb implementation

		public IEnumerable<IUploader> GetUploadersWithPendind ()
		{
			return GetAll (p => p.IsPendingUpload == true)
				.Cast<IUploader>();
		}

		#endregion
	}
}

