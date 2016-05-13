using System;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Sync;
using INetSales.ViewController.Sync.Integrators;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public class FirstController : BaseController<IFirstView>
    {
        public FirstController(IFirstView listView, IApplication application, ConfiguracaoDto configuracao)
            : base(listView, application, null)
        {
        }

        public void Initialize()
        {
            View.UpdateViewTitle();

            var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
            if(!configuracao.IsPrimeiroAcesso)
            {
                IniciarSistema(configuracao);
                return;
            }

            var manager = new IntegratorManager();
            var progress = View.ShowProgressView();
            manager.Enqueue(new ConfiguracaoSync());
            ExecuteOnBackgroundView(() =>
            {
                manager.Execute(new ProgressCompleteManager(progress));
                progress.Close();

                var db = DbHelper.GetOffline<IOfflineConfiguracaoDb>();
                configuracao = db.GetConfiguracaoAtiva();

                if (!TryConfigurarPrimeiroAcesso(configuracao, IniciarSistema))
                {
                    IniciarSistema(configuracao);
                }
            });


        }

        private void IniciarSistema(ConfiguracaoDto configuracao)
        {
            var login = new LoginController(View, View.GetAcesso(), Application);
            var manager = new IntegratorManager();
            var progress = View.ShowProgressView("Carregando...");
            manager.Enqueue(new UsuarioSync(configuracao));
            manager.Enqueue(new CondicaoPagamentoSync(configuracao));
            ExecuteOnBackgroundView(() =>
                {
                    manager.Execute(new ProgressCompleteManager(progress));
                    progress.Close();
                    login.IniciarAcesso();
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuracaoAtual"></param>
        /// <param name="inicioSistema"></param>
        /// <returns></returns>
        public bool TryConfigurarPrimeiroAcesso(ConfiguracaoDto configuracaoAtual, Action<ConfiguracaoDto> inicioSistema)
        {
            if (configuracaoAtual.IsPrimeiroAcesso)
            {
                var configuracaoView = View.GetFirstConfiguracao();
                var configuracaoDb = DbHelper.GetOffline<IOfflineConfiguracaoDb>();
				configuracaoView.ShowSelecaoTipoVenda (
					tipo => {
						configuracaoAtual.IsPreVenda = tipo == TipoVendaEnum.PreVenda;
					}, () => ShowConfiguracaoEmpresa(configuracaoAtual, configuracaoView, configuracaoDb, inicioSistema));
                
                return true;
            }
            return false;
        }

		private void ShowConfiguracaoEmpresa(ConfiguracaoDto configuracaoAtual, IConfiguracaoChildView configuracaoView, 
			IOfflineConfiguracaoDb configuracaoDb, Action<ConfiguracaoDto> inicioSistema)
		{
			configuracaoView.ShowSelecaoEmpresa(
				empresa =>
				{
					configuracaoAtual.UrlWebService = "http://inet.integratornet.com.br:8080/1.5/IntegraWS?wsdl";
					switch (empresa)
					{
					case EmpresaEnum.Lidimar:
						configuracaoAtual.ChaveIntegracao = "cd829da5-bcc4-47f4-97b4-3dad157eb419";
						configuracaoAtual.CodigoTabelaPreco = "939";
						configuracaoAtual.CnpjEmpresa = "09121078000139";
						break;
					case EmpresaEnum.TopChicle:
						configuracaoAtual.ChaveIntegracao = "bc356bb5-667e-48c3-ac82-2fc222dc3793";
						configuracaoAtual.CodigoTabelaPreco = "1627";
						configuracaoAtual.CnpjEmpresa = "05622093000100";
						break;
					case EmpresaEnum.Teste:
						configuracaoAtual.ChaveIntegracao = "c20c3912-39af-4149-a146-9050f6eece9b";
						configuracaoAtual.CodigoTabelaPreco = "4640";
						configuracaoAtual.CnpjEmpresa = "32493603000169";
						configuracaoAtual.CampoMarca = "Favorito";
						configuracaoAtual.CampoEspecie = "KG";
						configuracaoAtual.UsernameDebug = "aloisiog";
						configuracaoAtual.PasswordDebug = "1234";
						break;
					case EmpresaEnum.Producao:
						configuracaoAtual.ChaveIntegracao = "21da1f3a-d675-424f-b4cb-ced19951cbff";
						configuracaoAtual.CodigoTabelaPreco = "4296";
						configuracaoAtual.CnpjEmpresa = "32498750000121";
						configuracaoAtual.CampoMarca = "Faraó";
						configuracaoAtual.CampoEspecie = "KG";
						configuracaoAtual.UsernameDebug = "fernandon";
						configuracaoAtual.PasswordDebug = "123";
						break;
					case EmpresaEnum.Outros:
						configuracaoAtual.ChaveIntegracao = "213cab09-80d9-4dd8-8f8c-46a24841aca6";
						configuracaoAtual.CodigoTabelaPreco = "3525";
						configuracaoAtual.CnpjEmpresa = "16830052000125";
						break;
					}
				}, 
				() =>
				{
					configuracaoView.Show(configuracaoAtual, 
						(chave, preco, cnpj, urlErp, marca, especie) =>
						{
							if (!String.IsNullOrEmpty(chave) && !String.IsNullOrEmpty(preco) &&
								!String.IsNullOrEmpty(cnpj) && !String.IsNullOrEmpty(urlErp))
							{
								configuracaoAtual.ChaveIntegracao = chave;
								configuracaoAtual.CodigoTabelaPreco = preco;
								configuracaoAtual.CnpjEmpresa = cnpj;
								configuracaoAtual.UrlWebService = urlErp;
								configuracaoAtual.CampoMarca = marca;
								configuracaoAtual.CampoEspecie = especie;
								configuracaoAtual.IsPrimeiroAcesso = false;
								configuracaoDb.Save(configuracaoAtual);
								View.ShowMessage("Configuração salva com sucesso");
								return true;
							}
							return false;
						}, 
						() =>
						{
							inicioSistema(configuracaoAtual);
						});
				});
		}
    }
}