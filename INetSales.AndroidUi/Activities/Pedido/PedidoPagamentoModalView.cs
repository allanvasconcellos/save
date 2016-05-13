using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi.Activities.Pedido
{
    public class PedidoPagamentoModalView : IPagamentoChildView
    {
        private readonly Activity _activity;
        private readonly PedidoController _controller;
        private IEnumerable<CondicaoPagamentoDto> _condicoes;
        private View _view;
        private TableLayout _layoutChequeInfo;

        public PedidoPagamentoModalView(Activity activity, PedidoController controller)
        {
            _activity = activity;
            _controller = controller;
        }

        public void Show()
        {
            _controller.AtualizarPagamentoView(this);
            _activity.ShowDialog("Pagamento", dialog =>
            {
                var layout = BuildLayout.Create(_activity, Orientation.Vertical)
                    .SetView(_view)
                    .SetButton("Fechar", 0, 20, 0, 0, b =>
                    {
                        b.Gravity = GravityFlags.CenterHorizontal;
                        b.Click += (sender, e) => dialog.Cancel();
                    })
                    .Build();
                dialog.SetCancelable(false);
                return layout;
            });
        }

        private void LoadView()
        {
            if (_view == null)
            {
                _view = _activity.LayoutInflater.Inflate(Resource.Layout.PedidoPagamento, null);
                _layoutChequeInfo = _view.FindViewById<TableLayout>(Resource.Id.layoutChequeInfo);
                var snTipoPagamento = _view.FindViewById<Spinner>(Resource.Id.snTipoPagamento);
                snTipoPagamento.ItemSelected += (sender, e) => TipoCondicaoItemSelected(e.Position);
                LoadTableLayoutChequeInfo(_layoutChequeInfo);
                _layoutChequeInfo.SetGravity(GravityFlags.CenterHorizontal);
            }
        }

        private void LoadTableLayoutChequeInfo(TableLayout table)
        {
            const int textSize = 14;
            table.RemoveAllViewsInLayout();
            BuildTableLayout.Use(table, Orientation.Vertical)
                .SetHeader(header =>
                               {
                                   var numeroHeader = new TextView(_activity) { Text = "Número", Gravity = GravityFlags.CenterHorizontal, TextSize = textSize + 2, };
                                   var agenciaHeader = new TextView(_activity) { Text = "Agência", Gravity = GravityFlags.CenterHorizontal, TextSize = textSize + 2, };
                                   var bancoHeader = new TextView(_activity) {  Text = "Banco", Gravity = GravityFlags.CenterHorizontal, TextSize = textSize + 2, };
                                   var valorChequeHeader = new TextView(_activity) { Text = "Valor", Gravity = GravityFlags.Left, TextSize = textSize + 2, };
                                   var valorTrParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                                   var trParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                                   valorTrParams.Span = 3;
                                   trParams.SetMargins(15, 0, 0, 0);
                                   valorTrParams.SetMargins(25, 0, 0, 0);
                                   //numeroHeader.SetPadding(10, 0, 0, 0);
                                   //agenciaHeader.SetPadding(10, 0, 0, 0);
                                   //bancoHeader.SetPadding(10, 0, 0, 0);
                                   //valorChequeHeader.SetPadding(15, 0, 0, 0);
                                   header.AddView(numeroHeader);
                                   header.AddView(agenciaHeader, trParams);
                                   header.AddView(bancoHeader, trParams);
                                   header.AddView(valorChequeHeader, valorTrParams);
                               })
                .SetRow((position, row) => LoadRowChequeInfo(row, textSize, position))
                .SetFooter(fotter =>
                               {
                                   var btAdicionar = new ImageButton(_activity);
                                   var valorTrParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
                                   valorTrParams.Span = 5;
                                   valorTrParams.Gravity = GravityFlags.Left;
                                   btAdicionar.SetImageResource(Resource.Drawable.add);
                                   btAdicionar.SetBackgroundResource(0);
                                   btAdicionar.Click += AdicionarChequeClick;
                                   fotter.AddView(btAdicionar, valorTrParams);
                               })
                .Build();
        }

        private void AdicionarChequeClick(object sender, EventArgs e)
        {
            BuildTableLayout.Use(_layoutChequeInfo, Orientation.Vertical)
                .SetRow((position, row) => LoadRowChequeInfo(row, 14, position))
                .Build();
        }

        private void LoadRowChequeInfo(TableRow row, int textSize, int position)
        {
            const int width = 70;
            const int height = 10;
            Guid uid = Guid.NewGuid();
            var numeroColumn = new EditText(_activity) { Gravity = GravityFlags.CenterHorizontal, TextSize = textSize, };
            var agenciaColumn = new EditText(_activity) { Gravity = GravityFlags.CenterHorizontal, TextSize = textSize, };
            var bancoColumn = new EditText(_activity) { Gravity = GravityFlags.CenterHorizontal, TextSize = textSize, };
            var valorColumn = new EditText(_activity) { Gravity = GravityFlags.CenterHorizontal, TextSize = textSize, InputType = InputTypes.ClassNumber, };
            var btConfirmar = new ImageButton(_activity);
            var btExcluirRow = new ImageButton(_activity);
            btConfirmar.SetImageResource(Resource.Drawable.ok);
            btExcluirRow.SetImageResource(Resource.Drawable.excluir);
            btConfirmar.SetBackgroundResource(0);
            btExcluirRow.SetBackgroundResource(0);
            btConfirmar.Click += (sender, e) =>
                                     {
                                         if(_controller.AdicionarInfoCheque(uid, numeroColumn.Text, agenciaColumn.Text, bancoColumn.Text, ActivityHelper.GetTextDecimal(valorColumn.Text)))
                                         {
                                             numeroColumn.Enabled = agenciaColumn.Enabled = bancoColumn.Enabled = valorColumn.Enabled = false;
                                         }
                                     };
            btExcluirRow.Click += (sender, e) =>
                                    {
                                        if(numeroColumn.Enabled)
                                        {
                                            _layoutChequeInfo.RemoveView(row);
                                            return;
                                        }
                                        if (_controller.RemoverInfoCheque(uid))
                                        {
                                            _layoutChequeInfo.RemoveView(row);
                                        }
                                    };
            var lp = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            var lpBotoes = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            lp.SetMargins(15, 0, 0, 0);
            lpBotoes.SetMargins(5, 0, 0, 0);
            numeroColumn.SetHeight(height); numeroColumn.SetWidth(width);
            agenciaColumn.SetHeight(height); agenciaColumn.SetWidth(width);
            bancoColumn.SetHeight(height); bancoColumn.SetWidth(width);
            valorColumn.SetHeight(height); valorColumn.SetWidth(width);
            valorColumn.KeyListener = DigitsKeyListener.GetInstance("0123456789.");
            row.AddView(numeroColumn);
            row.AddView(agenciaColumn, lp);
            row.AddView(bancoColumn, lp);
            row.AddView(valorColumn, lp);
            row.AddView(btConfirmar, lpBotoes);
            if (position > 0)
            {
                row.AddView(btExcluirRow, lpBotoes);
            }
        }

        private void TipoCondicaoItemSelected(int position)
        {
            var condicaoSelecionada = _condicoes.ElementAt(position);
            _controller.SelecionarCondicao(condicaoSelecionada);
        }

        public void AtualizarValores(double valorTotalSolicitado, double valorTotalDesconto, double valorTotalFinal)
        {
            LoadView();
            var tvTotalSelecionado = _view.FindViewById<TextView>(Resource.Id.tvTotalSelecionado);
            var tvTotalDesconto = _view.FindViewById<TextView>(Resource.Id.tvTotalDesconto);
            var tvTotalFinal = _view.FindViewById<TextView>(Resource.Id.tvTotal);
            tvTotalSelecionado.Text = String.Format("{0:C}", valorTotalSolicitado);
            tvTotalDesconto.Text = String.Format("{0:C}", valorTotalDesconto);
            tvTotalFinal.Text = String.Format("{0:C}", valorTotalFinal);
        }

        #region Implementation of IPagamentoChildView

        public void AtualizarCondicoes(IEnumerable<CondicaoPagamentoDto> condicoes, CondicaoPagamentoDto selecionado = null)
        {
            LoadView();
            _condicoes = condicoes;
            var snTipoPagamento = _view.FindViewById<Spinner>(Resource.Id.snTipoPagamento);
            var adapter = new ArrayAdapter<CondicaoPagamentoDto>(_activity,
                                                                 Android.Resource.Layout.SimpleSpinnerItem,
                                                                 _condicoes.ToArray());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            snTipoPagamento.Adapter = adapter;

            if (selecionado != null)
            {
                for (int i = 0; i < condicoes.Count(); i++)
                {
                    if (condicoes.ElementAt(i).Id == selecionado.Id)
                    {
                        snTipoPagamento.SetSelection(i);
                        break;
                    }
                }
            }
        }

        public void PermitirInfoCheque()
        {
            var trValorCheque = _view.FindViewById<TableRow>(Resource.Id.trValorCheque);
            _layoutChequeInfo.Visibility = ViewStates.Visible;
            trValorCheque.Visibility = ViewStates.Visible;
        }

        public void DesabilitarInfoCheque()
        {
            var trValorCheque = _view.FindViewById<TableRow>(Resource.Id.trValorCheque);
            _layoutChequeInfo.Visibility = ViewStates.Gone;
            trValorCheque.Visibility = ViewStates.Gone;
        }

        public void AtualizarValorCheque(double valor)
        {
            var tvValorCheque = _view.FindViewById<TextView>(Resource.Id.tvValorCheque);
            tvValorCheque.Text = String.Format("{0:C}", valor);
        }

        #endregion
    }
}