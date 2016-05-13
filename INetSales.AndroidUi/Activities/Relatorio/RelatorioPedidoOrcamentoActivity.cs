using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Models;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi
{
	public enum TipoRelatorioErp
	{
		PedidoOrcamento = 1,
		FaturamentoCategoria,
		ClienteDevedores,
	}

	[Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.RelatorioPedidoOrcamentoCategory })]        
	public class RelatorioPedidoOrcamentoActivity : BaseActivity, 
	IRelatorioPedidoOrcamentoView, IRelatorioFaturamentoCategoriaView, IRelatorioClienteDevedoresView
    {
        RelatorioController Controller
        {
            get;
            set;
        }

        protected override void OnBeginView(Bundle bundle)
        {
			var tipo = Intent.GetExtra<TipoRelatorioErp>(ActivityFlags.TipoRelatorioErpParam);
			switch (tipo) {
			case TipoRelatorioErp.PedidoOrcamento:
				ApplicationController.InitializeRelatorioPedidoOrcamento (this);	
				break;
			case TipoRelatorioErp.FaturamentoCategoria:
				ApplicationController.InitializeRelatorioFaturamentoCategoria(this);	
				break;
			case TipoRelatorioErp.ClienteDevedores:
				ApplicationController.InitializeRelatorioClienteDevedores(this);	
				break;
			}
            
        }

        public void Initialize(RelatorioController controller)
        {
            this.Controller = controller;
            SetContentView(Resource.Layout.RelatorioPedidoOrcamento);

            var etDataInicio = FindViewById<EditText>(Resource.Id.txtRelPedOrcDataInicio);
            var etDataFim = FindViewById<EditText>(Resource.Id.txtRelPedOrcDataFim);
			var btnRelPedInicio = FindViewById<Button>(Resource.Id.btnRelPedInicio);
			var btnRelPedFim = FindViewById<Button>(Resource.Id.btnRelPedFim);
            var btnRelPedOrcGerar = FindViewById<Button>(Resource.Id.btnRelPedOrcGerar);
			var tvTitulo = FindViewById<TextView>(Resource.Id.tvTitulo);
			var tipo = Intent.GetExtra<TipoRelatorioErp>(ActivityFlags.TipoRelatorioErpParam);
			switch (tipo) {
			case TipoRelatorioErp.PedidoOrcamento:
				tvTitulo.Text = Resources.GetString(Resource.String.relatorio_pedido_orcamento);
				break;
			case TipoRelatorioErp.FaturamentoCategoria:
				tvTitulo.Text = Resources.GetString(Resource.String.relatorio_faturamento_categorio_produto);
				break;
			case TipoRelatorioErp.ClienteDevedores:
				tvTitulo.Text = Resources.GetString(Resource.String.relatorio_cliente_devedores);
				break;
			}

            etDataInicio.RequestFocus();
			etDataFim.ClearFocus ();
			btnRelPedInicio.Click += (sender, e) => 
			{
				DateTime inicio;
				if(!DateTime.TryParse(etDataInicio.Text, out inicio))
				{
					inicio = DateTime.Now;
				}
				var picker = new DatePickerDialog(this, OnDateSetInicio, inicio.Year, inicio.Month - 1, inicio.Day);
				picker.Show();
			};
			btnRelPedFim.Click += (sender, e) => 
			{
				DateTime fim;
				if(!DateTime.TryParse(etDataFim.Text, out fim))
				{
					if(!DateTime.TryParse(etDataInicio.Text, out fim))
					{
						fim = DateTime.Now;
					}
				}
				var picker = new DatePickerDialog(this, OnDateSetFim, fim.Year, fim.Month - 1, fim.Day);
				picker.Show();
			};
            bool firstFocusInicio = true;

            btnRelPedOrcGerar.Click += (object sender, EventArgs e) => {
				switch (tipo) {
				case TipoRelatorioErp.PedidoOrcamento:
					Controller.GerarRelatorioPedidoOrcamento(etDataInicio.Text, etDataFim.Text, true);
					break;
				case TipoRelatorioErp.FaturamentoCategoria:
					Controller.GerarRelatorioFaturamentoCategoria(etDataInicio.Text, etDataFim.Text, true);
					break;
				case TipoRelatorioErp.ClienteDevedores:
					Controller.GerarRelatorioClienteDevedores(etDataInicio.Text, etDataFim.Text, true);
					break;
				}
            };

            etDataInicio.FocusChange += (object sender, View.FocusChangeEventArgs e) => {
                if(e.HasFocus && !firstFocusInicio)
                {
                    DateTime dateForPicker = DateTime.Now;
                    if(!String.IsNullOrEmpty(etDataInicio.Text))
                    {
                        dateForPicker = DateTime.Parse(etDataInicio.Text);
                    }
                    var picker = new DatePickerDialog(this, OnDateSetInicio, dateForPicker.Year, dateForPicker.Month - 1, dateForPicker.Day);
                    picker.Show();
                }
                firstFocusInicio = false;
            };

            etDataFim.FocusChange += (object sender, View.FocusChangeEventArgs e) => {
                if(e.HasFocus)
                {
                    DateTime dateForPicker = DateTime.Now;
                    if(!String.IsNullOrEmpty(etDataFim.Text))
                    {
                        dateForPicker = DateTime.Parse(etDataFim.Text);
                    }
                    var picker = new DatePickerDialog(this, OnDateSetFim, dateForPicker.Year, dateForPicker.Month - 1, dateForPicker.Day);
                    picker.Show();
                }
            };
        }

		public void InitializeFaturamentoCategoria (RelatorioController controller)
		{
			Initialize (controller);
		}

		public void MostrarRelatorioFaturamentoCategoria (string url)
		{
			MostrarRelatorioOpenUrl (url);
		}

        private void OnDateSetInicio(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var etDataInicio = FindViewById<EditText>(Resource.Id.txtRelPedOrcDataInicio);
            etDataInicio.Text = String.Format("{0:dd/MM/yyyy}", e.Date);
        }

        private void OnDateSetFim(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var etDataFim = FindViewById<EditText>(Resource.Id.txtRelPedOrcDataFim);
            etDataFim.Text = String.Format("{0:dd/MM/yyyy}", e.Date);
        }

        public void MostrarRelatorioPedidoOrcamento(string url)
        {
			MostrarRelatorioOpenUrl (url);
        }

		private void MostrarRelatorioOpenUrl(string url)
		{
			var uri = Android.Net.Uri.Parse (url);
			var intent = new Intent (Intent.ActionView, uri);
			StartActivity (intent);
		}

		public void InitializeClienteDevedores (RelatorioController controller)
		{
			Initialize (controller);
		}

		public void MostrarRelatorioClienteDevedores (string url)
		{
			MostrarRelatorioOpenUrl (url);
		}
    }
}

