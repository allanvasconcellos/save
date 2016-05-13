using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Controllers;

namespace INetSales.AndroidUi.Activities.Pedido
{
    public class PedidoSelecionadoModalView
    {
        private readonly Activity _activity;
        private readonly PedidoController _controller;
        private IEnumerable<ProdutoDto> _selecionados;
        private TextView _textValorFooter;

        public PedidoSelecionadoModalView(Activity activity, PedidoController controller)
        {
            _activity = activity;
            _controller = controller;
        }

        public void AtualizarSelecionados(IEnumerable<ProdutoDto> selecionados)
        {
            _selecionados = selecionados;
        }

        public void Show()
        {
            if (_selecionados != null && _selecionados.Count() > 0)
            {
                const float textSize = 16;
                var cliente = _controller.PedidoCorrente.Cliente;
                string infoCliente = String.Empty;
                if (_controller.PedidoCorrente.Tipo != TipoPedidoEnum.Remessa && _controller.PedidoCorrente.Tipo != TipoPedidoEnum.Sos)
                {
                    infoCliente = String.Format("Cliente: {0}\n{1}: {2}",
                                                cliente.NomeFantasia,
                                                cliente.TipoPessoa == TipoPessoaEnum.Fisica ? "CPF" : "CNPJ",
                                                cliente.Documento);
                }
                var layout = BuildLayout.Create(_activity, Orientation.Vertical)
                    .SetText(infoCliente, 0, 0, 0, 20,
                             c =>
                             {
                                 c.Gravity = GravityFlags.CenterHorizontal;
                                 c.SetTextSize(ComplexUnitType.Px, 13);
                                 var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
                                 if (c.LayoutParameters != null)
                                 {
                                     lp = (LinearLayout.LayoutParams)c.LayoutParameters;
                                     lp.Width = ViewGroup.LayoutParams.FillParent;
                                     lp.Height = ViewGroup.LayoutParams.FillParent;
                                 }
                                 c.LayoutParameters = lp;
                             })
                    .SetTable(_selecionados,
                              header => // Header
                              {
                                  var descricaoHeader = new TextView(_activity)
                                                            {
                                                                Text = "Descrição",
                                                                Gravity = GravityFlags.Left,
                                                                TextSize = textSize + 2,
                                                            };
                                  var quantidadePedidoHeader = new TextView(_activity)
                                                                   {
                                                                       Text = "Pedido",
                                                                       Gravity = GravityFlags.CenterHorizontal,
                                                                       TextSize = textSize + 2,
                                                                   };
                                  quantidadePedidoHeader.SetPadding(20, 0, 0, 0);
                                  var valorUnitarioHeader = new TextView(_activity)
                                                                {
                                                                    Text = "Unitário",
                                                                    Gravity = GravityFlags.CenterHorizontal,
                                                                    TextSize = textSize + 2,
                                                                };
                                  valorUnitarioHeader.SetPadding(20, 0, 0, 0);
                                  var valorTotalPedidoHeader = new TextView(_activity)
                                                                   {
                                                                       Text = "Total",
                                                                       Gravity = GravityFlags.CenterHorizontal,
                                                                       TextSize = textSize + 2,
                                                                   };
                                  valorTotalPedidoHeader.SetPadding(30, 0, 0, 0);
                                  var descontoHeader = new TextView(_activity)
                                                           {
                                                               Text = "Desconto",
                                                               Gravity = GravityFlags.CenterHorizontal,
                                                               TextSize = textSize + 2,
                                                           };
                                  descontoHeader.SetPadding(25, 0, 0, 0);
                                  header.AddView(descricaoHeader);
                                  header.AddView(quantidadePedidoHeader);
                                  header.AddView(valorUnitarioHeader);
                                  header.AddView(valorTotalPedidoHeader);
                                  header.AddView(descontoHeader);
                              },
                              (position, produto, row) => // Itens
                              {
                                  row.SetPadding(0, 10, 0, 0);
                                  var nomeColumn = new TextView(_activity)
                                                       {
                                                           Text = GetNomeProduto(produto),
                                                           Gravity = GravityFlags.Left,
                                                           Typeface = Typeface.DefaultBold,
                                                           TextSize = textSize + 1
                                                       };
                                  var quantidadePedidoColumn = new TextView(_activity)
                                                                   {
                                                                       Text =
                                                                           String.Format("{0}",
                                                                                         produto.QuantidadePedido),
                                                                       Gravity = GravityFlags.CenterHorizontal,
                                                                       TextSize = textSize
                                                                   };
                                  quantidadePedidoColumn.SetPadding(20, 0, 0, 0);
                                  var valorUnitarioColumn = new TextView(_activity)
                                                                {
                                                                    Text =
                                                                        String.Format("{0:C}", produto.ValorUnitario),
                                                                    Gravity = GravityFlags.CenterHorizontal,
                                                                    TextSize = textSize,
                                                                };
                                  valorUnitarioColumn.SetPadding(20, 0, 0, 0);
                                  var valorTotalPedidoColumn = new TextView(_activity)
                                                                   {
                                                                       Text =
                                                                           String.Format("{0:C}",
                                                                                         produto.ValorTotalPedido),
                                                                       Gravity = GravityFlags.CenterHorizontal,
                                                                       TextSize = textSize,
                                                                   };
                                  valorTotalPedidoColumn.SetPadding(30, 0, 0, 0);

                                  Logger.Debug("Inserindo o campo para o desconto");
                                  var descontoLayout = new LinearLayout(_activity);
                                  descontoLayout.Orientation = Orientation.Horizontal;
                                  var descontoColumn = new EditText(_activity)
                                                           {
                                                               Gravity = GravityFlags.CenterHorizontal,
                                                               TextSize = textSize,
                                                               Text = String.Format("{0}", produto.Desconto),
                                                               InputType = InputTypes.NumberFlagDecimal,
                                                           };
                                  var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                                         ViewGroup.LayoutParams.WrapContent);
                                  descontoColumn.SetHeight(10);
                                  descontoColumn.SetWidth(50);
                                  descontoColumn.FocusChange += DescontoFocusChange;
                                  //descontoColumn.KeyListener = DigitsKeyListener.GetInstance("0123456789.");
                                  descontoColumn.KeyListener = DigitsKeyListener.GetInstance(false, true);
                                  lp.SetMargins(40, 0, 0, 0);
                                  descontoColumn.LayoutParameters = lp;
                                  var porcentTextColumn = new TextView(_activity) { Text = "%", Gravity = GravityFlags.CenterHorizontal };
                                  descontoColumn.TextChanged +=
                                      (sender, e) => DescontoTextChanged(produto, e, valorTotalPedidoColumn);
                                  descontoLayout.AddView(descontoColumn);
                                  descontoLayout.AddView(porcentTextColumn);

                                  if (_controller.PedidoCorrente.Tipo == TipoPedidoEnum.Remessa || _controller.PedidoCorrente.Tipo == TipoPedidoEnum.Sos)
                                  {
                                      descontoColumn.Enabled = false;
                                  }

                                  row.AddView(nomeColumn);
                                  row.AddView(quantidadePedidoColumn);
                                  row.AddView(valorUnitarioColumn);
                                  row.AddView(valorTotalPedidoColumn);
                                  row.AddView(descontoLayout);

                                  row.LayoutParameters =
                                      new TableLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                                   ViewGroup.LayoutParams.WrapContent);
                              },
                            footer => // Rodapé
                            {
                                var footerLayout = BuildLayout.Create(_activity, Orientation.Vertical)
                                    .SetText(String.Format("Total Pedido: {0}", _selecionados.Sum(p => p.QuantidadePedido)), 0, 20)
                                    .SetText(String.Empty, 0, 0, 0, 0, UpdateValorTotalPedidoFooter)
                                    .Build();

                                footer.SetGravity(GravityFlags.Right);
                                footer.AddView(footerLayout);
                            })
                    .Build(table => table.SetPadding(10, 10, 10, 10));
                var scroll = new ScrollView(_activity)
                                 {
                                     LayoutParameters = new ScrollView.LayoutParams(ViewGroup.LayoutParams.FillParent,
                                                                                    ViewGroup.LayoutParams.FillParent)
                                 };
                scroll.AddView(layout);
                _activity.ShowDialog("Pedidos Selecionados", scroll);
                return;
            }

