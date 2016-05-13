using System;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public class ConfiguracaoController : BaseController<IConfiguracaoView>
    {
        private StatusDetailsView _status;
        private ConfiguracaoDto _configuracaoAtualizando;

        public ConfiguracaoController(IConfiguracaoView view, IApplication application)
            : base(view, application, null)
        {
            _status = StatusDetailsView.Indefinido;
        }

        internal void InitializeView()
        {
            var configuracaoDb = DbHelper.GetOfflineConfiguracaoDb();
            _configuracaoAtualizando = configuracaoDb.GetConfiguracaoAtiva();

            View.Initialize(this);
            View.UpdateViewTitle(CurrentTitle);
            if (_configuracaoAtualizando != null)
            {
                View.ShowConfiguracaoAtual(_configuracaoAtualizando);
                _status = StatusDetailsView.Edit;
            }
            else
            {
                View.ShowCamposVazios();
                _status = StatusDetailsView.New;
            }
        }

        public void Salvar(ConfiguracaoDto configuracaoInfo)
        {
            try
            {
                var configuracaoDb = DbHelper.GetOfflineConfiguracaoDb();
                if (_status == StatusDetailsView.Edit)
                {
                    _configuracaoAtualizando.UrlWebService = configuracaoInfo.UrlWebService;
                    _configuracaoAtualizando.ChaveIntegracao = configuracaoInfo.ChaveIntegracao;
                    _configuracaoAtualizando.CodigoTabelaPreco = configuracaoInfo.CodigoTabelaPreco;
                    _configuracaoAtualizando.UrlSiteERP = configuracaoInfo.UrlSiteERP;
                    _configuracaoAtualizando.IsIndiceInicialDiaModificado = false;
                    _configuracaoAtualizando.CnpjEmpresa = configuracaoInfo.CnpjEmpresa;
                    if (!_configuracaoAtualizando.IndiceInicialDia.Equals(configuracaoInfo.IndiceInicialDia))
                    {
                        _configuracaoAtualizando.IsIndiceInicialDiaModificado = true;
                    }
                    _configuracaoAtualizando.IndiceInicialDia = configuracaoInfo.IndiceInicialDia;
                }
                else
                {
                    _configuracaoAtualizando = configuracaoInfo;
                }
                configuracaoDb.Save(_configuracaoAtualizando);
                View.ShowMessage("Configuracao salva com sucesso");
            }
            catch (Exception ex)
            {
                if (ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
            }
        }
    }
}