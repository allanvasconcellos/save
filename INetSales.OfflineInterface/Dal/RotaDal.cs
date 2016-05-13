using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;
using INetSales.Objects;

namespace INetSales.OfflineInterface.Dal
{
    public class RotaDal : DtoDal<RotaDto>, IOfflineRotaDb
    {
		public const string ID_COLUMN = "RotaId";
		public const string DIA_COLUMN = "Dia";
		public const string NOME_COLUMN = "Nome";
		public const string PASTA_COLUMN = "Pasta";
		public const string USUARIO_COLUMN = "UsuarioId";
		public const string BLOCOSTATUS_COLUMN = "BlocoStatus";

		public RotaDal(IDbContext context) :
			base(context,
				ID_COLUMN, DATA_CRIACAO_COLUMN,
				DIA_COLUMN, NOME_COLUMN, PASTA_COLUMN, USUARIO_COLUMN, BLOCOSTATUS_COLUMN)
        {
			TableName = "TRota";
			PrimaryKey = ID_COLUMN;
        }

		protected override bool TryInsert(RotaDto dto, out int id)
		{
			var map = FluentParameter.Open(GetCommonInsertMap(dto), ID_COLUMN)
				.Add(USUARIO_COLUMN, dto.Usuario.Id)
				.Add(NOME_COLUMN, dto.Nome)
				.Add(PASTA_COLUMN, dto.IndicePasta)
				.Add(DIA_COLUMN, dto.Dia)
				//.Add(BLOCOSTATUS_COLUMN, GetCodeBloco(dto.Bloco))
				;
			id = map.GetId();
			long c = Context.Insert(this, map.Mapper());
			if(c > 0)
			{
				Logger.Info(true, false, "Inserido rota - Dia: {0} - Pasta: {1} - Nome: {2}", dto.Dia, dto.IndicePasta, dto.Nome);
				return true;
			}
			Logger.Info(true, false, "Nao inserido rota - Dia: {0} - Pasta: {1} - Nome: {2}", dto.Dia, dto.IndicePasta, dto.Nome);
			return false;
		}

		protected override void Update(RotaDto dto)
		{
			var map = FluentParameter.Open(GetCommonUpdateMap(dto), ID_COLUMN)
				.Add(USUARIO_COLUMN, dto.Usuario.Id)
				.Add(NOME_COLUMN, dto.Nome)
				.Add(PASTA_COLUMN, dto.IndicePasta)
				.Add(DIA_COLUMN, dto.Dia)
				//.Add(BLOCOSTATUS_COLUMN, GetCodeBloco(dto.Bloco))
				;
			if(Context.Update(this, map.Mapper(), dto.Id) > 0)
			{
				Logger.Info(true, false, "Atualizado rota - Dia: {0} - Pasta: {1} - Nome: {2}", dto.Dia, dto.IndicePasta, dto.Nome);
				return;
			}
			Logger.Info(true, false, "Nao atualizou rota - Dia: {0} - Pasta: {1} - Nome: {2}", dto.Dia, dto.IndicePasta, dto.Nome);
		}

		protected override bool DoEspecificMap(SqlReader reader, RotaDto dto)
		{
			var usuarioDb = new UsuarioDal(Context);
			int usuarioIndex = reader.GetColumnIndex(USUARIO_COLUMN);
			int nomeIndex = reader.GetColumnIndex(NOME_COLUMN);
			int pastaIndex = reader.GetColumnIndex(PASTA_COLUMN);
			int blocoIndex = reader.GetColumnIndex(BLOCOSTATUS_COLUMN);

			dto.Usuario = usuarioDb.Find(reader.GetInt(usuarioIndex));
			dto.Nome = reader.GetString(nomeIndex);
			dto.IndicePasta = reader.GetInt(pastaIndex);
			//dto.Bloco = GetBloco(reader.GetString(blocoIndex));
			dto.Dia = reader.GetValueOrDefault<DateTime>(reader.GetColumnIndex(DIA_COLUMN));
			return true;
		}

        #region Implementation of IDb<RotaDto>

        public IEnumerable<RotaDto> GetAll(UsuarioDto usuario)
        {
            var clienteDal = new ClienteDal(Context);
			var query = FluentQuery.Create()
				.AddSelect(GetQueryField(String.Empty))
				.AddFrom(GetTableName(String.Empty))
				.AddWhere("UsuarioId = {0} ", 
					FluentParameter.Create()
					.Add(USUARIO_COLUMN, usuario.Id)
					.Mapper());
			return Context.GetList<RotaDto>(this, query.CommandText, query.Parameters, 
				(reader, r) => {
					r.Clientes = clienteDal.GetClientes(r);
                });
        }

