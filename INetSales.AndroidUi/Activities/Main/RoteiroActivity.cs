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

namespace INetSales.AndroidUi.Activities.Main
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.MainCategory })]
    public partial class RoteiroActivity : BaseActivity, IRoteiroView, IClienteView
    {
        private RoteiroController _controller;

        protected override void OnBeginView(Bundle bundle)
        {
            ApplicationController.Initialize(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            ApplicationController.Resume(this);
        }

        public override void OnBackPressed()
        {
            //_controller.Close(() => base.OnBackPressed());
        }

        public override void Next()
        {
            this.LaunchActivity(ActivityFlags.PedidoCategory);
        }

        public void Initialize(RoteiroController controller)
        {
            _controller = controller;
            SetContentView(Resource.Layout.Roteiro);
            var list = FindViewById<ListView>(Resource.Id.lvRoteiro);
            var tvDia = FindViewById<TextView>(Resource.Id.tvDia);
            var btDiaAnterior = FindViewById<ImageButton>(Resource.Id.btDiaAnterior);
            var btDiaProximo = FindViewById<ImageButton>(Resource.Id.btDiaProximo);
            var btnHoje = FindViewById<Button>(Resource.Id.btnHoje);
            var btnAtualizarLista = FindViewById<Button>(Resource.Id.btnAtualizarLista);
			var btnClientes = FindViewById<Button>(Resource.Id.btnClientesRoteiro);
            list.ItemClick += RoteiroItemClick;
            list.ItemLongClick += RoteiroItemLongClick;
            tvDia.Click += DiaClick;
            btnHoje.Click += HojeClick;
            btDiaAnterior.Click += DiaAnteriorClick;
            btDiaProximo.Click += DiaProximoClick;
            btnAtualizarLista.Click += (sender, e) => controller.AtualizarLista();
			btnClientes.Click += ClientesClick;

            InitializeFragment();
        }

        private void RoteiroItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var listView = (ListView)sender;
            var clienteSelecionado = ((DtoAdapter<ClienteDto>)listView.Adapter)[e.Position];
            _controller.SelectClienteDetail(clienteSelecionado);
        }

        private void ClientesClick (object sender, EventArgs e)
        {
			this.LaunchActivity(ActivityFlags.ClienteFilterable);
        }

        private void DiaAnteriorClick(object sender, EventArgs e)
        {
            _controller.SelectPreviousDia();
        }

        private void DiaProximoClick(object sender, EventArgs e)
        {
            _controller.SelectNextDia();
        }

        private void RoteiroItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var listView = (ListView)sender;
            var clienteSelecionado = ((DtoAdapter<ClienteDto>)listView.Adapter)[e.Position];
            _controller.SelectCliente(clienteSelecionado);
        }

        private void DiaClick(object sender, EventArgs e)
        {
            var picker = new DatePickerDialog(this, OnDateSet, _controller.DiaSelecionado.Year, _controller.DiaSelecionado.Month - 1, _controller.DiaSelecionado.Day);
            picker.Show();
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            _controller.SelectDia(e.Date);
        }

        private void HojeClick(object sender, EventArgs e)
        {
            _controller.SelectDia(DateTime.Now);
        }

        public void ShowRoteiroList(DateTime dia, int indicePasta, IEnumerable<ClienteDto> clientes)
        {
            var tvDia = FindViewById<TextView>(Resource.Id.tvDia);
            tvDia.Text = String.Format("{0}, {1} de {2} de {3}",
                GetDayOfWeekInPortugues(_controller.DiaSelecionado.DayOfWeek), _controller.DiaSelecionado.Day, GetMonthText(_controller.DiaSelecionado.Month - 1), _controller.DiaSelecionado.Year);

            var tvMsgSemCliente = FindViewById<TextView>(Resource.Id.tvMsgSemCliente);
            var lvRoteiro = FindViewById<ListView>(Resource.Id.lvRoteiro);
            var tvPasta = FindViewById<TextView>(Resource.Id.tvPasta);
            var adapter = new DtoAdapter<ClienteDto>(clientes)
            {
                BindingGetView = 
                    (p, v, vg, c) =>
                    {
                        var layout = BuildRelativeLayout.Create(this)
                            .SetImage(c.HasPendencia ? Resource.Drawable.cliente_item_pendencia : 
                                                    c.HasPedidoRoteiro ? Resource.Drawable.cliente_item_pedido : Resource.Drawable.cliente_item, 10, 15, 0, 0,
                                (b, lparam) =>
                                {
                                    lparam.AddRule(LayoutRules.CenterVertical);
                                })
                            .SetText(String.Format("{0}° - {1}", c.OrdemRoteiro, c.RazaoSocial), 60, 0, 0, 0,
                                (b, lparam) =>
                                {
                                    lparam.AddRule(LayoutRules.CenterVertical);
                                })
//                            .SetImage(Resource.Drawable.img_pesquisa, 0, 10, 0, 0, 
//                                (b, lparam) =>
//                                {
//                                    lparam.AddRule(LayoutRules.AlignParentRight);
//                                    lparam.AddRule(LayoutRules.CenterVertical);
//                                    lparam.Width = 80;
//                                    b.Click += delegate { ShowPesquisa(c); };
//                                    b.Focusable = false;
//                                    b.FocusableInTouchMode = false;
//                                })
                            .Build(l => l.SetGravity(GravityFlags.CenterVertical));

                        return layout;
                    }};
            lvRoteiro.Adapter = adapter;
            lvRoteiro.Visibility = ViewStates.Visible;
            tvMsgSemCliente.Visibility = ViewStates.Gone;
            tvPasta.Text = String.Format("Pasta {0}", indicePasta);
            tvPasta.Visibility = ViewStates.Visible;
        }

        private void ShowPesquisa(ClienteDto cliente)
        {
            _controller.SelectClientePesquisa(cliente);
        }

        public void ShowRoteiroVazio(DateTime dia)
        {
            var lvRoteiro = FindViewById<ListView>(Resource.Id.lvRoteiro);
            var tvMsgSemCliente = FindViewById<TextView>(Resource.Id.tvMsgSemCliente);
            var tvDia = FindViewById<TextView>(Resource.Id.tvDia);
            var tvPasta = FindViewById<TextView>(Resource.Id.tvPasta);
            tvPasta.Visibility = ViewStates.Gone;
            lvRoteiro.Visibility = ViewStates.Gone;
            tvMsgSemCliente.Visibility = ViewStates.Visible;

            tvDia.Text = String.Format("{0}, {1} de {2} de {3}",
                GetDayOfWeekInPortugues(dia.DayOfWeek), dia.Day, GetMonthText(dia.Month - 1), dia.Year);
        }

        public void ShowPesquisa()
        {
            this.LaunchActivity(ActivityFlags.PesquisaCategory);
        }

        private string GetMonthText(int month)
        {
            string[] months = { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };
            return months[month];
        }

        private string GetDayOfWeekInPortugues(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Segunda-Feira";
                case DayOfWeek.Tuesday:
                    return "Terça-Feira";
                case DayOfWeek.Wednesday:
                    return "Quarta-Feira";
                case DayOfWeek.Thursday:
                    return "Quinta-Feira";
                case DayOfWeek.Friday:
                    return "Sexta-Feira";
                case DayOfWeek.Saturday:
                    return "Sábado";
                case DayOfWeek.Sunday:
                    return "Domingo";
                default:
                    return String.Empty;
            }
        }

        #region Implementation of IClienteView

        public void Initialize(ClienteController controller)
        {
            throw new NotImplementedException();
        }

        public void ShowPedido()
        {
            throw new NotImplementedException();
        }

        public void ShowClienteDetail()
        {
            this.LaunchActivity(ActivityFlags.ClienteDetalheCategory);
        }

        #endregion
    }
}