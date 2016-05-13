using System;
using System.Linq;
using INetSales.Objects.Dtos;
using System.Collections.Generic;

namespace Save.LocalData.Repositories
{
	public class PendenciaRepository : BaseRepository<PendenciaDto>
	{
		public PendenciaRepository (IData<PendenciaDto> data)
			: base(data)
		{
		}

		public List<PendenciaDto> GetPendencias(ClienteDto cliente)
		{
			return GetAll (c => c.ClienteId == cliente.Id).ToList();
		}

		protected override void PreInsert (PendenciaDto dto)
		{
			base.PreInsert (dto);
			dto.ClienteId = dto.Cliente.Id;
		}
	}
}

