using System;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Save.LocalData;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Sync;
using INetSales.ViewController.Sync.Integrators;
using INetSales.ViewController.Views;

namespace INetSales.ViewController
{
    public static class ApplicationController
    {
        private static ClienteController _clienteController;
        public static ClienteController ClienteController {get { return _clienteController; } set { _clienteController = value; }}
        private static RoteiroController _roteiroController;
        private static PedidoController _pedidoController;
        private static IApplication _application;
        public static IApplication Application { get { return _application; } }
        private static IService _service;
        private const string CLIENTE_PESQUISA = "CLIENTE_PESQUISA";

        public static void Initialize(IApplication application)
        {
            _application = application;
        }

        public static void Initialize(IService service)
        {
            _service = service;
        }

        #region Initialize Views

        public static void Initialize(IFirstView view)
        {
			try
			{
	            var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
	            view.PrepararServico(new IntegratorManager());
	            var initial = new FirstController(view, _application, configuracao);
	            initial.Initialize();
			}
			catch(TypeInitializationException ex) {
				string messageError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				view.ShowModalMessage ("Inicialização Error", 
					String.Format ("Erro ao iniciar aplicação\n{0}", messageError),
					() => view.CloseView());
				
			}
        }

        public static void Initialize(IRoteiroView view)
        {
            _roteiroController = new RoteiroController(view, _application, _service);
            _roteiroController.InitializeView();
        }

        public static void Initialize(IPedidoView view)
        {
            _pedidoController = new PedidoController(view, _application);
            _pedidoController.InitializeViewPedidoRoteiro(_roteiroController.ClienteSelecionado, _roteiroController.DiaSelecionado);
        }

        public static void Initialize(IPedidoFinalizadoView view)
        {
            var controller = new PedidoProcessadoController(view, _application);
            controller.InitializeFinalizadoView();
        }

        public static void Initialize(IPesquisaView view)
        {
            var controller = new PesquisaController(view, _application, _service);
            var db = DbHelper.GetOffline<IOfflineClienteDb>();
            int clienteId = Convert.ToInt32(_application.GetValue(CLIENTE_PESQUISA));
            controller.Initialize(db.Find(clienteId));
        }

        public static void Initialize(IRemessaView view)
        {
            _pedidoController = new PedidoController(view, _application);
            _pedidoController.InitializeViewRemessa();
        }

        public static void Initialize(IPedidoClienteNovoView view)
        {
            _pedidoController = new PedidoController(view, _application);
            _pedidoController.InitializeViewClienteNovo(_clienteController.CurrentCliente);
        }

        public static void Initialize(IPedidoListaView view)
        {
            //var controller = new ListaPedidoController(view, _application);
            //controller.InitializeView();

            var c = new PedidoProcessadoController(view, _application);
            c.InitializeListaView();
        }

        public static void Initialize(IClienteListView listView)
        {
            //if (_clienteController == null)
            //{
                _clienteController = new ClienteController(listView, _application);
            //}
            _clienteController.InitializeListView();
        }

        public static void Initialize(IClienteDetailView view)
        {
            if (_clienteController == null)
            {
                _clienteController = new ClienteController(view, _application);
            }
            _clienteController.InitializeDetail(view);
        }

        public static void Initialize(IAcertoSaldoView view)
        {
            var controller = new AcertoSaldoController(view, _application);
            controller.InitializeView();
        }

        public static void Initialize(IConfiguracaoView view)
        {
            var controller = new ConfiguracaoController(view, _application);
            controller.InitializeView();
        }

		public static void Initialize(IRelatorioForcaVendaView view)
		{
			var controller = new RelatorioController(view, _application);
			controller.InitializeSinteticoForcaVenda();
		}

		public static void InitializeRelatorioPedidoOrcamento(IRelatorioPedidoOrcamentoView view)
		{
			var controller = new RelatorioController(view, _application);
			controller.InitializePedidoOrcamento();
		}

		public static void InitializeRelatorioFaturamentoCategoria(IRelatorioFaturamentoCategoriaView view)
		{
			var controller = new RelatorioController(view, _application);
			controller.InitializeFaturamentoCategoria();
		}

		public static void InitializeRelatorioClienteDevedores(IRelatorioClienteDevedoresView view)
		{
			var controller = new RelatorioController(view, _application);
			controller.InitializeClienteDevedores();
		}

        #endregion

        #region Resume Views

        public static void Resume(IFirstView view)
        {
            var login = new LoginController(view, view.GetAcesso(), _application);
            view.UpdateViewTitle();
            login.IniciarAcesso();
        }

        public static void Resume(IRoteiroView view)
        {
            _roteiroController.ResumeView();
        }

        #endregion

        #region Register

        internal static void RegisterClientePesquisa(ClienteDto cliente)
        {
            _application.SetValue(CLIENTE_PESQUISA, cliente.Id.ToString());
        }

        #endregion

        public static void Close()
        {
            Logger.Debug();
        }
    }
}