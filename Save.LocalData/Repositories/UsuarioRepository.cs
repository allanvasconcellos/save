using System;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;
using System.Collections.Generic;

namespace Save.LocalData.Repositories
{
	public class UsuarioRepository : BaseRepository<UsuarioDto>, IOfflineUsuarioDb
	{
		public UsuarioRepository (IData<UsuarioDto> data)
			: base(data)
		{
		}

		#region implemented abstract members of BaseRepository

		protected override void Map (UsuarioDto dto)
		{
		}

		#endregion
	}
}

