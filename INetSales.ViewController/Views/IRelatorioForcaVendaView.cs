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
    public interface IRelatorioForcaVendaView : IRelatorioView
    {
        void InitializeSintetico(RelatorioController controller);

		void ShowTotais(decimal quantidadeRecebida, decimal quantidadeVendida, decimal quantidadeDisponivel,
                        decimal valorTotalDinheiro, decimal valorTotalBoleto, decimal valorTotalCheque);

        void ShowGrupos(IEnumerable<GrupoInfoModel> grupos);

        void ShowProdutos(IEnumerable<ProdutoInfoModel> produtos);
    }
}