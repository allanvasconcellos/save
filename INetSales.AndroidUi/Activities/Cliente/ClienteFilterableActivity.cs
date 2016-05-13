using System;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Views;
using System.Collections;
using System.Text;
using Android.Graphics;
using Android.Util;

namespace INetSales.AndroidUi.Activities.Cliente
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.ClienteFilterable })]
	public class ClienteFilterableActivity : BaseActivity, IClienteListView
    {
        private ClienteController _controller;

        protected override void OnBeginView(Bundle bundle)
        {
            ApplicationController.Initialize(this);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            menu.Add(Menu.None, 0, 0, "Criar Pedido");
            menu.Add(Menu.None, 1, 1, "Visualizar");
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = ((AdapterView.AdapterContextMenuInfo)item.MenuInfo);
            var clienteSelecionado = GetClientePosicao(info.Position);
            if (item.ItemId == 0)
            {
                _controller.EnviarPedidoCliente(clienteSelecionado);
                return true;
            }
            if(item.ItemId == 1)
            {
                _controller.SelectCliente(clienteSelecionado);
                return true;
            }
            return false;
        }

        public override void Next()
        {
            this.LaunchActivity(ActivityFlags.ClienteDetalheCategory);
        }

        public void Initialize(ClienteController controller)
        {
            _controller = controller;
            SetContentView(Resource.Layout.ClienteList);

            var list = FindViewById<ListView>(Resource.Id.lvCliente);
            var tvMsgSemCliente = FindViewById<TextView>(Resource.Id.tvMsgSemCliente);
			var chkComRoteiro = FindViewById<CheckBox>(Resource.Id.chkComRoteiro);
			var edtFiltro = FindViewById<EditText>(Resource.Id.edtFiltro);
			var adapterFiltro = new FilterableAdapter<ClienteDto> ();
			adapterFiltro.OnBindingView += AdapterFiltro_OnBindingView;
			adapterFiltro.OnFilter += AdapterFiltro_OnFilter;
			edtFiltro.AddTextChangedListener(new TextWatcher(adapterFiltro.Filter));
			chkComRoteiro.Checked = true;
			chkComRoteiro.CheckedChange += ComRoteiroCheckedChange;
			chkComRoteiro.Visibility = ViewStates.Gone;
            list.ItemClick += ClienteItemClick;
            RegisterForContextMenu(list);
			list.Adapter = adapterFiltro;
            list.Visibility = ViewStates.Visible;
            tvMsgSemCliente.Visibility = ViewStates.Invisible;

			edtFiltro.Text = String.Empty;
        }

		private IEnumerable<ClienteDto> AdapterFiltro_OnFilter (object sender, string filtro)
        {
			var chkComRoteiro = FindViewById<CheckBox>(Resource.Id.chkComRoteiro);
			return _controller.FiltrarCliente (filtro, chkComRoteiro.Checked);
        }

		private View AdapterFiltro_OnBindingView (object sender, AdapterEventArgs<ClienteDto> args)
        {
			var cliente = args.Item;

			// Nome do cliente
			Color backColor = cliente.HasRota ? Color.Transparent : Color.Gray;
			var builder = BuildRelativeLayout.Create(this)
				.SetImage(cliente.HasPendencia ? Resource.Drawable.cliente_item_pendencia : 
					cliente.HasPedidoRoteiro ? Resource.Drawable.cliente_item_pedido : Resource.Drawable.cliente_item, 10, 15, 0, 0,
					(b, lparam) =>
					{
						lparam.AddRule(LayoutRules.CenterVertical);
					})
				.SetText(String.Format("Cliente: {0}", cliente.RazaoSocial), 60, 0, 0, 0,
					(t, param) =>
					{
						t.SetTypeface(null, TypefaceStyle.Bold);
						t.SetTextSize(ComplexUnitType.Px, 14);
					})
				;

			int top = 15;
			for (int i = 0; i <= 2; i++) { // Só exibe 3 roteiros
				if (cliente.Roteiros != null && cliente.Roteiros.Count () > i) {
					var roteiro = cliente.Roteiros.ElementAt(i);
					builder.SetText (String.Format ("Pasta: {0} - Ordem: {1} - Dia: {2}",
						roteiro.Rota.IndicePasta, roteiro.OrdemRoteiro, roteiro.Rota.DiaPasta), 60, top, 0, 0);
					top += 15;
				}
			}
			var layout = builder.Build ();
			layout.SetBackgroundColor(backColor);

			return layout;
        }

        public void ShowPedido()
        {
            this.LaunchActivity(ActivityFlags.PedidoCategory, new Dictionary<string, object> { { ActivityFlags.PedidoClienteNovoParam, true } });
        }

        public void ShowClienteDetail()
        {
            this.LaunchActivity(ActivityFlags.ClienteDetalheCategory);
        }

        #region Eventos

        private void ComRoteiroCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if(e.IsChecked)
            {
                _controller.AtualizarSemRoteiro();
            }
            else
            {
                _controller.AtualizarTodos();
            }
        }

        private void ClienteItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
			OpenContextMenu(e.View);
        }

        #endregion

        private ClienteDto GetClientePosicao(int posicao)
        {
            var lvCliente = FindViewById<ListView>(Resource.Id.lvCliente);
			return ((FilterableAdapter<ClienteDto>)lvCliente.Adapter)[posicao];
        }

        public void ShowClienteList(IEnumerable<ClienteDto> clientes)
        {
            var lvCliente = FindViewById<ListView>(Resource.Id.lvCliente);
			var adapter = (FilterableAdapter<ClienteDto>)lvCliente.Adapter;
            //adapter.UpdateContent(clientes);
        }

        public void ShowClienteListEmpty()
        {
            var lvCliente = FindViewById<ListView>(Resource.Id.lvCliente);
            var tvMsgSemCliente = FindViewById<TextView>(Resource.Id.tvMsgSemCliente);
            lvCliente.Visibility = ViewStates.Invisible;
            tvMsgSemCliente.Visibility = ViewStates.Visible;
        }
    }
}