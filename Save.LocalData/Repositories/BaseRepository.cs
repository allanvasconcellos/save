using System;
using INetSales.Objects;
using System.Collections.Generic;
using INetSales.Objects.Dtos;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Save.LocalData.Repositories
{
	public abstract class BaseRepository<TDto> : IOfflineDb<TDto>
		where TDto : class, IDto, new()
	{
		protected readonly IData<TDto> data;

		public BaseRepository (IData<TDto> data)
		{
			this.data = data;
		}

		public TDto Last (Expression<Func<TDto, bool>> predicate)
		{
			var d = data.Last (predicate);
			if (d != null) {
				Map (d);
			}
			return d;
		}

		public TDto Find (int id)
		{
			var d = data.Get (id);
			if (d != null) {
				Map (d);
			}
			return d;
		}

		public TDto Find (Expression<Func<TDto, bool>> predicate)
		{
			var d = data.Get (predicate);
			if (d != null) {
				Map (d);
			}
			return d;
		}

		public TDto FindByCodigo (string codigo)
		{
			var d = data.Get (codigo);
			if (d != null) {
				Map (d);
			}
			return d;
		}

		public virtual IEnumerable<TDto> GetAll (UsuarioDto usuario = null)
		{
			var all = data.All ();
			foreach (var d in all) {
				Map (d);
			}
			return all;
		}

		public virtual IEnumerable<TDto> GetAll (Expression<Func<TDto, bool>> predicate)
		{
			var all = data.Find (predicate);
			foreach (var d in all) {
				Map (d);
			}
			return all;
		}

		protected virtual void PreInsert (TDto dto) { }

		protected virtual void PreUpdate (TDto dto) { }

		protected virtual void PosInsert (TDto dto) { }

		public void Save (TDto dto)
		{
			if (dto.Id <= 0)
			{
				PreInsert (dto);
				dto.DataCriacao = DateTime.Now;
				data.Add(dto);
				PosInsert (dto);
				return;
			}
			dto.DataAlteracao = DateTime.Now;
			PreUpdate (dto);
			data.Update(dto);
		}

		protected virtual void Map (TDto dto) { }

	}
}

