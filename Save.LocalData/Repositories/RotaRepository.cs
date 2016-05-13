using System;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;
using System.Collections.Generic;

namespace Save.LocalData.Repositories
{
	public class RotaRepository : BaseRepository<RotaDto>, IOfflineRotaDb
	{
		public RotaRepository (IData<RotaDto> data)
			: base(data)
		{
		}

		public override IEnumerable<RotaDto> GetAll (UsuarioDto usuario)
		{
			var all = data.Find (r => r.UsuarioId == usuario.Id);
			foreach (var d in all) {
				Map (d);
			}
			return all;
		}

		public IEnumerable<ClienteRotaDto> GetRoteiros (ClienteDto cliente)
		{
			var roteiroDb = FactoryOffline.GetData<ClienteRotaDto> ();
			var roteiros = roteiroDb.Find (r => r.ClienteId == cliente.Id);
			foreach (var roteiro in roteiros) {
				var rota = data.Get (roteiro.RotaId);
				roteiro.Rota = rota;
				Map(rota);
			}
			return roteiros;
		}

		public RotaDto GetRota (DateTime dia, UsuarioDto usuario)
		{
			var diaSemHora = new DateTime(dia.Year, dia.Month, dia.Day); // Remover as horas
			var diaMaisUm = diaSemHora.AddDays(1);
			var rota = Find(r => (r.Dia >= diaSemHora && r.Dia < diaMaisUm) && r.UsuarioId == usuario.Id); 
			if (rota != null)
			{
				var clienteRepository = new ClienteRepository(FactoryOffline.GetData<ClienteDto>());
				rota.Clientes = clienteRepository.GetClientes(rota);
			}
			return rota;
		}

		public void IndicarPedidoCliente (RotaDto rota, ClienteDto cliente)
		{
			var roteiroData = FactoryOffline.GetData<ClienteRotaDto> ();
			var roteiro = roteiroData.Get(r => r.ClienteId == cliente.Id && r.RotaId == rota.Id);
			roteiro.HasPedidoRoteiro = true;
			roteiroData.Update (roteiro);
		}

		public void InserirClienteRota (ClienteDto cliente, RotaDto rota)
		{
			var roteiro = new ClienteRotaDto ();
			var roteiroData = FactoryOffline.GetData<ClienteRotaDto> ();
			roteiro.OrdemRoteiro = cliente.OrdemRoteiro;
			roteiro.IsAtivoRoteiro = cliente.IsAtivoRoteiro;
			roteiro.RotaId = rota.Id;
			roteiro.ClienteId = cliente.Id;
			roteiro.IsPermitidoForaDia = cliente.IsPermitidoForaDia;
			roteiroData.Add(roteiro);
		}

		public void AtualizarClienteRota (ClienteDto cliente, RotaDto rota)
		{
			var roteiroData = FactoryOffline.GetData<ClienteRotaDto> ();
			var roteiro = roteiroData.Get(r => r.ClienteId == cliente.Id && r.RotaId == rota.Id);
			roteiro.OrdemRoteiro = cliente.OrdemRoteiro;
			roteiro.IsAtivoRoteiro = cliente.IsAtivoRoteiro;
			roteiro.IsPermitidoForaDia = cliente.IsPermitidoForaDia;
			roteiroData.Update (roteiro);
		}

		public bool ExisteClienteRota (ClienteDto cliente, RotaDto rota)
		{
			var roteiroData = FactoryOffline.GetData<ClienteRotaDto> ();
			return roteiroData.Exist(r => r.ClienteId == cliente.Id && r.RotaId == rota.Id);
		}

		public int GetUltimaPasta (UsuarioDto usuario)
		{
			var rota = Last (r => r.UsuarioId == usuario.Id);
			return rota.IndicePasta;
		}

		public RotaDto GetUltimaRota (UsuarioDto usuario)
		{
			return Last (r => r.UsuarioId == usuario.Id);
		}

		public RotaDto[] GetUltimoBloco (UsuarioDto usuario)
		{
			var blocos = new List<RotaDto>();
			RotaDto inicial = GetUltimoBlocoInicial(usuario);
			RotaDto final = GetUltimoBlocoFinal(usuario);
			if (inicial != null && final != null)
			{
				blocos.Add(inicial);
				blocos.Add(final);
			}
			return blocos.ToArray();
		}

		private RotaDto GetUltimoBlocoInicial(UsuarioDto usuario)
		{
			return Last (r => r.UsuarioId == usuario.Id && r.Bloco == BlocoStatusEnum.Inicial);
		}

		private RotaDto GetUltimoBlocoFinal(UsuarioDto usuario)
		{
			return Last (r => r.UsuarioId == usuario.Id && r.Bloco == BlocoStatusEnum.Final);
		}

		public void AtualizarBlocoStatus (RotaDto rota)
		{
			Save (rota);
		}

		public void DesativarClienteNaRota (RotaDto rota, ClienteDto cliente)
		{
			var roteiroData = FactoryOffline.GetData<ClienteRotaDto> ();
			var roteiro = roteiroData.Get(r => r.ClienteId == cliente.Id && r.RotaId == rota.Id);
			roteiro.IsAtivoRoteiro = false;
			roteiro.DataAlteracao = DateTime.Now;
			roteiroData.Update (roteiro);
		}

		public bool VerificarDesabilitadoNaRota (RotaDto rota, ClienteDto cliente)
		{
			var roteiroData = FactoryOffline.GetData<ClienteRotaDto> ();
			return roteiroData.Exist(r => r.ClienteId == cliente.Id && r.RotaId == rota.Id && r.IsAtivoRoteiro == false);
		}

		protected override void Map (RotaDto dto)
		{
			var usuarioRepository = new UsuarioRepository (FactoryOffline.GetData<UsuarioDto> ());
			dto.Usuario = usuarioRepository.Find (dto.UsuarioId);
		}

		protected override void PreInsert (RotaDto dto)
		{
			base.PreInsert (dto);
			dto.UsuarioId = dto.Usuario.Id;
		}

		protected override void PreUpdate (RotaDto dto)
		{
			base.PreInsert (dto);
			dto.UsuarioId = dto.Usuario.Id;
		}
	}
}

