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
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Models;

namespace INetSales.ViewController.Views
{
    public interface IRelatorioPedidoOrcamentoView : IRelatorioView
    {
        void Initialize(RelatorioController controller);

        void MostrarRelatorioPedidoOrcamento(string url);
    }

	public interface IRelatorioFaturamentoCategoriaView : IRelatorioView
	{
		void InitializeFaturamentoCategoria(RelatorioController controller);

		void MostrarRelatorioFaturamentoCategoria(string url);
	}

	public interface IRelatorioClienteDevedoresView : IRelatorioView
	{
		void InitializeClienteDevedores(RelatorioController controller);

		void MostrarRelatorioClienteDevedores(string url);
	}
}