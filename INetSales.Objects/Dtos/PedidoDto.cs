using System;
using System.Collections.Generic;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Pedido")]
    public class PedidoDto : Dto<PedidoDto>, IUploader
    {
        public string NFe { get; set; }

        public TipoPedidoEnum Tipo { get; set; }

		public int UsuarioId { get; set; }
        /// <summary>
        /// Usuario que realizou o pedido.
        /// </summary>
		[Ignore]
        public UsuarioDto Usuario { get; set; }

		public int ClienteId { get; set; }
		[Ignore]
        public ClienteDto Cliente { get; set; }

		public int RotaId { get; set; }
		[Ignore]
        public RotaDto Roteiro { get; set; }

		[Ignore]
        public IEnumerable<ProdutoDto> Produtos { get; set; }

		[Ignore]
        public IEnumerable<PagamentoDto> Pagamentos { get; set; }

        public bool HasCondicaoBoleto { get; set; }

		[Ignore]
        public StatusEmissaoPedido StatusEmissao { get; set; }

        public bool IsCancelado { get; set; }

        public bool IsRejeitado { get; set; }

        public string UrlHttpNFe { get; set; }

        public string UrlHttpBoleto { get; set; }

        public string UrlLocalNFe { get; set; }

        public string UrlLocalBoleto { get; set; }

		public string OrdemCompra { get; set; }

        #region Implementation of IUploader

        /// <summary>
        /// Indica que esta pendente de upload.
        /// </summary>
        public bool IsPendingUpload { get; set; }

        /// <summary>
        /// Data em foi feito o ultimo upload.
        /// </summary>
        public DateTime? DataLastUpload { get; set; }

        #endregion
    }
}