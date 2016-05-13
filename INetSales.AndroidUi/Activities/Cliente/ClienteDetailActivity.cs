using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi.Activities.Cliente
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.ClienteDetalheCategory })]
    public class ClienteDetailActivity : BaseTabActivity, IClienteDetailView
    {
        private ClienteDto _cliente;
        protected ClienteDetailGeralActivity GeralActivity { get; private set; }
        protected ClienteDetailEnderecoActivity EnderecoActivity { get; private set; }
		protected ClienteDetailPendenciasActivity PendenciaActivity { get; private set; }

        private const string GERAL_TAG = "Geral";
        private const string ENDERECO_TAG = "Endereco";
		private const string PENDENCIA_TAG = "Pendencia";

        protected override void OnBeginView(Bundle bundle)
        {
            ApplicationController.Initialize(this);
        }

        protected override void OnTabChange(string tag)
        {
            if (_cliente != null)
            {
                switch (tag)
                {
                    case GERAL_TAG:
                        CarregaGeralInfo(_cliente);
                        break;
                    case ENDERECO_TAG:
                        CarregarEnderecoInfo(_cliente);
                        if(!EnderecoActivity.IsDesabilitado)
                        {
                            EnderecoActivity.Desabilitar();
                        }
                        break;
					case PENDENCIA_TAG:
						CarregarPendenciaInfo(_cliente);
						break;
                }
            }
        }

        private void CarregarEnderecoInfo(ClienteDto cliente)
        {
            EnderecoActivity.Rua = cliente.EnderecoRua;
            EnderecoActivity.Numero = cliente.EnderecoNumero;
            EnderecoActivity.Bairro = cliente.Bairro;
            EnderecoActivity.Cep = cliente.Cep;
            EnderecoActivity.Cidade = cliente.Cidade;
        }

        private void CarregaGeralInfo(ClienteDto cliente)
        {
            GeralActivity.NomeFantasia = cliente.NomeFantasia;
            GeralActivity.RazaoSocial = cliente.RazaoSocial;
            GeralActivity.Email = cliente.Email;
            GeralActivity.Documento = cliente.Documento;
            GeralActivity.Telefone = cliente.Telefone;
        }

		void CarregarPendenciaInfo (ClienteDto cliente)
		{
			PendenciaActivity.CarregarClientePendente(cliente);
		}

        public void RegistrarGeralActivity(ClienteDetailGeralActivity activity)
        {
            GeralActivity = activity;
        }

        public void RegistrarEnderecoActivity(ClienteDetailEnderecoActivity activity)
        {
            EnderecoActivity = activity;
        }

		public void RegistrarPendenciaActivity(ClienteDetailPendenciasActivity activity)
		{
			PendenciaActivity = activity;
		}

        #region Implementation of IClienteView

        public override void Next()
        {
            this.LaunchActivity(ActivityFlags.PedidoCategory, new Dictionary<string, object> { { ActivityFlags.PedidoClienteNovoParam, true } });
        }

        public void Initialize(ClienteController controller)
        {
            AddTab<ClienteDetailGeralActivity>(GERAL_TAG, "Geral", Resources.GetDrawable(Resource.Drawable.ic_tab_artists));
            AddTab<ClienteDetailEnderecoActivity>(ENDERECO_TAG, "Endere鏾", Resources.GetDrawable(Resource.Drawable.ic_tab_artists));
			if (controller.CurrentCliente.HasPendencia) {
				AddTab<ClienteDetailPendenciasActivity> (PENDENCIA_TAG, "Pendencia", Resources.GetDrawable (Resource.Drawable.ic_tab_artists));
			}

            BuildLayout.Use(BottomLayout, this)
                .SetButton("Salvar", 10, 10, 10, 10, b =>
                {
                    b.Enabled = false;
                    b.Click += delegate
                    {
                        if (GeralActivity.Validar() && EnderecoActivity.Validar())
                        {
                            var cliente = new ClienteDto();
                            cliente.NomeFantasia = GeralActivity.NomeFantasia;
                            cliente.RazaoSocial = GeralActivity.RazaoSocial;
                            cliente.Documento = GeralActivity.Documento;
                            cliente.TipoPessoa = GeralActivity.TipoPessoa;
                            cliente.Email = GeralActivity.Email;
                            cliente.Telefone = GeralActivity.Telefone;
                            cliente.EnderecoRua = EnderecoActivity.Rua;
                            cliente.EnderecoNumero = EnderecoActivity.Numero;
                            cliente.Bairro = EnderecoActivity.Bairro;
                            cliente.Cep = EnderecoActivity.Cep;
                            cliente.Cidade = EnderecoActivity.Cidade;
                            controller.UpdateClienteInfo(cliente);
                            controller.CriarCliente();
                        }
                    };
                })
                .Build();
        }

        public void ShowPedido()
        {
            this.LaunchActivity(ActivityFlags.PedidoCategory, new Dictionary<string, object> { { ActivityFlags.PedidoClienteNovoParam, true } });
        }

        public void ShowClienteDetail()
        {
            throw new NotImplementedException();
        }

        public void ShowDetail(ClienteDto cliente)
        {
            _cliente = cliente;
            CarregaGeralInfo(_cliente);
            GeralActivity.Desabilitar();
        }

        public void ShowRamoList(IEnumerable<RamoDto> ramos)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}