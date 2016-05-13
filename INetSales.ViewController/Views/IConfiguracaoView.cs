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
    public interface IConfiguracaoView : IView
    {
        void Initialize(ConfiguracaoController controller);

        void ShowConfiguracaoAtual(ConfiguracaoDto configuracao);

        void ShowCamposVazios();
    }
}