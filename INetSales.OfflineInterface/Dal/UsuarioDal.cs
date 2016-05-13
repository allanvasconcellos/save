using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface.Dal
{
    public class UsuarioDal : DtoDal<UsuarioDto>, IOfflineUsuarioDb
    {
		public const string ID_COLUMN = "UsuarioId";
		public const string NOME_COLUMN = "Nome";
		public const string USERNAME_COLUMN = "Username";
		public const string SENHA_HASH_COLUMN = "SenhaHash";
		public const string PLACA_COLUMN = "PlacaVeiculo";
		public const string IS_ADM_COLUMN = "IsAdm";
		public const string CODIGO_SECUNDARIO_COLUMN = "CodigoSecundario";
		public const string IS_SYNC_PENDING = "IsSyncPending";

        public UsuarioDal(IDbContext context)
            : base(context)
        {
        }

        public override UsuarioDto FindByCodigo(string codigo)
        {
			var where = FluentParameter.Create()
				.Add("");
            var query = new StringBuilder();
			//query.AppendFormat("WHERE Upper(Codigo) = {0}", SqlReader.GetParamText(DtoMap.C));

			//parameters.Add(SqlReader.GetParamText("CODIGO"), codigo.ToUpper());
			return Context.GetObject(this, query.ToString(), parameters, (reader, d) => DtoMap(reader, d));
        }

        public UsuarioDto GetUsuario(string username)
        {
            var query = new StringBuilder();
			var parameters = new Dictionary<string, object>();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
			query.Append("WHERE Upper(Username) = {0}", SqlReader.GetParamText("USER_FILTRO"));

			parameters.Add(SqlReader.GetParamText("USER_FILTRO"), username.ToUpper());
			return Context.GetObject<UsuarioDto>(this, query.ToString(), parameters, (reader, d) => DtoMap(reader, d));
        }

        public List<UsuarioDto> GetUsuarios()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
			return Context.GetList<UsuarioDto>(this, query.ToString(), null, (reader, d) => DtoMap(reader, d));
        }

        public IEnumerable<UsuarioDto> GetAll(UsuarioDto usuario)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
			return Context.GetList<UsuarioDto>(this, query.ToString(), null, (reader, d) => DtoMap(reader, d));
        }

        public IEnumerable<UsuarioDto> GetAll()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
			return Context.GetList<UsuarioDto>(this, query.ToString(), null, (reader, d) => DtoMap(reader, d));
        }
    }
}