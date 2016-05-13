using System;
using System.Collections.Generic;
using System.Text;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;

namespace INetSales.OfflineInterface.Dal
{
    public class ConfiguracaoDal : BaseDal<ConfiguracaoDto>, IOfflineConfiguracaoDb
    {
        public ConfiguracaoDal(SqliteConnection conn)
            : base(conn)
        {
            QueryField =
				"{0}UrlWebService, {0}ChaveIntegracao, {0}CodigoTabelaPreco, {0}UrlSiteERP, {0}CurrentVersion, {0}IndiceInicialDia, {0}IsIndiceInicialDiaModificado, {0}IsPrimeiroAcesso, {0}CnpjEmpresa, {0}CampoEspecie, {0}CampoMarca, {0}IsPreVenda ";
            TableName = "TConfiguracao {0}";
            PrimaryKey = "ConfiguracaoId";
            FinalizeQueryField(FIELD_CODIGO, FIELD_DATA_ALTERACAO, FIELD_DATA_LAST_UPLOAD, FIELD_IS_PENDING_UPLOAD);
        }

        #region Overrides of BaseDal<ConfiguracaoDto>

        protected override void DoMapDto(SqliteDataReader reader, ConfiguracaoDto dto, int nextIndex)
        {
            dto.UrlWebService = reader.GetString(nextIndex++);
            dto.ChaveIntegracao = reader.GetString(nextIndex++);
            dto.CodigoTabelaPreco = reader.GetString(nextIndex++);
            dto.UrlSiteERP = GetValueOrNull<string>(reader, nextIndex++);
            dto.CurrentVersion = GetValueOrNull<string>(reader, nextIndex++);
            dto.IndiceInicialDia = GetValueOrNull<int>(reader, nextIndex++);
            dto.IsIndiceInicialDiaModificado = GetValueOrNull<bool>(reader, nextIndex++);
            dto.IsPrimeiroAcesso = GetValueOrNull<bool>(reader, nextIndex++);
            dto.CnpjEmpresa = GetValueOrNull<string>(reader, nextIndex++);
            dto.CampoEspecie = GetValueOrNull<string>(reader, nextIndex++);
            dto.CampoMarca = GetValueOrNull<string>(reader, nextIndex++);
			dto.IsPreVenda = GetValueOrNull<bool>(reader, nextIndex++);
        }

        protected override void Insert(ConfiguracaoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
            commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
            commandText.Append("VALUES ");
            commandText.Append("(" + PrefixInsertValues +
				", @URL_WEB_SERVICE, @CHAVE_INTEGRACAO, @CODIGO_TABELA_PRECO, @URL_SITE_ERP, @CURRENT_VERSION, @INDICE_INICIAL_DIA, @IS_INDICE_INICIAL_DIA_MODIFICADO, @IS_PRE_VENDA) ");

            int newConfiguracaoId = GetNextPkValue(PrimaryKey, GetTableName(String.Empty));

            SqliteParameter[] parameters = GetParameters(dto, newConfiguracaoId,
				new SqliteParameter("@URL_WEB_SERVICE", dto.UrlWebService),
				new SqliteParameter("@CHAVE_INTEGRACAO", dto.ChaveIntegracao),
				new SqliteParameter("@CODIGO_TABELA_PRECO", dto.CodigoTabelaPreco),
				new SqliteParameter("@URL_SITE_ERP", dto.UrlSiteERP),
				new SqliteParameter("@CURRENT_VERSION", dto.CurrentVersion),
				new SqliteParameter("@INDICE_INICIAL_DIA", dto.IndiceInicialDia),
				new SqliteParameter("@IS_INDICE_INICIAL_DIA_MODIFICADO", dto.IsIndiceInicialDiaModificado),
				new SqliteParameter("@IS_PRE_VENDA", dto.IsPreVenda)
                );

            if (ExecuteNonQuery(commandText.ToString(), parameters))
            {
                dto.Id = newConfiguracaoId;
            }
        }

        protected override void Update(ConfiguracaoDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("UPDATE {0} SET ", GetTableName(String.Empty));
            commandText.Append(PrefixUpdateValues);
            commandText.Append(",UrlWebService = @URL_WEB_SERVICE");
            commandText.Append(",ChaveIntegracao = @CHAVE_INTEGRACAO");
            commandText.Append(",CodigoTabelaPreco = @CODIGO_TABELA_PRECO");
            commandText.Append(",UrlSiteERP = @URL_SITE_ERP");
            commandText.Append(",CurrentVersion = @CURRENT_VERSION");
            commandText.Append(",IndiceInicialDia = @INDICE_INICIAL_DIA");
            commandText.Append(",IsIndiceInicialDiaModificado = @IS_INDICE_INICIAL_DIA_MODIFICADO");
            commandText.Append(",IsPrimeiroAcesso = @IS_PRIMEIRO_ACESSO");
            commandText.Append(",CnpjEmpresa = @CNPJ_EMPRESA");
            commandText.Append(",CampoEspecie = @CAMPO_ESPECIE");
            commandText.Append(",CampoMarca = @CAMPO_MARCA");
			commandText.Append(",IsPreVenda = @IS_PRE_VENDA");
            commandText.AppendFormat(" WHERE {0} = @ID", PrimaryKey);

            SqliteParameter[] parameters = GetParameters(dto, dto.Id,
				new SqliteParameter("@URL_WEB_SERVICE", dto.UrlWebService),
				new SqliteParameter("@CHAVE_INTEGRACAO", dto.ChaveIntegracao),
				new SqliteParameter("@CODIGO_TABELA_PRECO",
				                 dto.CodigoTabelaPreco),
				new SqliteParameter("@URL_SITE_ERP", dto.UrlSiteERP),
				new SqliteParameter("@CURRENT_VERSION", dto.CurrentVersion),
				new SqliteParameter("@INDICE_INICIAL_DIA", dto.IndiceInicialDia),
				new SqliteParameter("@IS_INDICE_INICIAL_DIA_MODIFICADO",
				                 dto.IsIndiceInicialDiaModificado),
				new SqliteParameter("@IS_PRIMEIRO_ACESSO", dto.IsPrimeiroAcesso),
				new SqliteParameter("@CNPJ_EMPRESA", dto.CnpjEmpresa),
				new SqliteParameter("@CAMPO_ESPECIE", dto.CampoEspecie),
				new SqliteParameter("@CAMPO_MARCA", dto.CampoMarca),
				new SqliteParameter("@IS_PRE_VENDA", dto.IsPreVenda)
                );

            ExecuteNonQuery(commandText.ToString(), parameters);
        }

        #endregion

        #region Implementation of IDb<ConfiguracaoDto>

        public IEnumerable<ConfiguracaoDto> GetAll(UsuarioDto usuario)
        {
            throw new NotImplementedException();
        }

        public ConfiguracaoDto GetConfiguracaoAtiva()
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE IsDesabilitado = @IS_DESABILITADO OR IsDesabilitado IS NULL ");
            var parameters = new List<SqliteParameter> {new SqliteParameter("@IS_DESABILITADO", false),};
            return GetObject(query.ToString(), parameters);
        }

        public IEnumerable<ConfiguracaoDto> GetAll()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}