using System.Collections.Generic;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Controllers;

namespace INetSales.ViewController.Views
{
    public interface IClienteView : IView
    {
        void Initialize(ClienteController controller);

        void ShowPedido();

        void ShowClienteDetail();
    }

    public interface IClienteListView : IClienteView
    {
        void ShowClienteList(IEnumerable<ClienteDto> clientes);

        void ShowClienteListEmpty();
    }

    public interface IClienteDetailView : IClienteView
    {
        void ShowDetail(ClienteDto cliente);

        void ShowRamoList(IEnumerable<RamoDto> ramos);
    }
}