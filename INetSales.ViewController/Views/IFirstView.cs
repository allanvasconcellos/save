using INetSales.ViewController.Sync;

namespace INetSales.ViewController.Views
{
    public interface IFirstView : IView
    {
        void PrepararServico(IntegratorManager manager);

        IAcessoChildView GetAcesso();

        IConfiguracaoChildView GetFirstConfiguracao();
    }
}