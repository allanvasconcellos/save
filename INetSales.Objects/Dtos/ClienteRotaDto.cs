using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("ClienteRota")]
	public class ClienteRotaDto : Dto<ClienteRotaDto>
	{
		public int RotaId { get; set; }

		[Ignore]
		public RotaDto Rota { get; set; }

		public int ClienteId { get; set; }

		public int OrdemRoteiro { get; set; }

		public bool IsAtivoRoteiro { get; set; }

		public bool HasPedidoRoteiro { get; set; }

		public bool IsPermitidoForaDia { get; set; }
	}
}

