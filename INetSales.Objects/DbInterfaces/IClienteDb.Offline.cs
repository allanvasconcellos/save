using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IOfflineClienteDb : IOfflineDb<ClienteDto>
    {
        IEnumerable<ClienteDto> GetClientes(RotaDto rota);

        ClienteDto FindClienteByDocumento(string documento);

		IEnumerable<ClienteDto> GetClientesByNomeFiltro (string nomeFiltro, UsuarioDto usuario);

		IEnumerable<ClienteDto> GetClientesByRazaoFiltro(string razaoFiltro, UsuarioDto usuario);
    }
}