using System;
using System.Linq;
using INetSales.Objects;
using System.Threading.Tasks;
using System.Collections.Generic;
using INetSales.Objects.Dtos;
using System.Linq.Expressions;
using System.IO;
using SQLite;

namespace Save.LocalData
{
	public class SqliteOrmData
	{
		public static void Initialize(string dbpath)
		{
			var db = new SQLiteConnection(dbpath);
			Logger.Info (false, "Iniciando mapeamento Sqlite ORM");
			db.CreateTable<UsuarioDto> ();
			db.CreateTable<CondicaoPagamentoDto> ();
			db.CreateTable<ConfiguracaoDto> ();
			db.CreateTable<IntegraDto> ();
			db.CreateTable<GrupoDto> ();
			db.CreateTable<ClienteDto> ();
			db.CreateTable<ClienteRotaDto> ();
			db.CreateTable<RotaDto> ();
			db.CreateTable<PendenciaDto> ();
			db.CreateTable<ProdutoDto> ();
			db.CreateTable<ProdutoSaldoDto> ();
			db.CreateTable<ProdutoPedidoDto> ();
			db.CreateTable<ProdutoHistoricoDto> ();
			db.CreateTable<PedidoDto> ();
			db.CreateTable<PagamentoDto> ();
			db.CreateTable<PagamentoChequeDto> ();
			Logger.Info (false, "Terminando mapeamento Sqlite ORM");
			// Primeira configuração
			if (db.Find<ConfiguracaoDto> (1) == null) {
				var configuracao = new ConfiguracaoDto ();
				configuracao.Id = 1;
				configuracao.UrlSiteERP = "http://inet.integratornet.com.br:8080/1.6.7/IntegraWS?wsdl";
				configuracao.UrlWebService = "http://inet.integratornet.com.br:8080/1.6.7/IntegraWS?wsdl";
				configuracao.IsPrimeiroAcesso = true;
				configuracao.CampoMarca = "displays";
				configuracao.CampoEspecie = "Kraft";
				db.Insert (configuracao);
			}
		}
	}

	public class SqliteOrmData<T> : IData<T>
		where T: class, IDto, new()
	{
		SQLiteConnection db;

		public SqliteOrmData(string dbpath)
		{
			this.db = new SQLiteConnection (dbpath);
		}

		public T First (Expression<Func<T, bool>> predicate = null)
		{
			if (predicate == null) {
				return db.Table<T> ().FirstOrDefault ();
			} else {
				return db.Table<T> ().FirstOrDefault (predicate.Compile());
			}
		}

		public T Last (Expression<Func<T, bool>> predicate = null)
		{
			if (predicate == null) {
				return db.Table<T> ().LastOrDefault ();
			} else {
				return db.Table<T> ().LastOrDefault (predicate.Compile());
			}
		}

		public bool Exist (Expression<Func<T, bool>> predicate)
		{
			return db.Table<T> ().All (predicate.Compile());
		}

		public IEnumerable<T> All (Func<T, bool> dataBound = null)
		{
			var lista = db.Table<T> ().ToList();
			var listToRemove = new List<T>();
			foreach (var l in lista) {
				if (dataBound != null && !dataBound(l))
				{
					listToRemove.Add(l);
				}
			}
			lista.RemoveAll(p => listToRemove.Exists(r => r.Id == p.Id));
			return lista;
		}

		public IEnumerable<T> Find (Expression<Func<T, bool>> predicate)
		{
			return db.Table<T> ()
					.Where (predicate)
					.ToList();
		}

		public T Get (int id)
		{
			return db.Find<T> (id);
		}

		public T Get (string codigo)
		{
			return db.Find<T> (m => m.Codigo.ToUpper ().Equals (codigo.ToUpper ()));
		}

		public T Get (Expression<Func<T, bool>> predicate)
		{
			return db.Find<T> (predicate);
		}

		public int Update (T item)
		{
			return db.Update (item);
		}

		public int Delete (T item)
		{
			return db.Delete (item);
		}

		public int Add (T item)
		{
			return db.Insert (item);
		}

		#region IDisposable implementation

		public void Dispose ()
		{
		}

		#endregion
	}
}

