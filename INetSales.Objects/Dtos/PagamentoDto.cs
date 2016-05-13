using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Pagamento")]
    public class PagamentoDto : Dto<PagamentoDto>
    {
        public int PedidoId { get; set; }

		public int CondicaoId { get; set; }
		[Ignore]
        public CondicaoPagamentoDto Condicao { get; set; }

        public double ValorFinal { get; set; }
    }

	[Table("PagamentoCheque")]
    public class PagamentoChequeDto : PagamentoDto
    {
        public Guid? Uid { get; set; }

        public string Numero { get; set; }

        public string Agencia { get; set; }

        public string Banco { get; set; }
    }
}