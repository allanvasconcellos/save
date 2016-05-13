using System;
using System.Collections.Generic;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Rota")]
    public class RotaDto : Dto<RotaDto>
    {
        public DateTime Dia { get; set; }

        /// <summary>
        /// Roteiro do dia.
        /// </summary>
		[Ignore]
        public IEnumerable<ClienteDto> Clientes { get; set; }

		public int UsuarioId { get; set; }
		[Ignore]
        public UsuarioDto Usuario { get; set; }

        public string Nome { get; set; }

        public int IndicePasta { get; set; }

        public DateTime DiaPasta { get; set; }

        public BlocoStatusEnum Bloco { get; set; }
    }
}