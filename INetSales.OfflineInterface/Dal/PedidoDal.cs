using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface.Dal
{
    public class PedidoDal : BaseDal<PedidoDto>, IOfflinePedidoDb, IUploadDb
    {
        public PedidoDal(SqliteConnection conn)
            : base(conn)
        {
            QueryField =
				"{0}ClienteId, {0}UsuarioId, {0}RoteiroId, {0}NFe, {0}TipoPedido, {0}UrlHttpNFe, {0}UrlHttpBoleto, {0}UrlLocalNFe, {0}UrlLocalBoleto, {0}IsCancelado, {0}IsRejeitado, {0}OrdemCompra ";
            TableName = "TPedido {0}";
            PrimaryKey = "PedidoId";
            FinalizeQueryField(FIELD_IS_DESABILITADO);
        }

        protected override void DoMapDto(SqliteDataReader reader, PedidoDto dto, int nextIndex)
        {
            var clienteDal = new ClienteDal(Connection);
            var pagamentoDal = new PagamentoDal(Connection);
            var usuarioDal = new UsuarioDal(Connection);

            // Cliente
            var clienteId = GetValueOrNull<int>(reader, nextIndex++);
            if (clienteId > 0)
            {
                dto.Cliente = clienteDal.Find(clienteId);
            }
            ////
            dto.Usuario = usuarioDal.Find(reader.GetInt32(nextIndex++));
            //dto.Roteiro =  rotaDal.Find(reader.GetInt32(nextIndex++)); 
            nextIndex++; // Roteiro
            dto.NFe = GetValueOrNull<string>(reader, nextIndex++);
            dto.Tipo = GetTipoPedidoCode(reader.GetString(nextIndex++));
            dto.UrlHttpNFe = GetValueOrNull<string>(reader, nextIndex++);
            dto.UrlHttpBoleto = GetValueOrNull<string>(reader, nextIndex++);
            dto.UrlLocalNFe = GetValueOrNull<string>(reader, nextIndex++);
            dto.UrlLocalBoleto = GetValueOrNull<string>(reader, nextIndex++);
            dto.IsCancelado = GetValueOrNull<bool>(reader, nextIndex++);
            dto.IsRejeitado = GetValueOrNull<bool>(reader, nextIndex++);
			dto.OrdemCompra = GetValueOrNull<string>(reader, nextIndex++);

            dto.Pagamentos = pagamentoDal.GetPagamentos(dto);

            dto.HasCondicaoBoleto = dto.Pagamentos.Count() > 0 && dto.Pagamentos.Any(p => p.Condicao.IsBoleto);
        }

        protected override void Insert(PedidoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
            commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
            commandText.Append("VALUES ");
            commandText.Append("(" + PrefixInsertValues +
                               ", @CLIENTE, @USUARIO, @ROTEIRO, @NFE, @TIPO, @URL_HTTP_NFE, @URL_HTTP_BOLETO, @URL_LOCAL_NFE, @URL_LOCAL_BOLETO, @IS_CANCELADO, @IS_REJEITADO, @ORDEM_COMPRA )");
            int newPedidoId = GetNextPkValue();
            newPedidoId = newPedidoId > 400 ? newPedidoId : 400;
            SqliteParameter[] parameters = GetParameters(dto, newPedidoId,
                                                         new SqliteParameter("@CLIENTE",
                                                                             dto.Cliente != null
                                                                                 ? (object) dto.Cliente.Id
                                                                                 : DBNull.Value),
                                                         new SqliteParameter("@ROTEIRO",
                                                                             dto.Roteiro != null
                                                                                 ? (object) dto.Roteiro.Id
                                                                                 : DBNull.Value),
                                                         new SqliteParameter("@NFE", dto.NFe),
                                                         new SqliteParameter("@USUARIO", dto.Usuario.Id),
                                                         new SqliteParameter("@TIPO", GetTipoPedidoCode(dto.Tipo)),
                                                         new SqliteParameter("@URL_HTTP_NFE", dto.UrlHttpNFe),
                                                         new SqliteParameter("@URL_HTTP_BOLETO", dto.UrlHttpBoleto),
                                                         new SqliteParameter("@URL_LOCAL_NFE", dto.UrlLocalNFe),
                                                         new SqliteParameter("@URL_LOCAL_BOLETO", dto.UrlLocalBoleto),
                                                         new SqliteParameter("@IS_CANCELADO", dto.IsCancelado),
                                                         new SqliteParameter("@IS_REJEITADO", dto.IsRejeitado),
														 new SqliteParameter("@ORDEM_COMPRA", dto.OrdemCompra)
                );

            if (ExecuteNonQuery(commandText.ToString(), parameters))
            {
                dto.Id = newPedidoId;
            }
        }

        private string GetTipoPedidoCode(TipoPedidoEnum tipo)
        {
            switch (tipo)
            {
                case TipoPedidoEnum.Venda:
                    return "V";
                case TipoPedidoEnum.Bonificacao:
                    return "B";
                case TipoPedidoEnum.Remessa:
                    return "R";
                case TipoPedidoEnum.Sos:
                    return "S";
                default:
                    throw new InvalidOperationException();
            }
        }

        private TipoPedidoEnum GetTipoPedidoCode(string code)
        {
            switch (code)
            {
                case "V":
                    return TipoPedidoEnum.Venda;
                case "B":
                    return TipoPedidoEnum.Bonificacao;
                case "R":
                    return TipoPedidoEnum.Remessa;
                case "S":
                    return TipoPedidoEnum.Sos;
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override void Update(PedidoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("UPDATE {0} SET ", GetTableName(String.Empty));
            commandText.Append(PrefixUpdateValues);
            commandText.Append(",NFe = @NFE");
            commandText.Append(",IsRejeitado = @IS_REJEITADO");
            commandText.Append(",IsCancelado = @IS_CANCELADO");
            commandText.Append(",UrlHttpNFe = @URL_HTTP_NFE");
            commandText.Append(",UrlHttpBoleto = @URL_HTTP_BOLETO");
            commandText.Append(",UrlLocalNFe = @URL_LOCAL_NFE");
            commandText.Append(",UrlLocalBoleto = @URL_LOCAL_BOLETO");
			commandText.Append(",OrdemCompra = @ORDEM_COMPRA");
            commandText.AppendFormat(" WHERE {0} = @ID", PrimaryKey);

            SqliteParameter[] parameters = GetParameters(dto, dto.Id,
                                                         new SqliteParameter("@NFE", dto.NFe),
                                                         new SqliteParameter("@IS_REJEITADO", dto.IsRejeitado),
                                                         new SqliteParameter("@IS_CANCELADO", dto.IsCancelado),
                                                         new SqliteParameter("@URL_HTTP_NFE", dto.UrlHttpNFe),
                                                         new SqliteParameter("@URL_HTTP_BOLETO", dto.UrlHttpBoleto),
                                                         new SqliteParameter("@URL_LOCAL_NFE", dto.UrlLocalNFe),
                                                         new SqliteParameter("@URL_LOCAL_BOLETO", dto.UrlLocalBoleto),
														 new SqliteParameter("@ORDEM_COMPRA", dto.OrdemCompra)
                );

            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        #region Implementation of IDb<PedidoDto>

        public IEnumerable<PedidoDto> GetAll(UsuarioDto usuario)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE UsuarioId = @USUARIO ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                 };
            return GetList(query.ToString(), parameters);
        }

        public bool VerificarPedidoPendente()
        {
            const string query = "SELECT 1 FROM TPedido WHERE IsPendingUpload = @IS_PENDING_UPLOAD  ";
            var parameters = new[]
                                 {
                                     new SqliteParameter("@IS_PENDING_UPLOAD", true),
                                 };
            return Exist(query, parameters);
        }

        public IEnumerable<PedidoDto> GetPedidosPorTipo(UsuarioDto usuario, params TipoPedidoEnum[] tipoFiltro)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE UsuarioId = @USUARIO ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                 };
            if (tipoFiltro.Count() > 0)
            {
                query.Append(" AND (");
                foreach (TipoPedidoEnum tipoPedido in tipoFiltro)
                {
                    query.AppendFormat(" TipoPedido = '{0}' OR ", GetTipoPedidoCode(tipoPedido));
                }
                query.Remove(query.Length - 4, 4);
                query.Append(" ) ");
            }
            return GetList(query.ToString(), parameters);
        }

        public IEnumerable<PedidoDto> GetPedidosPorData(UsuarioDto usuario, DateTime dataInicio, DateTime dataFinal)
        {
            var query = new StringBuilder();
            DateTime hoje = DateTime.Today;
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE (DataCriacao >= @DATA_INICIAL AND DataCriacao <= @DATA_FINAL) AND UsuarioId = @USUARIO ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@USUARIO", usuario.Id),
                                     new SqliteParameter("@DATA_INICIAL", dataInicio),
                                     new SqliteParameter("@DATA_FINAL", dataFinal),
                                 };
            return GetList(query.ToString(), parameters);
        }

        public int GetTotalValorPagoBoleto(ProdutoDto produtoDto)
        {
            var query = new StringBuilder();
            query.Append("SELECT sum(ValorUnitarioVendido) * QuantidadePedido ");
            query.Append("FROM TPedidoProduto PPROD ");
            query.Append("JOIN TPedidoPagamento PPAG ON PPROD.PedidoId = PPAG.PedidoId ");
            query.Append("JOIN TCondicaoPagamento CP ON PPAG.CondicaoPagamentoId = CP.CondicaoId ");
            query.Append("WHERE ProdutoId = @PRODUTO AND CP.IsBoleto = @IS_BOLETO ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@PRODUTO", produtoDto.Id),
                                     new SqliteParameter("@IS_BOLETO", true),
                                 };
            return GetScalar<int>(query.ToString(), parameters);
        }

        public int GetTotalValorPagoCheque(ProdutoDto produtoDto)
        {
            var query = new StringBuilder();
            query.Append("SELECT sum(ValorUnitarioVendido) * QuantidadePedido ");
            query.Append("FROM TPedidoProduto PPROD ");
            query.Append("JOIN TPedidoPagamento PPAG ON PPROD.PedidoId = PPAG.PedidoId ");
            query.Append("JOIN TCondicaoPagamento CP ON PPAG.CondicaoPagamentoId = CP.CondicaoId ");
            query.Append("WHERE ProdutoId = @PRODUTO AND CP.Codigo = @CODIGO_PAG ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@PRODUTO", produtoDto.Id),
                                     new SqliteParameter("@CODIGO_PAG", "430"),
                                 };
            return GetScalar<int>(query.ToString(), parameters);
        }

        public int GetTotalValorPagoDinheiro(ProdutoDto produtoDto)
        {
            var query = new StringBuilder();
            query.Append("SELECT sum(ValorUnitarioVendido) * QuantidadePedido ");
            query.Append("FROM TPedidoProduto PPROD ");
            query.Append("JOIN TPedidoPagamento PPAG ON PPROD.PedidoId = PPAG.PedidoId ");
            query.Append("JOIN TCondicaoPagamento CP ON PPAG.CondicaoPagamentoId = CP.CondicaoId ");
            query.Append("WHERE ProdutoId = @PRODUTO AND CP.Codigo = @CODIGO_PAG ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@PRODUTO", produtoDto.Id),
                                     new SqliteParameter("@CODIGO_PAG", "294"),
                                 };
            return GetScalar<int>(query.ToString(), parameters);
        }

        public void InserirProdutoPedido(PedidoDto pedido, ProdutoDto produto)
        {
            var commandText = new StringBuilder();
            commandText.Append("INSERT INTO TPedidoProduto ");
            commandText.Append("      (PedidoId, ProdutoId, ValorUnitarioVendido, DescontoPercent, QuantidadePedido) ");
            commandText.Append("      VALUES ");
            commandText.Append("      (@PEDIDO, @PRODUTO, @VALOR_UNITARIO_VENDIDO, @DESCONTO, @QUANTIDADE_PEDIDO) ");

            var parameters = new[]
                                 {
                                     new SqliteParameter("@PEDIDO", pedido.Id),
                                     new SqliteParameter("@PRODUTO", produto.Id),
                                     new SqliteParameter("@VALOR_UNITARIO_VENDIDO", produto.ValorUnitario),
                                     new SqliteParameter("@DESCONTO", produto.Desconto),
                                     new SqliteParameter("@QUANTIDADE_PEDIDO", produto.QuantidadePedido)
                                 };

            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        public void InserirPagamentoPedido(PedidoDto pedido, PagamentoDto pagamento)
        {
            var dal = new PagamentoDal(Connection);
            pagamento.PedidoId = pedido.Id;
            dal.Save(pagamento);
        }

        public IEnumerable<PedidoDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public int GetQuantidadeTotalPedido(ProdutoDto produto, UsuarioDto usuario)
        {
            var query = new StringBuilder();
            var parameters = new List<SqliteParameter>();
            query.Append("SELECT SUM(QuantidadePedido) ");
            query.Append("FROM TPedidoProduto PP ");
            query.Append("JOIN TPedido P ON P.PedidoId = PP.PedidoId ");
            query.Append("WHERE PP.ProdutoId = @PRODUTO ");
            query.Append(" AND P.UsuarioId = @USUARIO ");
            parameters.Add(new SqliteParameter("@PRODUTO", produto.Id));
            parameters.Add(new SqliteParameter("@USUARIO", usuario.Id));
            return GetScalar<int>(query.ToString(), parameters);
        }

        #endregion

        #region Implementation of IUploadDb

        public IEnumerable<IUploader> GetUploadersWithPendind()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE IsPendingUpload = @IS_PENDING_UPLOAD ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@IS_PENDING_UPLOAD", true),
                                 };
            return GetList(query.ToString(), parameters)
                .Cast<IUploader>();
        }

        #endregion
    }
}