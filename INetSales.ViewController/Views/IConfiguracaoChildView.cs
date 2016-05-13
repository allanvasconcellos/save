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
    public enum EmpresaEnum
    {
        Lidimar = 1,
        TopChicle,
		Teste,
		Producao,
        Outros,
    }

	public enum TipoVendaEnum
	{
		PreVenda = 1,
		ProntaEntrega,
	}

    public interface IConfiguracaoChildView
    {
		void ShowSelecaoTipoVenda(Action<TipoVendaEnum> tipovendaSelecionadaAction, Action terminouSelecao);

        void ShowSelecaoEmpresa(Action<EmpresaEnum> empresaSelecionadaAction, Action terminouSelecao);

        void Show(ConfiguracaoDto configuracao, Func<string, string, string, string, string, string, bool> ok, Action terminouConfiguracao);
    }
}