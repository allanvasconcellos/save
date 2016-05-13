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
    public class SelecaoRemessaVisaoGrupo
    {
        public string Grupo { get; set; }

        public int QuantidadeRecebida { get; set; }

        public int QuantidadeVendida { get; set; }

        public int Saldo { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorVenda { get; set; }
    }

    public interface IAcertoSaldoView : IView
    {
        void Initialize(AcertoSaldoController controller);

        void ShowRemessas(IEnumerable<RemessaDto> remessas);

        /// <summary>
        /// Mostra as remessas selecionadas de acordo com a visão de grupo.
        /// </summary>
        /// <param name="visao"></param>
        void ShowRemessasSelecionadasVisaoGrupo(IEnumerable<SelecaoRemessaVisaoGrupo> visao);

        void PermitirAcerto();

        void DesabilitarAcerto();
    }
}