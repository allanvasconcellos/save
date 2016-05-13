using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface.Dal
{
    public class ProdutoDal : BaseDal<ProdutoDto>, IOfflineProdutoDb
    {
        public ProdutoDal(SqliteConnection conn)
            : base(conn)
        {
            QueryField = "{0}Nome, {0}ValorUnitario, {0}GrupoId ";
            TableName = "TProduto {0}";
            PrimaryKey = "ProdutoId";
            FinalizeQueryField(FIELD_DATA_CRIACAO, FIELD_DATA_ALTERACAO, FIELD_IS_DESABILITADO);
        }

        protected override void DoMapDto(SqliteDataReader reader, ProdutoDto dto, int nextIndex)
        {
            //int qtdEstoqueIndex = reader.GetOrdinal("QuantidadeInicial");
            int qtdDisponivelIndex = reader.GetOrdinal("QuantidadeDisponivel");
            int qtdPedidaIndex = reader.GetOrdinal("QuantidadePedido");
            int descontoIndex = reader.GetOrdinal("DescontoPercent");
            var grupoDal = new GrupoDal(Connection);
            dto.Nome = reader.GetString(nextIndex++);
            dto.ValorUnitario = reader.GetDouble(nextIndex++);
            dto.Grupo = grupoDal.Find(reader.GetInt32(nextIndex++)) ?? new GrupoDto {Id = 1};

            // Nao fixa na constante
            dto.QuantidadePedido = GetValueOrNull<decimal>(reader, qtdPedidaIndex);
            dto.Desconto = GetValueOrNull<double>(reader, descontoIndex);
			dto.QuantidadeDisponivel = GetValueOrNull<decimal>(reader, qtdDisponivelIndex);
            dto.SaldoAtual = dto.QuantidadeDisponivel;
        }

        protected override void Insert(ProdutoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
            commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
            commandText.Append("VALUES ");
            commandText.Append("(" + PrefixInsertValues + ", @NOME, @VALOR_UNIT, @GRUPO) ");

            int newProdutoId = GetNextPkValue(PrimaryKey, GetTableName(String.Empty));

            SqliteParameter[] parameters = GetParameters(dto, newProdutoId,
                                                         new SqliteParameter("@NOME", dto.Nome),
                                                         new SqliteParameter("@VALOR_UNIT", dto.ValorUnitario),
                                                         new SqliteParameter("@GRUPO", dto.Grupo.Id)
                );

            if (ExecuteNonQuery(commandText.ToString(), parameters))
            {
                dto.Id = newProdutoId;
            }
        }

        protected override void Update(ProdutoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("UPDATE {0} SET ", GetTableName(String.Empty));
            commandText.Append(PrefixUpdateValues);
            commandText.Append(",Nome = @NOME");
            commandText.Append(",ValorUnitario = @VALOR_UNIT");
            commandText.Append(",GrupoId = @GRUPO");
            commandText.AppendFormat(" WHERE {0} = @ID", PrimaryKey);

            SqliteParameter[] parameters = GetParameters(dto, dto.Id,
                                                         new SqliteParameter("@NOME", dto.Nome),
                                                         new SqliteParameter("@VALOR_UNIT", dto.ValorUnitario),
                                                         new SqliteParameter("@GRUPO", dto.Grupo.Id)
                );

            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        #region Implementation of IDb<ProdutoDto>

        public IEnumerable<ProdutoDto> GetAll(UsuarioDto usuario)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0}, S.Saldo QuantidadeDisponivel ", GetQueryField("P"));
            query.AppendFormat("FROM {0} ", GetTableName("P"));
            query.Append("LEFT JOIN TProdutoSaldo S ON P.ProdutoId = S.ProdutoId ");
            query.Append("WHERE S.UsuarioId = @USUARIO ");
            if (usuario == null)
            {
                query.Append("Or S.UsuarioId is not NULL");
            }
            var parameters = new List<SqliteParameter>
                                 {
                                     new SqliteParameter("@USUARIO", usuario != null ? usuario.Id : 1),
                                 };
            return GetList(query.ToString(), parameters);
        }

        public IEnumerable<ProdutoDto> GetProdutosEstocados(UsuarioDto usuario, GrupoDto grupo = null)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0}, S.Saldo QuantidadeDisponivel ", GetQueryField("P"));
            query.AppendFormat("FROM {0} ", GetTableName("P"));
            query.Append("JOIN TProdutoSaldo S ON P.ProdutoId = S.ProdutoId ");
            query.Append("WHERE S.UsuarioId = @USUARIO ");
            query.Append("  AND S.Saldo > 0 ");
            var parameters = new List<SqliteParameter>
                                 {
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                 };
            if (grupo != null)
            {
                query.Append("AND P.GrupoId = @GRUPO ");
                parameters.Add(new SqliteParameter("@GRUPO", grupo.Id));
            }
            //query.AppendFormat("GROUP BY " + QueryField, "P.");
            return GetList(query.ToString(), parameters);
        }

        public IEnumerable<ProdutoDto> GetProdutos(PedidoDto pedido)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0}, PP.QuantidadePedido, PP.DescontoPercent, S.Saldo QuantidadeDisponivel ",
                               GetQueryField("P"));
            query.AppendFormat("FROM {0} ", GetTableName("P"));
            query.Append("JOIN TPedidoProduto PP ON P.ProdutoId = PP.ProdutoId ");
            query.Append("LEFT JOIN TProdutoSaldo S ON P.ProdutoId = S.ProdutoId ");
            query.Append("WHERE PP.PedidoId = @PEDIDO ");
            query.Append("AND S.UsuarioId = @USUARIO");
            var parameters = new List<SqliteParameter>
                                 {
                                     new SqliteParameter("@PEDIDO", pedido.Id),
                                     new SqliteParameter("@USUARIO", pedido.Usuario.Id),
                                 };
            return GetList(query.ToString(), parameters);
        }

		public bool InserirHistorico(ProdutoDto produto, UsuarioDto usuario, decimal quantidadeAntiga, decimal quantidadeNova,
                                     double valor, string motivo)
        {
            var commandText = new StringBuilder();
            commandText.Append("INSERT INTO TProdutoHistorico ");
            commandText.Append("(ProdutoId, UsuarioId, QuantidadeNova, QuantidadeAntiga, Valor, Motivo, DataCriacao) ");
            commandText.Append("VALUES ");
            commandText.Append(
                "(@PRODUTO, @USUARIO, @QUANTIDADE_NOVA, @QUANTIDADE_ANTIGA, @VALOR, @MOTIVO, @DATA_CRIACAO) ");

            var parameters = new List<SqliteParameter>
                                 {
                                     new SqliteParameter("@PRODUTO", produto.Id),
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                     new SqliteParameter("@QUANTIDADE_NOVA", quantidadeNova),
                                     new SqliteParameter("@QUANTIDADE_ANTIGA", quantidadeAntiga),
                                     new SqliteParameter("@VALOR", valor),
                                     new SqliteParameter("@MOTIVO", motivo),
                                     new SqliteParameter("@DATA_CRIACAO", DateTime.Now),
                                 };
            return ExecuteNonQuery(commandText.ToString(), parameters);
        }

        public void AtualizarSaldo(ProdutoDto produto, UsuarioDto usuario, decimal saldoAtualizado)
        {
            const string query = "SELECT 1 FROM TProdutoSaldo WHERE ProdutoId = @PRODUTO AND UsuarioId = @USUARIO ";
            var parameters = new List<SqliteParameter>
                                 {
                                     new SqliteParameter("@PRODUTO", produto.Id),
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                 };
            if (Exist(query, parameters))
            {
                var commandText = new StringBuilder();
                commandText.Append("UPDATE TProdutoSaldo ");
                commandText.Append("SET Saldo = @SALDO, DataAlteracao = @DATA_ALT ");
                commandText.Append("WHERE ProdutoId = @PRODUTO AND UsuarioId = @USUARIO ");

                parameters = new List<SqliteParameter>
                                 {
                                     new SqliteParameter("@PRODUTO", produto.Id),
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                     new SqliteParameter("@SALDO", saldoAtualizado),
                                     new SqliteParameter("@DATA_ALT", DateTime.Now),
                                 };
                ExecuteNonQuery(commandText.ToString(), parameters);
            }
            else
            {
                InserirSaldo(produto, usuario, saldoAtualizado);
            }
        }

        public IEnumerable<ProdutoDto> GetAll()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField("P"));
            query.AppendFormat("FROM {0} ", GetTableName("P"));
            return GetList(query.ToString());
        }

        private void InserirSaldo(ProdutoDto produto, UsuarioDto usuario, decimal novoSaldo)
        {
            var commandText = new StringBuilder();
            commandText.Append("INSERT INTO TProdutoSaldo ");
            commandText.Append("(ProdutoId, UsuarioId, Saldo, DataCriacao) ");
            commandText.Append("VALUES ");
            commandText.Append("(@PRODUTO, @USUARIO, @SALDO, @DATA_CRIACAO) ");

            var parameters = new List<SqliteParameter>
                                 {
                                     new SqliteParameter("@PRODUTO", produto.Id),
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                     new SqliteParameter("@SALDO", novoSaldo),
                                     new SqliteParameter("@DATA_CRIACAO", DateTime.Now),
                                 };
            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        #endregion
    }
}