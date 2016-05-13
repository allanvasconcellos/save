using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects;
using INetSales.Objects.Dtos;

namespace INetSales.OfflineInterface
{
	public class DtoDal<TDto>
		where TDto : IDto, new()
    {
		protected IDbContext<TDto> Context { get; private set; } 

		public DtoDal(IDbContext<TDto> context)
		{
			this.Context = context;
		}
    }

    
}