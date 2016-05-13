using System;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Sync;
using INetSales.ViewController.Sync.Integrators;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public partial class RoteiroController
    {
        public void AtualizarVersao()
        {
            View.MakeQuestion("Deseja atualizar a versão?", () =>
            {
                var package = DbHelper.GetOnline<IPackageDb>();
                var configuracaoDb = DbHelper.GetOfflineConfiguracaoDb();
                var configuracao = configuracaoDb.GetConfiguracaoAtiva();
                string url = String.Empty;
                string lastVersion = String.Empty;
                string packagePath = String.Empty;
                if (
                    !package.TryGetUrlAndroidPackage(
                        configuracao.CurrentVersion, out url,
                        out lastVersion))
                {
                    View.ShowModalMessage("Versão",
                                        "A última versão já está atualizada",
                                        () => { });
                }
                var stream = Application.Download(url);
                Application.SaveStreamOnApplicationDisk(
                    "monodroid.inetsales.apk", stream, out packagePath);
                View.ExecuteInstall(packagePath);
                configuracao.CurrentVersion = lastVersion;
                configuracaoDb.Save(configuracao);
            }, null);
        }

        public void Logoff()
        {
            View.MakeQuestion("Deseja sair?", () =>
            {
                var login = new LoginController(View, null, Application);
                login.Logoff();
                //login.IniciarAcesso();
            }, null);
        }

        public void Configurar(IAcessoChildView acessoView)
        {
            var controller = new LoginController(View, acessoView, Application);
            controller.IniciarAcessoAdm(
                () =>
                {
                    var configuracaoView = View.GetConfiguracaoView();
                    var configuracaoDb = DbHelper.GetOffline<IOfflineConfiguracaoDb>();
                    var configuracao = configuracaoDb.GetConfiguracaoAtiva();
                    configuracaoView.Show(configuracao, 
                        (chave, preco, cnpj, urlErp, marca, especie) =>
                            {
                                if (!String.IsNullOrEmpty(chave) && !String.IsNullOrEmpty(preco) &&
                                    !String.IsNullOrEmpty(cnpj) && !String.IsNullOrEmpty(urlErp))
                                {
                                    configuracao.ChaveIntegracao = chave;
                                    configuracao.CodigoTabelaPreco = preco;
                                    configuracao.CnpjEmpresa = cnpj;
                                    configuracao.UrlWebService = urlErp;
                                    configuracao.CampoMarca = marca;
                                    configuracao.CampoEspecie = especie;
                                    configuracaoDb.Save(configuracao);
                                    View.ShowMessage("Configuração salva com sucesso");
                                    return true;
                                }
                                return false;
                            }, null);
                });
        }

        #region Sincronização

        private void Sincronizar(string messageError, params Integrator[] syncs)
        {
            Sincronizar(messageError, null, syncs);
        }

        private void Sincronizar(string messageError, Action viewActionAfterSync, params Integrator[] syncs)
        {
            var manager = new IntegratorManager();
            var progressView = View.ShowProgressView("Carregando...");
            var consoleView = View.GetConsoleView("Sincronização - detalhes");
            //var logSaver = new LogConsoleSaver(consoleView);
            foreach (var sync in syncs)
            {
                manager.Enqueue(sync);
            }
            ExecuteOnBackgroundView(() =>
            {
                //Logger.AddLog(logSaver);
                manager.Execute(new ProgressCompleteManager(progressView));
                //Logger.RemoveLog(logSaver);
                progressView.Close();
                if(manager.IsSemConexao)
                {
                    View.ExecuteOnUI(() => View.ShowModalMessage("Sincronização", "Sistema sem conexão"));
                    return;
                }
                else if(manager.HasTimeout)
                {
                    View.ExecuteOnUI(() => View.ShowModalMessage("Sincronização", "A comunicação expirou, tente mais tarde novamente"));
                    return;
                }
                else if(manager.AllErrors)
                {
                    View.ExecuteOnUI(() => View.ShowModalMessage("Sincronização", messageError));
                    return;
                }
                if(viewActionAfterSync != null)
                {
                    View.ExecuteOnUI(() => viewActionAfterSync());
                }
            });
        }

        private void Upload(string messageError, params Integrator[] uploads)
        {
            var manager = new IntegratorManager();
            var progressView = View.ShowProgressView("Enviando...");
            foreach (var upload in uploads)
            {
                manager.Enqueue(upload);
            }
            ExecuteOnBackgroundView(() =>
            {
                bool hasError = false;
                manager.Execute(new ProgressCompleteManager(progressView), integrator =>
                {
                    if (integrator.HasError)
                    {
                        hasError = true;
                    }
                });
                progressView.Close();
                if (hasError)
                {
                    View.ExecuteOnUI(() => View.ShowModalMessage("Envio", messageError));
                }
            });
        }

        public void SincronizarTodos()
        {
            View.MakeQuestion("Deseja sincronizar tudo?",
            () =>
                {
                    var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
                    Sincronizar("Ocorreu um erro ao sincronizar",
                                () => AtualizarLista(),
                                new UsuarioSync(configuracao),
                                new CondicaoPagamentoSync(configuracao),
                                new PedidoUpload(),
                                new GrupoSync(configuracao),
                                new ProdutoSync(configuracao),
                                //new RamoSync(configuracao),
                                new ClienteSync(configuracao),
                                new RotaSync(configuracao)
                                //new PesquisaSync()
                                );
                },
            () => { });
        }

        public void SincronizarUsuario()
        {
            Sincronizar("Ocorreu um erro ao carregar os usuários",
                new UsuarioSync(DbHelper.GetOfflineConfiguracaoAtiva()));
        }

        public void SincronizarProduto()
        {
            var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
            const string errorMessage = "Ocorreu um erro ao carregar os produtos";
            Sincronizar(errorMessage,
                new PedidoUpload(), // É necessario enviar os pedidos pendentes para atualizar o estoque no ERP.
                new GrupoSync(configuracao),
                new ProdutoSync(configuracao));
        }

        public void SincronizarCliente()
        {
            var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
            Sincronizar("Ocorreu um erro ao carregar os clientes",
                () => AtualizarLista(),
                new ClienteSync(configuracao));
        }

        public void SincronizarRota()
        {
            var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
            Sincronizar("Ocorreu um erro ao carregar a rota",
                new ClienteSync(configuracao),
                new RotaSync(configuracao));
        }

        public void SincronizarConfiguracao()
        {
            Sincronizar("Ocorreu um erro ao carregar a configuração",
                new ConfiguracaoSync());
        }

        public void ReenviarPendentes()
        {
            Upload("Ocorreu um erro ao enviar",
                new PedidoUpload());
        }

        #endregion
    }
}