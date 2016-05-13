using System;
using INetSales.Objects.Dtos;
using INetSales.Objects.DbInterfaces;

namespace Save.LocalData.Repositories
{
	public class IntegraRepository : BaseRepository<IntegraDto>, IOfflineIntegraDb
	{
		public IntegraRepository (IData<IntegraDto> data)
			: base(data)
		{
		}

		#region implemented abstract members of BaseRepository

		protected override void Map (IntegraDto dto)
		{
		}

		#endregion
	}
}

