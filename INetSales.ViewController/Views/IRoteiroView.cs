using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Controllers;

namespace INetSales.ViewController.Views
{
    public interface IRoteiroView : IView
    {
        void Initialize(RoteiroController controller);

        /// <summary>
        /// Prepara a lista de clientes referente ao roteiro do dia.
        /// </summary>
        /// <param name="dia"></param>
        /// <param name="indicePasta"></param>
        /// <param name="clientes"></param>
        void ShowRoteiroList(DateTime dia, int indicePasta, IEnumerable<ClienteDto> clientes);

        void ShowRoteiroVazio(DateTime dia);

        void ExecuteInstall(string packagePath);

        void ShowPesquisa();
    }
}