using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using INetSales.Objects;
using System.Linq.Expressions;

namespace Save.LocalData
{
	public interface IData<T> : IDisposable where T: class, IDto, new()
	{
		IEnumerable<T> All(Func<T, bool> dataBound = null);

		T First(Expression<Func<T, bool>> predicate = null);

		T Last(Expression<Func<T, bool>> predicate = null);

		IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

		T Get(int id);

		T Get(string codigo);

		T Get(Expression<Func<T, bool>> predicate);

		bool Exist(Expression<Func<T, bool>> predicate);

		int Update(T item);

		int Delete(T item);

		int Add(T item);
	}
}