            ActivityHelper.ShowMessageBox(_activity, "Nenhum produto selecionado", "Pedido", (sender, e) => { });
        }

        private void UpdateValorTotalPedidoFooter(TextView textView)
        {
            _textValorFooter = textView;
            _textValorFooter.Text = String.Format("Total Valor: {0:C}", _selecionados.Sum(p => p.ValorTotalPedido));
        }

        private string GetNomeProduto(ProdutoDto produto)
        {
            if (produto.Nome.Length > 15)
            {
                return String.Format("{0}...", produto.Nome.Substring(0, 15));
            }
            return produto.Nome;
        }

        private void DescontoFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                ((EditText)sender).SelectAll();
            }
        }

        private void DescontoTextChanged(ProdutoDto produto, TextChangedEventArgs e, TextView valorTotalControl)
        {
            string texto = e.Text.ToString();
            decimal valor = 0;
            if (!String.IsNullOrEmpty(texto))
            {
                valor = ActivityHelper.GetTextDecimal(e.Text.ToString());
            }
            ProdutoDto produtoAtualizado = null;
            if (_controller.AtualizarDesconto(produto, valor, out produtoAtualizado))
            {
                valorTotalControl.Text = String.Format("{0:C}", produtoAtualizado.ValorTotalPedido);
                UpdateValorTotalPedidoFooter(_textValorFooter);
            }
        }
    }
}