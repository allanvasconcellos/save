using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace INetSales.Objects.Dtos
{
    public class EstoqueDto : Dto<EstoqueDto>
    {
        public int QuantidadeInicial { get; set; }

        public int QuantidadeDisponivel { get; set; }

        public int QuantidadePedido { get; set; }

        public ProdutoDto Produto { get; set; }

        public int RemessaId { get; set; }

        public int UsuarioId { get; set; }

        public bool IsAcerto { get; set; }

        public DateTime DataEntrega { get; set; }
    }
}