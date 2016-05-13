using System;
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

namespace INetSales.AndroidUi.Activities.Cliente
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.ClienteCategory })]
    public class ClienteListActivity : BaseActivity, IClienteListView
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
            var chkSemRoteiro = FindViewById<CheckBox>(Resource.Id.chkComRoteiro);
            chkSemRoteiro.Checked = false;
            chkSemRoteiro.CheckedChange += SemRoteiroCheckedChange;
            list.ItemClick += ClienteItemClick;
            RegisterForContextMenu(list);
            var adapter = new DtoAdapter<ClienteDto>
            {
                BindingGetView = (p, v, vg, c) =>
                {
                    var layout = new LinearLayout(this);
                    var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
                    lp.SetMargins(10, 10, 0, 10);

                    // Nome do cliente
                    var nomeText = BuildControl<TextView>
                        .Create(this)
                        .SetText(c.RazaoSocial)
                        .Build();
                    layout.AddView(nomeText, lp);

                    return layout;
                }
            };
            list.Adapter = adapter;
            list.Visibility = ViewStates.Visible;
            tvMsgSemCliente.Visibility = ViewStates.Invisible;
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

        private void SemRoteiroCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
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
            var clienteSelecionado = GetClientePosicao(e.Position);
            if(!clienteSelecionado.HasRota)
            {
                OpenContextMenu(e.View);
                return;
            }
            _controller.SelectCliente(clienteSelecionado);
        }

        #endregion

        private ClienteDto GetClientePosicao(int posicao)
        {
            var lvCliente = FindViewById<ListView>(Resource.Id.lvCliente);
            return ((DtoAdapter<ClienteDto>)lvCliente.Adapter)[posicao];
        }

        public void ShowClienteList(IEnumerable<ClienteDto> clientes)
        {
            var lvCliente = FindViewById<ListView>(Resource.Id.lvCliente);
            var adapter = (DtoAdapter<ClienteDto>)lvCliente.Adapter;
            adapter.UpdateContent(clientes);
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