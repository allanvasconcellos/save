using System.Collections.Generic;

namespace INetSales.ViewController.Models
{
    public class GrupoInfoModel
    {
        public string Nome { get; set; }

        public IEnumerable<ProdutoInfoModel> Produtos { get; set; }

		public decimal QuantidadeRecebida { get; set; }

		public decimal QuantidadeVendida { get; set; }

		public decimal QuantidadeDisponivel { get; set; }

        public decimal ValorPagoDinheiro { get; set; }

        public decimal ValorPagoCheque { get; set; }

        public decimal ValorPagoBoleto { get; set; }
    }
}