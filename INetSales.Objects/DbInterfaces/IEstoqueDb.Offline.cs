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
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IOfflineEstoqueDb : IOfflineDb<EstoqueDto>
    {
        /// <summary>
        /// Retorna o estoque mais antigo do carro, que não foi feito o acerto de saldo.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        IEnumerable<EstoqueDto> GetEstoquesAntigosNaoAcertado(ProdutoDto produto, UsuarioDto usuario);
    }
}