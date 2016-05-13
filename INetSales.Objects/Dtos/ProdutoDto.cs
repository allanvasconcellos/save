using System;
using System.Collections.Generic;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Produto")]
    public class ProdutoDto : Dto<ProdutoDto>
    {
        public string Nome { get; set; }

		//[ForeignKey(typeof(GrupoDto))]
		public int GrupoId { get; set; }
		//[ManyToOne]
		[Ignore]
        public GrupoDto Grupo { get; set; }

        public decimal QuantidadeInicial { get; set; }

		[Ignore]
        public decimal QuantidadeDisponivel { get; set; }

        /// <summary>
        /// Saldo atual da quantidade do produto.
        /// </summary>
        public decimal SaldoAtual { get; set; }

		[Ignore]
        public decimal QuantidadePedido { get; set; }

        /// <summary>
        /// Calculo para todos os pedidos feitos para este produto e usuário
        /// </summary>
        public decimal QuantidadeTotalPedido { get; set; }

        public double ValorUnitario { get; set; }

        /// <summary>
        /// Valor do pedido sem o desconto.
        /// </summary>
        public double ValorPedidoSemDesconto { get; set; }

        /// <summary>
        /// Valor total do pedido já calculado com o desconto.
        /// </summary>
        public double ValorTotalPedido { get; set; }

        /// <summary>
        /// Percentual do desconto
        /// </summary>
		[Ignore]
        public double Desconto { get; set; }

        /// <summary>
        /// Valor do desconto no pedido.
        /// </summary>
        public double ValorTotalDesconto { get; set; }

//		[OneToMany(CascadeOperations = CascadeOperation.All)]
//		public List<ProdutoSaldoDto> Saldos { get; set; }

    }
}