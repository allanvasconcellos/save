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
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public class AcertoSaldoController : BaseController<IAcertoSaldoView>
    {
        public AcertoSaldoController(IAcertoSaldoView view, IApplication application) : base(view, application, null)
        {
        }

        public void SelecionarRemessa(RemessaDto remessa)
        {

        }

        internal void InitializeView()
        {
            var remessas = new List<RemessaDto>
            {
                new RemessaDto {Codigo = "XXX10", QuantidadeInicial = 100, QuantidadeDisponivel = 10},
                new RemessaDto {Codigo = "XXX11", QuantidadeInicial = 200, QuantidadeDisponivel = 150},
                new RemessaDto {Codigo = "XXX12", QuantidadeInicial = 288, QuantidadeDisponivel = 98},
                new RemessaDto {Codigo = "XXX13", QuantidadeInicial = 400, QuantidadeDisponivel = 300},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
                new RemessaDto {Codigo = "XXX14", QuantidadeInicial = 500, QuantidadeDisponivel = 67},
            };
            View.Initialize(this);
            View.ShowRemessas(remessas);
        }
    }
}