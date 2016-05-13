using System;
using System.Collections.Generic;
using INetSales.Objects;
using INetSales.OfflineInterface.Dal;

namespace INetSales.OfflineInterface
{
	internal interface IDbContext<TDto> : IDbSession
		where TDto : IDto, new()
    {
		TDto Find (int id, Action<SqlReader, TDto> dataBound = null);

		TDto FindByCodigo(string codigo, Action<TDto> dataBound = null);

        bool Exist(string query, IDictionary<string, object> parameters);

        int GetNextPkValue();

		void ExecuteNonQuery(string sql);

        TDto GetObject(IDictionary<string, object> where, Action<TDto> dataBound = null);

        TObject GetScalar<TObject>(string scalarField, IDictionary<string, object> where);

		List<TDto> GetList(IDictionary<string, object> where, Action<TDto> dataBound = null);

        long Insert(TDto dto);

        long Update(TDto dto);

        long Delete(int id);

        //long Delete(IDictionary<string, object> conditionParameter);
    }
}