using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("ProdutoPedido")]
	public class ProdutoPedidoDto : Dto<ProdutoPedidoDto>
	{
		public int PedidoId { get; set; }
		public int ProdutoId { get; set; }
		public double ValorUnitarioVendido { get; set; }
		public double Desconto { get; set; }
		public decimal QuantidadePedido { get; set; }
	}
}

