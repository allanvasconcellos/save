using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("ProdutoSaldo")]
	public class ProdutoSaldoDto : Dto<ProdutoSaldoDto>
	{
		//[ForeignKey(typeof(ProdutoDto))]
		public int ProdutoId { get; set; }

		public int UsuarioId { get; set; }
		public decimal Saldo { get; set; }
	}
}

