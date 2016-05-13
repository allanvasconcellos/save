using System;
using System.Collections.Generic;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public class PesquisaController : BaseController<IPesquisaView>
    {
        private ClienteDto _clientePesquisa;

        public PesquisaController(IPesquisaView listView, IApplication application, IService service) : base(listView, application, service)
        {
        }

        internal void Initialize(ClienteDto clientePesquisa)
        {
            _clientePesquisa = clientePesquisa;
            View.Initialize(this);

            // Atualizar o titulo
            string titleCliente = String.Format("{0}|Cliente: {1}", CurrentTitle, _clientePesquisa.RazaoSocial);
            View.UpdateViewTitle(titleCliente.Split('|'));
        }

        public void FinalizarPesquisa(Dictionary<TipoPesquisaPergunta, bool> perguntas, Dictionary<string, bool> categorias, Dictionary<string, double> precos)
        {
            try
            {
                // Enviar a pesquisa para o ERP.
                var online = DbHelper.GetOnline<IPesquisaDb>();
                online.EnviarPesquisa(Session.UsuarioLogado, _clientePesquisa, perguntas, categorias, precos);
                View.CloseView();
            }
            catch (OnlineException ex)
            {
                Logger.Error(ex);
                switch (ex.ReturnType)
                {
                    case OnlineReturnType.SemConexao:
                        View.ShowModalMessage("Problema ao enviar a pesquisa", "Sem conexão");
                        break;
                    case OnlineReturnType.Timeout:
                        View.ShowModalMessage("Problema ao enviar a pesquisa", "A comunicação expirou, tente reenviar o pedido mais tarde");
                        break;
                    default:
                        View.ShowModalMessage("Erro ao enviar a pesquisa", "Erro - Messagem: " + ex.Message);
                        break;
                }
                View.CloseView();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                View.ShowModalMessage("Erro", "Ocorreu um erro, favor contacte o administrador");
                View.CloseView();
            }
        }
    }
}