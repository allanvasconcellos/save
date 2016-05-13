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
    public interface IPedidoListaView : IPedidoProcessadoView
    {
        void ShowPedidos(IEnumerable<PedidoDto> pedidos);
    }
}