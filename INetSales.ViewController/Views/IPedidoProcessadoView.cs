using INetSales.ViewController.Controllers;
using INetSales.Objects.Dtos;

namespace INetSales.ViewController.Views
{
    public interface IPedidoProcessadoView : IView
    {
        void Initialize(PedidoProcessadoController controller, ConfiguracaoDto configuracao);

        void ShowBoleto(string urlBoleto);

        void ShowNotaFiscal(string urlNfe);

		void ShowOC(string numero);
    }
}