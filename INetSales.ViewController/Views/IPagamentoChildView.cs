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

namespace INetSales.ViewController.Views
{
    public interface IPagamentoChildView
    {
        void AtualizarCondicoes(IEnumerable<CondicaoPagamentoDto> condicoes, CondicaoPagamentoDto selecionado = null);

        void PermitirInfoCheque();

        void DesabilitarInfoCheque();

        void AtualizarValorCheque(double valor);
    }
}