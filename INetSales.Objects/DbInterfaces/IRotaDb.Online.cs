using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IRotaDb : IDb<RotaDto>, IOnlineDb
    {
        IEnumerable<Pasta> GetPastas(UsuarioDto usuario);
    }
}