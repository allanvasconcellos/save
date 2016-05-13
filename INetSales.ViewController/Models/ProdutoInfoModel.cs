namespace INetSales.ViewController.Models
{
    public class ProdutoInfoModel
    {
        public string Nome { get; set; }

        public decimal QuantidadeRecebida { get; set; }

		public decimal QuantidadeDisponivel { get; set; }

		public decimal QuantidadeVendida { get; set; }

		public decimal QuantidadeDevolver { get; set; }

        public decimal ValorPagoDinheiro { get; set; }

        public decimal ValorPagoCheque { get; set; }

        public decimal ValorPagoBoleto { get; set; }
    }
}