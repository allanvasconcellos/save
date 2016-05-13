using INetSales.ViewController.Controllers;

namespace INetSales.ViewController.Views
{
    public interface IPesquisaView : IView
    {
        void Initialize(PesquisaController controller);
    }
}