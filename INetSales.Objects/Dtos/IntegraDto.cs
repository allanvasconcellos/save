using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Integra")]
    public class IntegraDto : Dto<IntegraDto>
    {
        public DateTime DataInicio { get; set; }

        public TimeSpan Intervalo { get; set; }

        public DateTime? DataUltimaIntegracao { get; set; }
    }
}