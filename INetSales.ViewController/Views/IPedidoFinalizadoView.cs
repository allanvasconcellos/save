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
using INetSales.ViewController.Controllers;

namespace INetSales.ViewController.Views
{
    public interface IPedidoFinalizadoView : IPedidoProcessadoView
    {
        void ShowPedidoInfo(PedidoDto pedido, decimal totalPedido, double totalValor);

        void ShowInfoCancelado();

        void ShowInfoRejeitado();

        void ShowProdutosPedido(IEnumerable<ProdutoDto> produtos);

        void DesabilitarEnvio();

        void DesabilitarGerarNfe();

        void DesabilitarGerarBoleto();

        void HabilitarGerarNfe();

        void HabilitarGerarBoleto();
    }
}