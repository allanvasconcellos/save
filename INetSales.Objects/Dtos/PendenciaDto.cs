using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("ClientePendencia")]
	public class PendenciaDto : Dto<PendenciaDto>
	{
		[Ignore]
        public ClienteDto Cliente { get; set;}

        public int ClienteId { get; set;}

		public string Documento {get; set;}

		public double ValorTotal { get; set; }

		public double ValorEmAberto { get; set; }

		public DateTime? DataEmissao { get; set; }

		public DateTime? DataVencimento { get; set; }

		public string LinkPagamento { get; set; }
	}
}

