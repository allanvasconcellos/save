using System;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;

namespace Save.LocalData.Repositories
{
	public class ConfiguracaoRepository : BaseRepository<ConfiguracaoDto>, IOfflineConfiguracaoDb
	{
		public ConfiguracaoRepository (IData<ConfiguracaoDto> data)
			: base(data)
		{
		}

		public ConfiguracaoDto GetConfiguracaoAtiva ()
		{
			return Find(c => c.IsDesabilitado == false);
		}

		#region implemented abstract members of BaseRepository

		protected override void Map (ConfiguracaoDto dto)
		{
		}

		#endregion
	}
}