        public RotaDto GetRota(DateTime dia, UsuarioDto usuario)
        {
			var diaSemHora = new DateTime(dia.Year, dia.Month, dia.Day); // Remover as horas
			var query = FluentQuery.Create()
				.AddSelect(GetQueryField(String.Empty))
				.AddFrom(GetTableName(String.Empty))
				.AddWhere("(Dia >= {0} AND Dia < {1}) AND UsuarioId = {2} ", 
					FluentParameter.Create()
					.Add("DIA_INICIO", diaSemHora)
					.Add("DIA_FIM", diaSemHora.AddDays(1))
					.Add("USUARIO", usuario.Id)
					.Mapper());
			RotaDto roteiro = Context.GetObject<RotaDto>(this, query.CommandText, query.Parameters, 
				(reader, r) => {
					var clienteDal = new ClienteDal(Context);
					r.Clientes = clienteDal.GetClientes(r);
				});
            return roteiro;
        }

        public void IndicarPedidoCliente(RotaDto rota, ClienteDto cliente)
        {
			RotaClienteDal dal = new RotaClienteDal (Context);
			dal.IndicarPedidoCliente (rota, cliente);
        }

        public void InserirClienteRota(ClienteDto cliente, RotaDto rota)
        {
			RotaClienteDal dal = new RotaClienteDal (Context);
			dal.InserirClienteRota (rota, cliente);
        }

        public void AtualizarClienteRota(ClienteDto cliente, RotaDto rota)
        {
			RotaClienteDal dal = new RotaClienteDal (Context);
			dal.AtualizarClienteRota (rota, cliente);
        }

        public bool ExisteClienteRota(ClienteDto cliente, RotaDto rota)
        {
			RotaClienteDal dal = new RotaClienteDal (Context);
			dal.ExisteClienteRota (rota, cliente);
        }

        public int GetUltimaPasta(UsuarioDto usuario)
        {
			var query = FluentQuery.Create()
				.AddSelect("Pasta")
				.AddFrom(GetTableName(String.Empty))
				.AddWhere("UsuarioId = {0} ", 
					FluentParameter.Create()
					.Add(USUARIO_COLUMN, usuario.Id)
					.Mapper())
				.AddOrderDescending("Dia")
				;
            return Context.GetScalar<int>(query.CommandText, query.Parameters);
        }

        public RotaDto GetUltimaRota(UsuarioDto usuario)
        {
			var query = FluentQuery.Create()
				.AddSelect(GetQueryField(String.Empty))
				.AddFrom(GetTableName(String.Empty))
				.AddWhere("UsuarioId = {0} ", 
					FluentParameter.Create()
					.Add(USUARIO_COLUMN, usuario.Id)
					.Mapper())
				.AddOrderDescending("Dia")
				;
			return Context.GetObject<RotaDto>(query.CommandText, query.Parameters);
        }

        public void DesativarClienteNaRota(RotaDto rota, ClienteDto cliente)
        {
			RotaClienteDal dal = new RotaClienteDal (Context);
			dal.DesativarClienteNaRota (rota, cliente);
        }

        public bool VerificarDesabilitadoNaRota(RotaDto rota, ClienteDto cliente)
        {
			RotaClienteDal dal = new RotaClienteDal (Context);
			dal.VerificarDesabilitadoNaRota (rota, cliente);
        }

        #region Bloco

        public RotaDto[] GetUltimoBloco(UsuarioDto usuario)
        {
            var blocos = new List<RotaDto>();
            RotaDto inicial = GetUltimoBlocoInicial(usuario);
            RotaDto final = GetUltimoBlocoFinal(usuario);
            if (inicial != null && final != null)
            {
                blocos.Add(inicial);
                blocos.Add(final);
            }
            return blocos.ToArray();
        }

        public void AtualizarBlocoStatus(RotaDto rota)
        {
            var query = new StringBuilder();
            query.Append("UPDATE TRota SET ");
            query.Append("BlocoStatus = @BLOCO_STATUS ");
            query.Append("WHERE RotaId = @ROTA ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@BLOCO_STATUS",
                                                         rota.Bloco == BlocoStatusEnum.Inicial ? "I" : "F"),
                                     new SqliteParameter("@ROTA", rota.Id),
                                 };
            ExecuteNonQuery(query.ToString(), parameters);
        }

        private RotaDto GetUltimoBlocoInicial(UsuarioDto usuario)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE UsuarioId = @USUARIO AND BlocoStatus = @BLOCO_STATUS ");
            query.Append("ORDER BY Dia DESC ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                     new SqliteParameter("@BLOCO_STATUS", "I"),
                                 };
            return GetObject(query.ToString(), parameters);
        }

        private RotaDto GetUltimoBlocoFinal(UsuarioDto usuario)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE UsuarioId = @USUARIO AND BlocoStatus = @BLOCO_STATUS ");
            query.Append("ORDER BY Dia DESC ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                     new SqliteParameter("@BLOCO_STATUS", "F"),
                                 };
            return GetObject(query.ToString(), parameters);
        }

        #endregion

        public IEnumerable<RotaDto> GetAll()
        {
            var clienteDal = new ClienteDal(Connection);
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            return GetList(query.ToString(), r =>
                                                 {
                                                     r.Clientes = clienteDal.GetClientes(r);
                                                     return true;
                                                 });
        }

        #endregion
    }
}