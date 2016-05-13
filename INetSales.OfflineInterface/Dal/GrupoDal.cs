using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface.Dal
{
    public class GrupoDal : BaseDal<GrupoDto>, IOfflineGrupoDb
    {
        public GrupoDal(SqliteConnection conn)
            : base(conn)
        {
            QueryField = "{0}Nome, {0}GrupoPaiId";
            TableName = "TGrupo {0}";
            PrimaryKey = "GrupoId";
            FinalizeQueryField(FIELD_DATA_CRIACAO, FIELD_DATA_ALTERACAO, FIELD_IS_DESABILITADO);
        }

        protected override void DoMapDto(SqliteDataReader reader, GrupoDto dto, int nextIndex)
        {
            dto.Nome = reader.GetString(nextIndex++);
            dto.GrupoPai = !reader.IsDBNull(nextIndex) ? Find(reader.GetInt32(nextIndex++)) : null;

            dto.IsSubgrupo = dto.GrupoPai != null;
        }

        protected override void Insert(GrupoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
            commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
            commandText.Append("VALUES ");
            commandText.Append("(" + PrefixInsertValues + ", @NOME, @GRUPO_PAI) ");

            int newGrupoId = GetNextPkValue(PrimaryKey, GetTableName(String.Empty));

            SqliteParameter[] parameters = GetParameters(dto, newGrupoId,
                                                         new SqliteParameter("@NOME", dto.Nome),
                                                         new SqliteParameter("@GRUPO_PAI",
                                                                             dto.GrupoPai != null
                                                                                 ? (object) dto.GrupoPai.Id
                                                                                 : DBNull.Value)
                );

            if (ExecuteNonQuery(commandText.ToString(), parameters))
            {
                dto.Id = newGrupoId;
            }
        }

        protected override void Update(GrupoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("UPDATE {0} SET ", GetTableName(String.Empty));
            commandText.Append(PrefixUpdateValues);
            commandText.Append(",Nome = @NOME");
            commandText.Append(",GrupoPaiId = @GRUPO_PAI");
            commandText.AppendFormat(" WHERE {0} = @ID", PrimaryKey);

            SqliteParameter[] parameters = GetParameters(dto, dto.Id,
                                                         new SqliteParameter("@NOME", dto.Nome),
                                                         new SqliteParameter("@GRUPO_PAI",
                                                                             dto.GrupoPai != null
                                                                                 ? (object) dto.GrupoPai.Id
                                                                                 : DBNull.Value)
                );

            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        #region Implementation of IDb<GrupoDto>

        public IEnumerable<GrupoDto> GetAll(UsuarioDto usuario)
        {
            var query = new StringBuilder();
            var produtoDal = new ProdutoDal(Connection);
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            return GetList(query.ToString(), g =>
                                                 {
                                                     //g.Produtos = produtoDal.GetProdutosEstocados(usuario, g);
                                                     //return g.Produtos.Count() > 0;
                                                     return true;
                                                 });
        }

        public GrupoDto GetGrupoDefault()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            return GetObject(query.ToString(), new SqliteParameter[] {});
        }

        public IEnumerable<GrupoDto> GetGruposEstocados(UsuarioDto usuario)
        {
            var query = new StringBuilder();
            var produtoDal = new ProdutoDal(Connection);
            var pedidoDal = new PedidoDal(Connection);
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            return GetList(query.ToString(),
                           g =>
                               {
                                   g.Produtos = produtoDal.GetProdutosEstocados(usuario, g);
                                   foreach (ProdutoDto produto in g.Produtos)
                                   {
                                       produto.QuantidadeTotalPedido = pedidoDal.GetQuantidadeTotalPedido(produto,
                                                                                                          usuario);
                                   }
                                   return g.Produtos.Count() > 0;
                               });
        }

        public IEnumerable<GrupoDto> GetAll()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            return GetList(query.ToString(), g =>
                                                 {
                                                     //g.Produtos = produtoDal.GetProdutos(g);
                                                     //return g.Produtos.Count() > 0;
                                                     return true;
                                                 });
        }

        #endregion
    }
}