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
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public partial class PedidoController
    {
        /// <summary>
        /// Valor total sem os descontos.
        /// </summary>
        private double ValorTotalPedido
        {
            get
            {
                return _produtosSelecionados.Sum(selecionado => selecionado.ValorTotalPedido);
            }
        }

        public void AtualizarPagamentoView(IPagamentoChildView view)
        {
            var offline = DbHelper.GetOffline<IOfflineCondicaoPagamentoDb>();
            var pagamentoCorrente = PedidoCorrente.Pagamentos.FirstOrDefault();
            _pagamentoView = view;
            // INET.032 - Verifica de acordo com o perfil do usuário se exibirá somente as condicões publicas. Se for adm exibe tudo, publicas e privadas.
            var condicoes = offline.GetAll();
            if (Session.HasLogin && !Session.UsuarioLogado.IsAdm)
            {
                condicoes = condicoes // Só exibe publicas
                    .Where(c => c.IsPublica)
                    .ToList();
            }
            // INET.031
            if (!PedidoCorrente.Cliente.IsPermitidoBoleto)
            {
                condicoes = condicoes // Para o cliente do pedido não é permitido boleto, não exibe boleto.
                    .Where(c => !c.IsBoleto)
                    .ToList();
            }
            _pagamentoView.AtualizarCondicoes(condicoes, pagamentoCorrente != null ? pagamentoCorrente.Condicao : null);
        }

        public void SelecionarCondicao(CondicaoPagamentoDto condicao)
        {
            var pagamentos = new List<PagamentoDto>(PedidoCorrente.Pagamentos);
            if (pagamentos.Count > 0
                && pagamentos.ElementAt(0).Condicao.IsCheque)
            {
                View.MakeQuestion("Deseja desfazer a sele玢o do pagamento?", () => DoSelecionarCondicao(condicao), () => { });
                return;
            }
            DoSelecionarCondicao(condicao);
        }

        private void DoSelecionarCondicao(CondicaoPagamentoDto condicao)
        {
            var pagamentos = new List<PagamentoDto>(PedidoCorrente.Pagamentos);
            pagamentos.Clear();
            PedidoCorrente.Pagamentos = pagamentos;
            if (condicao.IsCheque)
            {
                pagamentos.Add(new PagamentoChequeDto { ValorFinal = 0, Condicao = condicao });
                _pagamentoView.PermitirInfoCheque();
                return;
            }
            _pagamentoView.DesabilitarInfoCheque();
            pagamentos.Add(new PagamentoDto { ValorFinal = ValorTotalPedido, Condicao = condicao });

            PedidoCorrente.Pagamentos = pagamentos;
        }

        public bool AdicionarInfoCheque(Guid uid, string numero, string agencia, string banco, decimal valor)
        {
            if (String.IsNullOrEmpty(numero)
                || String.IsNullOrEmpty(agencia)
                || String.IsNullOrEmpty(banco)
                || valor <= 0)
            {
                View.ShowModalMessage("Aten玢o", "Valores informados inv醠idos", null);
                return false;
            }

            var pagamentos = new List<PagamentoDto>(PedidoCorrente.Pagamentos);
            var valorTotalCheque = pagamentos.Sum(p => p.ValorFinal);

            // Verificar valores dos cheques.
            if ((valorTotalCheque + Convert.ToDouble(valor)) > ValorTotalPedido)
            {
                View.ShowModalMessage("Aten玢o", "Valores informados ultrapassam o valor do pedido", null);
                return false;
            }

            var cheque = new PagamentoChequeDto();
            if (pagamentos.Count == 1 && !(pagamentos.First() as PagamentoChequeDto).Uid.HasValue)
            {
                cheque = pagamentos.First() as PagamentoChequeDto;
            }
            else
            {
                pagamentos.Add(cheque);
            }
            cheque.Uid = uid;
            cheque.Numero = numero;
            cheque.Agencia = agencia;
            cheque.Banco = banco;
            cheque.ValorFinal = Convert.ToDouble(valor);
            _pagamentoView.AtualizarValorCheque(pagamentos.Sum(p => p.ValorFinal));
            PedidoCorrente.Pagamentos = pagamentos;
            return true;
        }

        public bool RemoverInfoCheque(Guid uid)
        {
            var pagamentos = new List<PagamentoDto>(PedidoCorrente.Pagamentos);
            var cheque = pagamentos.Cast<PagamentoChequeDto>().FirstOrDefault(p => p.Uid == uid);
            if (cheque != null)
            {
                return pagamentos.Remove(cheque);
            }
            return false;
        }

        public void EditarInfoCheque(Guid uid, string numero, string agencia, string banco, decimal valor)
        {
            var pagamentos = new List<PagamentoDto>(PedidoCorrente.Pagamentos);
            if (pagamentos.Count > 0)
            {
                var cheque = pagamentos
                    .Cast<PagamentoChequeDto>()
                    .FirstOrDefault(p => p.Uid.Value == uid);
                if (cheque != null)
                {
                    cheque.Numero = numero;
                    cheque.Agencia = agencia;
                    cheque.Banco = banco;
                    cheque.ValorFinal = Convert.ToDouble(valor);
                }
            }
        }

        public void ConcluirSelecaoCondicao(Action ok)
        {
            var pagamentos = new List<PagamentoDto>(PedidoCorrente.Pagamentos);
            if (pagamentos.Count > 0 && pagamentos.ElementAt(0).Condicao.IsCheque)
            {
                var valorPagamento = pagamentos
                    .Sum(p => p.ValorFinal);
                if (valorPagamento != ValorTotalPedido)
                {
                    View.ShowModalMessage("Aten玢o", "Valores indicados do cheque inv醠idos", null);
                    return;
                }
            }
            ok();
        }

        private void CalcularValores()
        {
			var valorTotalPedidoSemDesconto = PedidoCorrente.Produtos.Sum(p => p.ValorUnitario * Convert.ToDouble(p.QuantidadePedido));
			var valorTotalDesconto = PedidoCorrente.Produtos.Sum(p => (p.Desconto / 100) * (p.ValorUnitario * Convert.ToDouble(p.QuantidadePedido))); ;
            var diferencaValor = valorTotalPedidoSemDesconto - valorTotalDesconto;

            View.AtualizarValores(valorTotalPedidoSemDesconto, valorTotalDesconto, diferencaValor);
        }

        public bool AtualizarDesconto(ProdutoDto produto, decimal desconto, out ProdutoDto produtoAtualizado)
        {
            var produtoPedido = _produtosSelecionados.FirstOrDefault(p => p.Id == produto.Id);
            produtoAtualizado = null;
            if (produtoPedido != null)
            {
                produtoPedido.Desconto = Convert.ToDouble(desconto);
                AtualizarValorTotal(produtoPedido);
                //View.AtualizarProdutosSelecionados(_produtosSelecionados); // TODO: Verificar esta atualiza玢o
                produtoAtualizado = produtoPedido;
                CalcularValores();
                return true;
            }
            return false;
        }

        private void AtualizarValorTotal(ProdutoDto produto)
        {
			produto.ValorTotalPedido = produto.ValorUnitario * Convert.ToDouble(produto.QuantidadePedido);
            var valorDesconto = produto.ValorTotalPedido * (produto.Desconto/100);
            produto.ValorTotalPedido = produto.ValorTotalPedido - valorDesconto;
        }
    }
}