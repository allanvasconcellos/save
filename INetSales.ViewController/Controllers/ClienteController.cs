using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Sync;
using INetSales.ViewController.Sync.Integrators;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public class ClienteController : BaseController<IClienteView>
    {
        public ClienteDto CurrentCliente { get; private set; }
        public StatusDetailsView CurrenteStatusView { get; private set; }
        private List<ClienteDto> _firstList;

        private IClienteDetailView _detailView;

        public ClienteController(IClienteView view, IApplication application)
            : base(view, application, null)
        {
            CurrenteStatusView = StatusDetailsView.New;
            _firstList = new List<ClienteDto>();
        }

        #region Internal Methods
            
        internal void InitializeListView()
        {
            try
            {
                View.Initialize(this);
                View.UpdateViewTitle(CurrentTitle);
                //AtualizarTodos();
            }
            catch(Exception ex)
            {
                if(ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
            }
        }

        internal void InitializeDetail(IClienteDetailView detail)
        {
            _detailView = detail;
            _detailView.Initialize(this);
            View.UpdateViewTitle(CurrentTitle);
            //_detailView.ShowRamoList(ramoDb.GetAll());
            if (CurrenteStatusView == StatusDetailsView.New)
            {
                //((IClienteNewView)_detailView).InitializeNew(this, CurrentCliente);
            }
            else if (CurrenteStatusView == StatusDetailsView.Query)
            {
                detail.ShowDetail(CurrentCliente);
            }
            //else if (CurrenteStatusView == StatusDetailsView.Edit)
            //{
            //    ((IClienteEditView)_detailView).InitializeEdit(this, CurrentCliente);
            //}
        }

        #endregion

		public IEnumerable<ClienteDto> FiltrarCliente(string filtro, bool comRoteiro)
		{
			var db = DbHelper.GetOffline<IOfflineClienteDb>();
			var clientes = db.GetClientesByRazaoFiltro (filtro, Session.UsuarioLogado);
			if (clientes.Count () <= 0) {
				clientes = db.GetClientesByNomeFiltro (filtro, Session.UsuarioLogado);
			}
			return clientes.Where (c => c.HasRota == comRoteiro);
		}

        public void SelectCliente(ClienteDto cliente)
        {
            CurrentCliente = cliente;
            CurrenteStatusView = StatusDetailsView.Query;
            //if (cliente.IsPendingUpload)
            //{
            //    CurrenteStatusView = StatusDetailsView.Edit;
            //}
            View.ShowClienteDetail();
        }
			
        public void EnviarPedidoCliente(ClienteDto cliente)
        {
            // INET.033
            if (cliente.HasPendencia)
            {
                View.ShowModalMessage("Cliente", "Cliente tem pendências, não é permitido realizar pedidos.", () => { });
                return;
            }
//            if (cliente.HasRota)
//            {
//                View.ShowModalMessage("Cliente", "Cliente ja tem roteiro definido, selecione este cliente a partir do roteiro.", () => { });
//                return;
//            }
            CurrentCliente = cliente;
            View.ShowPedido();
            Logger.Info(false, "Cliente {0} enviado para realizar pedido", cliente.NomeFantasia);
        }

        public void UpdateClienteInfo(ClienteDto cliente)
        {
            if(CurrenteStatusView == StatusDetailsView.New)
            {
                CurrentCliente = cliente;
                return;
            }
            // TODO: Atualizar as informaçoes no CurrentCliente
        }

        public void CriarCliente()
        {
            // Verificar o status da listView.
            try
            {
                if (VerificarCliente())
                {
                    CurrentCliente.DataCriacao = DateTime.Now;
                    CurrentCliente.HasRota = false;
                    CurrentCliente.Usuario = Session.UsuarioLogado;
                    if(CurrentCliente.TipoPessoa == TipoPessoaEnum.Fisica)
                    {
                        CurrentCliente.RazaoSocial = CurrentCliente.NomeFantasia;
                    }
                    // Realizar envio
                    var progress = View.ShowProgressView("Enviando...");
                    var manager = new IntegratorManager();
                    manager.Enqueue(new ClienteUpload(CurrentCliente));
                    ExecuteOnBackgroundView(() => manager.Execute(new ProgressCompleteManager(progress), 
                        integrator =>
                            {
                                View.ExecuteOnUI(() =>
                                    {
                                        if (integrator.HasError)
                                        {
                                            View.ShowModalMessage(
                                                "Cadastro cliente",
                                                "Ocorreu um erro ao cadastrar o cliente");
                                        }
                                        else
                                        {
                                            View.ShowMessage("Cliente salvo");
                                            View.MakeQuestion(
                                                "Deseja realizar um pedido para este cliente?",
                                                () =>
                                                    {
                                                        EnviarPedidoCliente(CurrentCliente);
                                                        View.CloseView();
                                                    },
                                                () => View.CloseView());
                                            FecharCadastro();
                                        }
                                    });
                            }), 
                        ex => {});
                }
            }
            catch(Exception ex)
            {
                if(ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
                View.ShowModalMessage("Erro cadastro cliente", "Ocorreu um erro ao salvar o cliente", () => { });
            }
        }

        private bool VerificarCliente()
        {
            var offline = DbHelper.GetOffline<IOfflineClienteDb>(); // TODO: Colocar o metodo de salvar na interface comum.
            if(String.IsNullOrEmpty(CurrentCliente.NomeFantasia))
            {
                View.ShowModalMessage("Validacao Cliente", String.Format("Nome {0} inválido", CurrentCliente.TipoPessoa == TipoPessoaEnum.Juridica ? " Fantasia " : String.Empty), 
                    () => { });
                return false;
            }
            if (CurrentCliente.TipoPessoa == TipoPessoaEnum.Juridica && String.IsNullOrEmpty(CurrentCliente.NomeFantasia))
            {
                View.ShowModalMessage("Validacao Cliente", String.Format("Razao social inválida"), () => { });
                return false;
            }
            if(     String.IsNullOrEmpty(CurrentCliente.Documento)
                || (CurrentCliente.TipoPessoa == TipoPessoaEnum.Fisica && !Utils.ValidarCpf(CurrentCliente.Documento))
                || (CurrentCliente.TipoPessoa == TipoPessoaEnum.Juridica && !Utils.ValidarCnpj(CurrentCliente.Documento)))
            {
                View.ShowModalMessage("Validacao Cliente", String.Format("Documento inválido"), () => { });
                return false;
            }
            if(offline.FindClienteByDocumento(CurrentCliente.Documento) != null)
            {
                View.ShowModalMessage("Validacao Cliente", String.Format("Documento já cadastrado para um cliente"), () => { });
                return false;
            }
            if (String.IsNullOrEmpty(CurrentCliente.Email) || !Utils.ValidarEmail(CurrentCliente.Email))
            {
                View.ShowModalMessage("Validacao Cliente", String.Format("Email inválido"), () => { });
                return false;
            }
            return true;
        }

        private void FecharCadastro()
        {
            CurrenteStatusView = StatusDetailsView.New;
        }

        /// <summary>
        /// Atualizar a lista de cliente, somente clientes que ainda não tem roteiro.
        /// </summary>
        public void AtualizarSemRoteiro()
        {
            var progress = View.ShowProgressView();
            ExecuteOnBackgroundView(() =>
            {
                var query = _firstList
                    .Where(c => !c.HasRota);
                var listView = (IClienteListView) View;
                if (query.Count() > 0)
                {
                    View.ExecuteOnUI(() => listView.ShowClienteList(query));
                }
                else
                {
                    View.ExecuteOnUI(listView.ShowClienteListEmpty);
                }
                progress.Close();
            });
        }

        public void AtualizarTodos()
        {
            var offline = DbHelper.GetOffline<IOfflineClienteDb>();
            _firstList = offline.GetAll(Session.UsuarioLogado)
                .OrderBy(c => c.RazaoSocial)
                .ToList();
            var listView = (IClienteListView)View;
            if (_firstList.Count() > 0)
            {
                listView.ShowClienteList(_firstList);
            }
            else
            {
                listView.ShowClienteListEmpty();
            }
        }
    }
}