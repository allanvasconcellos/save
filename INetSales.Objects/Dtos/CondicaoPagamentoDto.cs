using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("CondicaoPagamento")]
    public class CondicaoPagamentoDto : Dto<CondicaoPagamentoDto>
    {
        public string Descricao { get; set; }

        public bool IsDefault { get; set; }

        public bool IsCheque { get; set; }

        public bool IsBoleto { get; set; }

        public bool IsPublica { get; set; }

        public override string ToString()
        {
            return this.Descricao;
        }
    }
}