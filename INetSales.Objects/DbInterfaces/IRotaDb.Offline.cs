using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IOfflineRotaDb : IOfflineDb<RotaDto>
    {
        /// <summary>
        /// Retorna o roteiro do dia para um usuário especifico.
        /// </summary>
        /// <returns></returns>
        RotaDto GetRota(DateTime dia, UsuarioDto usuario);

		IEnumerable<ClienteRotaDto> GetRoteiros(ClienteDto cliente);

        /// <summary>
        /// Indica no roteiro do cliente que foi feito um pedido.
        /// </summary>
        /// <param name="roteiro"></param>
        /// <param name="cliente"></param>
        void IndicarPedidoCliente(RotaDto roteiro, ClienteDto cliente);

        void InserirClienteRota(ClienteDto cliente, RotaDto rota);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="rota"></param>
        void AtualizarClienteRota(ClienteDto cliente, RotaDto rota);

        bool ExisteClienteRota(ClienteDto cliente, RotaDto rota);

        int GetUltimaPasta(UsuarioDto usuario);

        /// <summary>
        /// Retorna a ultima rota ordenada pelo dia.
        /// </summary>
        /// <param name="usuario">Filtra por rotas do usuário</param>
        /// <returns></returns>
        RotaDto GetUltimaRota(UsuarioDto usuario);

        RotaDto[] GetUltimoBloco(UsuarioDto usuario);

        void AtualizarBlocoStatus(RotaDto rota);

        void DesativarClienteNaRota(RotaDto rota, ClienteDto cliente);

        bool VerificarDesabilitadoNaRota(RotaDto rota, ClienteDto cliente);
    }
}