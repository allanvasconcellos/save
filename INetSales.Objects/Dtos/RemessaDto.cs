using System;
using System.Collections.Generic;

namespace INetSales.Objects.Dtos
{
    public class RemessaDto : Dto<RemessaDto>, IUploader
    {
        public string Descricao { get; set; }

        public int QuantidadeTotal { get; set; }

        public int QuantidadeInicial { get; set; }

        public int QuantidadeDisponivel { get; set; }

        public ProdutoDto Produto { get; set; }

        public UsuarioDto Usuario { get; set; }

        public DateTime DataPedido { get; set; }

        public DateTime DataEntrega { get; set; }

        public bool IsEstoqueZero { get; set; }

        public bool IsSos { get; set; }

        public IEnumerable<EstoqueDto> Estoques { get; set; }

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