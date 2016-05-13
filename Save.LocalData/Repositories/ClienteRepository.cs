using System;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;
using System.Collections.Generic;
using INetSales.Objects;
using System.Linq;

namespace Save.LocalData.Repositories
{
	public class ClienteRepository : BaseRepository<ClienteDto>, IOfflineClienteDb, IUploadDb
	{
		public ClienteRepository (IData<ClienteDto> data)
			: base(data)
		{
		}

		public IEnumerable<ClienteDto> GetClientes (RotaDto rota)
		{
			var dataClienteRota = FactoryOffline.GetData<ClienteRotaDto> ();
			var roteiros = dataClienteRota.Find (c => c.RotaId == rota.Id);
			List<ClienteDto> clientes = new List<ClienteDto> ();
			foreach (var roteiro in roteiros) {
				var cliente = Find (roteiro.ClienteId);
				cliente.OrdemRoteiro = roteiro.OrdemRoteiro;
				cliente.IsAtivoRoteiro = roteiro.IsAtivoRoteiro;
				cliente.HasPedidoRoteiro = roteiro.HasPedidoRoteiro;
				cliente.IsPermitidoForaDia = roteiro.IsPermitidoForaDia;
				cliente.RoteiroCorrente = new RotaDto
				{
					Id = rota.Id,
					Nome = rota.Nome,
					Dia = rota.Dia,
					DiaPasta = rota.DiaPasta,
					Bloco = rota.Bloco,
					UsuarioId = rota.UsuarioId,
					Usuario = rota.Usuario,
					IndicePasta = rota.IndicePasta,
				};
				clientes.Add (cliente);
			}
			return clientes;
		}

		public IEnumerable<ClienteDto> GetClientesByNomeFiltro (string nomeFiltro, UsuarioDto usuario)
		{
			var rotaRepository = new RotaRepository(FactoryOffline.GetData<RotaDto>());
			IEnumerable<ClienteDto> clientes;
			if (String.IsNullOrEmpty (nomeFiltro)) {
				clientes = data.Find (c => c.UsuarioId == usuario.Id);
			} else {
				clientes = data.Find(c => c.UsuarioId == usuario.Id && c.NomeFantasia.Contains(nomeFiltro));
			}
			foreach (var cliente in clientes) {
				Map (cliente);
				cliente.Roteiros = rotaRepository.GetRoteiros(cliente);
				cliente.RoteiroCorrente = cliente.Roteiros.FirstOrDefault () != null ? cliente.Roteiros.FirstOrDefault().Rota : null;
			}
			return clientes;
		}

		public IEnumerable<ClienteDto> GetClientesByRazaoFiltro (string razaoFiltro, UsuarioDto usuario)
		{
			var rotaRepository = new RotaRepository(FactoryOffline.GetData<RotaDto>());
			IEnumerable<ClienteDto> clientes;
			if (String.IsNullOrEmpty (razaoFiltro)) {
				clientes = data.Find (c => c.UsuarioId == usuario.Id);
			} else {
				clientes = data.Find(c => c.UsuarioId == usuario.Id && c.RazaoSocial.Contains(razaoFiltro));
			}
			foreach (var cliente in clientes) {
				Map (cliente);
				cliente.Roteiros = rotaRepository.GetRoteiros(cliente).OrderByDescending(r => r.Rota.Dia).ToList();
				cliente.RoteiroCorrente = cliente.Roteiros.FirstOrDefault () != null ? cliente.Roteiros.FirstOrDefault().Rota : null;
			}
			return clientes;
		}

		public ClienteDto FindClienteByDocumento (string documento)
		{
			return Find (c => c.Documento == documento);
		}

		#region implemented members of BaseRepository

		public override IEnumerable<ClienteDto> GetAll (UsuarioDto usuario)
		{
			var all = data.Find (c => c.UsuarioId == usuario.Id);
			foreach (var d in all) {
				Map (d);
			}
			return all;
		}

		protected override void Map (ClienteDto dto)
		{
			var usuarioRepository = new UsuarioRepository (FactoryOffline.GetData<UsuarioDto> ());
			var pendenciaRepository = new PendenciaRepository (FactoryOffline.GetData<PendenciaDto> ());
			dto.Usuario = usuarioRepository.Find (dto.UsuarioId);
			dto.Pendencias = pendenciaRepository.GetPendencias (dto);
		}

		protected override void PreInsert (ClienteDto dto)
		{
			base.PreInsert (dto);
			dto.UsuarioId = dto.Usuario.Id;
		}

		protected override void PreUpdate (ClienteDto dto)
		{
			base.PreInsert (dto);
			dto.UsuarioId = dto.Usuario.Id;
		}

		protected override void PosInsert (ClienteDto dto)
		{
			base.PosInsert (dto);
			var pendenciaRepository = new PendenciaRepository (FactoryOffline.GetData<PendenciaDto> ());
			foreach (var pendencia in dto.Pendencias)
			{
				var pendenciaLocal = pendenciaRepository.FindByCodigo(pendencia.Codigo);
				pendencia.Cliente = dto;
				pendencia.ClienteId = dto.Id;
				if (pendenciaLocal != null)
				{
					pendencia.Id = pendenciaLocal.Id;
				}
				pendenciaRepository.Save(pendencia);
			}
		}

		#endregion

		#region IUploadDb implementation

		public IEnumerable<IUploader> GetUploadersWithPendind ()
		{
			return GetAll (p => p.IsPendingUpload == true)
				.Cast<IUploader>();
		}

		#endregion
	}
}

