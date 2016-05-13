using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Views;
using Android.Util;
using INetSales.Objects;

namespace INetSales.AndroidUi.Activities.Pedido
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.PedidoCategory })]
	public partial class PedidoActivity : BaseActivity, IRemessaView, IPedidoClienteNovoView
    {
        private PedidoController _controller;
        private PedidoPagamentoModalView _pagamentoView;
        private PedidoSelecionadoModalView _selecionadoView;
		private PedidoOCModalView _ocView;
        private IEnumerable<ProdutoDto> _produtos;
        private IEnumerable<GrupoDto> _grupos;
        private DtoAdapter<ProdutoDto> _adapterProduto;
        private DtoAdapter<GrupoDto> _adapterGrupo;

		protected override void OnBeginView(Bundle bundle)
        {
            var tipoRemessa = Intent.GetExtra<TipoPedidoEnum>(ActivityFlags.TipoPedidoParam);
            if (Intent.GetExtra<bool>(ActivityFlags.PedidoClienteNovoParam))
            {
                ApplicationController.Initialize((IPedidoClienteNovoView)this);
                return;
            }
            if(tipoRemessa == TipoPedidoEnum.Remessa || tipoRemessa == TipoPedidoEnum.Sos)
            {
                IsSos = tipoRemessa == TipoPedidoEnum.Sos;
                ApplicationController.Initialize((IRemessaView)this);
                return;
            }
            ApplicationController.Initialize((IPedidoView)this);
        }

        public override void OnBackPressed()
        {
            _controller.Close(() => base.OnBackPressed());
        }

        public void MakeQuestion(string question, Action ok, Action cancel)
        {
            ActivityHelper.ShowQuestion(this, question, "Pedido", ok, cancel);
        }

        public void UpdateViewTitle(params string[] titleLines)
        {
			this.SetTitle("SAVE - SISTEMA DE AUTOMACAO DE VENDAS", titleLines);
        }

        public void ShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        public void ShowModalMessage(string title, string message, Action ok)
        {
            ActivityHelper.ShowMessageBox(this, message, title, (sender, e) => { });
        }

        public void Next()
        {
            this.RunOnUiThread(() => this.LaunchActivity(ActivityFlags.PedidoFinalizadoCategory));
        }

        public void CloseView()
        {
            Finish();
        }

        public IProgressView ShowProgressView(string title)
        {
            var progress = new ProgressModalView(this);
            progress.Show(title);
            return progress;
        }

        public IConsoleView GetConsoleView(string title)
        {
            return new ConsoleModalView(this);
        }

        public IConfiguracaoChildView GetConfiguracaoView(string title)
        {
            return new ConfiguracaoModalView(this);
        }

        public void ExecuteOnBackground(Action execute)
        {
#if DEBUG
            execute();
            return;
#endif
            new Thread(() => execute()).Start();
        }

        public void ExecuteOnUI(Action execute)
        {
            throw new NotImplementedException();
        }

        public void Initialize(PedidoController controller)
        {
            SetContentView(Resource.Layout.Pedido);

            _controller = controller;
            _pagamentoView = new PedidoPagamentoModalView(this, _controller);
            _selecionadoView = new PedidoSelecionadoModalView(this, _controller);
			_ocView = new PedidoOCModalView(this, _controller);

            #region Produtos

            // Atualizar listview
            var lvProduto = FindViewById<ListView>(Resource.Id.lvProduto);
            var snGrupo = FindViewById<Spinner>(Resource.Id.snGrupo);

            snGrupo.ItemSelected += GrupoItemSelected;
            _adapterGrupo = new DtoAdapter<GrupoDto>();
            _adapterGrupo.BindingGetView = (p, v, vg, a) =>
            {
                var layout = new LinearLayout(this);
				Logger.Debug("Grupo info: {0} - {1} - {2}", a.Id, a.Codigo, a.Nome);
                // Nome do grupo
                var nomeText = BuildControl<TextView>
                    .Create(this)
                    .SetText((a.IsSubgrupo ? a.GrupoPai.Nome + " - " : "") + a.Nome)
                    .SetMargins(10, 10, 0, 10)
                    .Build();
                layout.AddView(nomeText);

                return layout;
            };
            snGrupo.Adapter = _adapterGrupo;

            _adapterProduto = new DtoAdapter<ProdutoDto>();
            _adapterProduto.BindingGetView = (p, v, vg, a) =>
            {
                var itemView = LayoutInflater.Inflate(Resource.Layout.PedidoSelecaoItem, null);
                var tvQuantidades = itemView.FindViewById<TextView>(Resource.Id.tvQuantidades);
                var tvProdutoNome = itemView.FindViewById<TextView>(Resource.Id.tvProdutoNome);
                var netQtdSolicitada = itemView.FindViewById<NumericEditText>(Resource.Id.netQtdSolicitada);

                tvQuantidades.SetTextColor(Color.Blue);

                tvProdutoNome.Text = a.Nome;
                tvQuantidades.Text = String.Format("{0} | {1}", a.QuantidadeDisponivel, a.SaldoAtual);
                netQtdSolicitada.MaximumValue = a.QuantidadeDisponivel;
                if (controller.PedidoCorrente.Tipo == TipoPedidoEnum.Remessa || controller.PedidoCorrente.Tipo == TipoPedidoEnum.Sos)
                {
                    tvQuantidades.Visibility = ViewStates.Invisible;
                    netQtdSolicitada.MaximumValue = Int32.MaxValue;
                }
                netQtdSolicitada.Value = a.QuantidadePedido;
                netQtdSolicitada.BindingValue = i =>
                {
                    var produtoAtualizado =
                        _controller.AtualizarQuantidadePedido(a, i);
                    a.QuantidadePedido = produtoAtualizado.QuantidadePedido;

                    tvQuantidades.Text = String.Format("{0} | {1}", produtoAtualizado.QuantidadeDisponivel, produtoAtualizado.SaldoAtual);
                };
                return itemView;
            };
            lvProduto.Adapter = _adapterProduto;
            if (_produtos != null && _produtos.Count() > 0)
            {
                AtualizarProdutosSelecionados(_produtos);
            }

            #endregion

            #region Botoes Inferiores
            var btnFinalizarPedido = FindViewById<Button>(Resource.Id.btnFinalizarPedido);
			var btnOC = FindViewById<Button>(Resource.Id.btnOC);
            var btnProdutosSelecionados = FindViewById<Button>(Resource.Id.btnProdutosSelecionados);
            var btnPagamento = FindViewById<Button>(Resource.Id.btnPagamento);
            btnFinalizarPedido.Click +=
                (sender, e) => _controller.FinalizarPedido();
			btnOC.Click +=
				(sender, e) => _ocView.Show();
            btnProdutosSelecionados.Click +=
                (sender, e) => _selecionadoView.Show();
            btnPagamento.Click +=
                (sender, e) => _pagamentoView.Show();
            #endregion
        }

        private void AtualizarGrupos(IEnumerable<GrupoDto> grupos)
        {
            _grupos = grupos;
            if (_adapterGrupo != null)
            {
                _adapterGrupo.UpdateContent(_grupos);
            }
        }

        public void AtualizarProdutosSelecao(IEnumerable<ProdutoDto> produtos)
        {
            _produtos = produtos;

            // 
			var grupos = _produtos.GroupBy (p => p.Grupo)
				.Select (g => g.Key);
			grupos = grupos.GroupBy(g => g.Id).Select(g => g.First());
			int countGrupo = grupos.Count();
            if (countGrupo > 0)
            {
				//grupos.Select(g => g.Key).Distinct(
				AtualizarGrupos(grupos);
            }
        }

        public void AtualizarProdutosSelecionados(IEnumerable<ProdutoDto> produtos)
        {
            _selecionadoView.AtualizarSelecionados(produtos);
        }

        public void AtualizarValores(double valorTotalSolicitado, double valorTotalDesconto, double valorFinal)
        {
            _pagamentoView.AtualizarValores(valorTotalSolicitado, valorTotalDesconto, valorFinal);
        }

        public void ShowSelecaoTipoPedido(Action selecaoVenda, Action selecaoBonificao)
        {
            this.ShowDialog("Tipo de Pedido", dialog =>
            {
                var tipoPedidoView = LayoutInflater.Inflate(Resource.Layout.TipoPedidoSelecao, null);
                var rbVenda = tipoPedidoView.FindViewById<RadioButton>(Resource.Id.rbVenda);
                rbVenda.SetTextSize(ComplexUnitType.Px, 18);
                rbVenda.Checked = true;
                var rbBonificacao = tipoPedidoView.FindViewById<RadioButton>(Resource.Id.rbBonificacao);
                rbBonificacao.SetTextSize(ComplexUnitType.Px, 18);
                rbVenda.Click += delegate
                {
                    selecaoVenda();
                    dialog.Cancel();
                };
                rbBonificacao.Click += delegate
                {
                    selecaoBonificao();
                    dialog.Cancel();
                };
                dialog.SetCancelable(false);
                return tipoPedidoView;
            });
        }

        public void PermitirPagamento()
        {
            var btnPagamento = FindViewById<Button>(Resource.Id.btnPagamento);
            btnPagamento.Enabled = true;
        }

        public void DesabilitarPagamento()
        {
            var btnPagamento = FindViewById<Button>(Resource.Id.btnPagamento);
            if (_controller.PedidoCorrente.Tipo == TipoPedidoEnum.Venda)
            {
                btnPagamento.Enabled = false;
                return;
            }
            btnPagamento.Visibility = ViewStates.Invisible;
        }

		public void DesabilitarOC ()
		{
			var btn = FindViewById<Button>(Resource.Id.btnOC);
			btn.Visibility = ViewStates.Invisible;
		}

        public void PermitirFinalizarPedido()
        {
            var btnFinalizarPedido = FindViewById<Button>(Resource.Id.btnFinalizarPedido);
            btnFinalizarPedido.Enabled = true;
        }

        public void DesabilitarFinalizarPedido()
        {
            var btnFinalizarPedido = FindViewById<Button>(Resource.Id.btnFinalizarPedido);
            btnFinalizarPedido.Enabled = false;
        }

        private void GrupoItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var grupoSelecionado = _grupos.ElementAt(e.Position);
            var produtosGrupo = _produtos.Where(p => p.Grupo.Equals(grupoSelecionado));
            if (_adapterProduto != null)
            {
                _adapterProduto.UpdateContent(produtosGrupo);
            }
        }

        #region Implementation of IRemessaView

        public bool IsSos
        {
            get;
            private set;
        }

        #endregion
    }
}