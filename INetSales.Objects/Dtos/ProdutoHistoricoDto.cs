using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("ProdutoHistorico")]
	public class ProdutoHistoricoDto : Dto<ProdutoHistoricoDto>
	{
		public int ProdutoId { get; set; }
		public int UsuarioId { get; set; }
		public decimal QuantidadeAntiga { get; set; }
		public decimal QuantidadeNova { get; set; }
		public double Valor { get; set; }
		public string Motivo { get; set; }
	}
}

