using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects
{
    public interface IOnlineDb
    {
    }

    public interface IOfflineDb
    {
    }

    public interface IDb<TDto>
        where TDto : IDto
    {
        IEnumerable<TDto> GetAll(UsuarioDto usuario = null);

        void Save(TDto dto);
    }

    public interface IOfflineDb<TDto> : IDb<TDto>, IOfflineDb
        where TDto : IDto
    {
        TDto Find(int id);

        TDto FindByCodigo(string codigo);
    }

    public interface IOfflineTransactionDb : IOfflineDb
    {
    }

    public interface IUploadDb : IOfflineDb
    {
        /// <summary>
        /// Retorna todos os objetos com pendencias de upload.
        /// </summary>
        IEnumerable<IUploader> GetUploadersWithPendind();
    }
}