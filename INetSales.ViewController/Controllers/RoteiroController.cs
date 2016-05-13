using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public partial class RoteiroController : BaseController<IRoteiroView>
    {
        public ClienteDto ClienteSelecionado { get; private set; }
        public DateTime DiaSelecionado { get; private set; }

        public RoteiroController(IRoteiroView view, IApplication application, IService service)
            : base(view, application, service)
        {
        }

        internal void InitializeView()
        {
            View.Initialize(this);
            View.UpdateViewTitle(CurrentTitle);
            SelectDia(DateTime.Now);
            //Service.Play();
        }

        internal void ResumeView()
        {
            SelectDia(DiaSelecionado);
        }

        public void AtualizarLista()
        {
            SelectDia(DiaSelecionado);
        }

        public void SelectNextDia()
        {
            DiaSelecionado = DiaSelecionado.AddDays(1);
            SelectDia(DiaSelecionado);
        }

        public void SelectPreviousDia()
        {
            DiaSelecionado = DiaSelecionado.AddDays(-1);
            SelectDia(DiaSelecionado);
        }

        public void SelectDia(DateTime dia)
        {
            try
            {
                dia = GetDiaValido(dia);
                RotaDto rota = GetRota(dia, Session.UsuarioLogado);
                var clientes = rota != null ? rota.Clientes
                    .Where(c => c.IsAtivoRoteiro) // Somente clientes ativos no roteiro.
                    .OrderBy(r => r.OrdemRoteiro)
                    .ToList() : new List<ClienteDto>();
                DiaSelecionado = dia;
                if (clientes.Count() > 0)
                {
                    View.ShowRoteiroList(dia, rota.IndicePasta, clientes);
                    return;
                }
                View.ShowRoteiroVazio(dia);
            }
            catch (Exception ex)
            {
                if (ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
            }
        }

        private DateTime GetDiaValido(DateTime dia)
        {
            if(dia.DayOfWeek == DayOfWeek.Saturday)
            {
                return dia.AddDays(2);
            }
            if(dia.DayOfWeek == DayOfWeek.Sunday)
            {
                return dia.AddDays(-2);
            }
            return dia;
        }

        public void SelectCliente(ClienteDto cliente)
        {
#if !DEBUG
            if (!DiaSelecionado.Day.Equals(DateTime.Today.Day) && !cliente.IsPermitidoForaDia)
            {
                View.ShowModalMessage("Cliente", "Não é permitido realizar pedido para este cliente fora do dia corrente", null);
                return;
            }
#endif

            // Verifica se o cliente está com pendencia.
            if (cliente.HasPendencia)
            {
                View.ShowModalMessage("Cliente", "Cliente está com pendencias de pagamento", null);
                return;
            }
            ClienteSelecionado = cliente;
            View.Next();
        }

        public void SelectClienteDetail(ClienteDto cliente)
        {
            if(View is IClienteView)
            {
                ApplicationController.ClienteController = new ClienteController(View as IClienteView, Application);
                ApplicationController.ClienteController.SelectCliente(cliente);
            }
        }


        private RotaDto GetRota(DateTime dia, UsuarioDto usuario)
        {
            var offline = DbHelper.GetOffline<IOfflineRotaDb>();
            var rota = offline.GetRota(dia, usuario);
            return rota;
        }

        protected override void DoClose(Action actionToClose)
        {
            View.MakeQuestion("Desejar sair?", actionToClose, () => { });
        }

        public void SelectClientePesquisa(ClienteDto cliente)
        {
            ApplicationController.RegisterClientePesquisa(cliente);
            View.ShowPesquisa();
        }
    }
}