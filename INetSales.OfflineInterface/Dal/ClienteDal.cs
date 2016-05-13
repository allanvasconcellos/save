using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Mono.Data.Sqlite;
using System.Diagnostics;

namespace INetSales.OfflineInterface.Dal
{
    public class ClienteDal : DtoDal<ClienteDto>, IOfflineClienteDb, IUploadDb
    {
		public const string ID_COLUMN = "ClienteId";
		public const string USUARIO_COLUMN = "UsuarioId";
		public const string RAZAOSOCIAL_COLUMN = "RazaoSocial";
		public const string NOMEFANTASIA_COLUMN = "NomeFantasia";
		public const string DOCUMENTO_COLUMN = "Documento";
		public const string TIPOPESSOA_COLUMN = "TipoPessoa";
		public const string HASPENDENCIA_COLUMN = "HasPendencia";
		public const string ISATIVOADM_COLUMN = "IsAtivoAdm";
		public const string ENDERECORUA_COLUMN = "EnderecoRua";
		public const string ENDERECONUMERO_COLUMN = "EnderecoNumero";
		public const string ENDERECOBAIRRO_COLUMN = "EnderecoBairro";
		public const string ENDERECOCEP_COLUMN = "EnderecoCep";
		public const string ENDERECOCIDADE_COLUMN = "EnderecoCidade";
		public const string PERMITEBOLETO_COLUMN = "PermiteBoleto";

		public ClienteDal(IDbContext context)
			: base(context,
				ID_COLUMN, CODIGO_COLUMN, IS_DESABILITADO_COLUMN, DATA_CRIACAO_COLUMN, DATA_ALTERACAO_COLUMN, IS_PENDING_UPLOAD_COLUMN, 
				DATA_LAST_UPLOAD_COLUMN, USUARIO_COLUMN, RAZAOSOCIAL_COLUMN, NOMEFANTASIA_COLUMN, DOCUMENTO_COLUMN, TIPOPESSOA_COLUMN, 
				HASPENDENCIA_COLUMN, ISATIVOADM_COLUMN, ENDERECORUA_COLUMN, ENDERECONUMERO_COLUMN, ENDERECOBAIRRO_COLUMN, 
				ENDERECOCEP_COLUMN, ENDERECOCIDADE_COLUMN, PERMITEBOLETO_COLUMN)
        {
			TableName = "TCliente";
			PrimaryKey = ID_COLUMN;
        }

		protected override bool DoEspecificMap(SqlReader reader, ClienteDto dto)
        {
			int ordemIndex = reader.GetColumnIndex("Ordem");
			int isAtivoRoteiroIndex = reader.GetColumnIndex("IsAtivoRoteiro");
			int hasPedidoIndex = reader.GetColumnIndex("HasPedido");
			int IsPermitidoForaDiaIndex = reader.GetColumnIndex("IsPermitidoForaDia");
            var usuarioDal = new UsuarioDal(Connection);
            var pendenciaDal = new PendenciaDal(Connection);

			int usuarioIndex = reader.GetColumnIndex(USUARIO_COLUMN);
			int razaoIndex = reader.GetColumnIndex(RAZAOSOCIAL_COLUMN);
			int fantasiaIndex = reader.GetColumnIndex(NOMEFANTASIA_COLUMN);
			int documentoIndex = reader.GetColumnIndex(DOCUMENTO_COLUMN);
			int tipoPessoaIndex = reader.GetColumnIndex(TIPOPESSOA_COLUMN);
			int pendenciaIndex = reader.GetColumnIndex(HASPENDENCIA_COLUMN);
			int ativoIndex = reader.GetColumnIndex(ISATIVOADM_COLUMN);

			dto.Usuario = usuarioDal.Find(reader.GetInt32(nextIndex++));
			dto.Pendencias = pendenciaDal.GetPendencias(dto);

			dto.RazaoSocial = reader.GetString(RAZAOSOCIAL_COLUMN);
			dto.Documento = reader.GetString(DOCUMENTO_COLUMN);
			dto.HasPendencia = reader.GetString(HASPENDENCIA_COLUMN);
			dto.NomeFantasia = reader.GetString(NOMEFANTASIA_COLUMN);
			dto.HasRota = reader.GetBool(ISATIVOADM_COLUMN);
            dto.TipoPessoa = GetTipoPessoaCliente(reader.GetChar(TIPOPESSOA_COLUMN));
			dto.EnderecoRua = reader.GetString(ENDERECORUA_COLUMN);
			dto.EnderecoNumero = reader.GetInt(ENDERECONUMERO_COLUMN);
			dto.Bairro = reader.GetString(ENDERECOBAIRRO_COLUMN);
			dto.Cep = reader.GetString(ENDERECOCEP_COLUMN);
			dto.Cidade = reader.GetString(ENDERECOCIDADE_COLUMN);
			dto.IsPermitidoBoleto = reader.GetBool(PERMITEBOLETO_COLUMN);

            // Não fixa na constante
            dto.OrdemRoteiro = reader.GetInt(ordemIndex);
            dto.IsAtivoRoteiro = reader.GetBool(isAtivoRoteiroIndex);
            dto.HasPedidoRoteiro = reader.GetBool(hasPedidoIndex);
            dto.IsPermitidoForaDia = reader.GetBool(IsPermitidoForaDiaIndex);
        }

        protected override void Insert(ClienteDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("INSERT INTO {0} ", GetTableName(String.Empty));
            commandText.AppendFormat("({0}) ", GetQueryField(String.Empty));
            commandText.Append("VALUES ");
            commandText.Append("(" + PrefixInsertValues +
                               ", @RAZAO, @DOCUMENTO, @HAS_PENDENCIA, @NOME_FANTASIA, @IS_ATIVO_ADM, @USUARIO, @TIPO_PESSOA," +
				"  @ENDERECORUA, @ENDERECONUMERO, @ENDERECOBAIRRO, @ENDERECOCEP, @ENDERECOCIDADE, @PERMITIDOBOLETO) ");

            int newClienteId = GetNextPkValue(PrimaryKey, GetTableName(String.Empty));

            SqliteParameter[] parameters = GetParameters(dto, newClienteId,
                                                         new SqliteParameter("@RAZAO", dto.RazaoSocial),
                                                         new SqliteParameter("@DOCUMENTO", dto.Documento),
                                                         new SqliteParameter("@TIPO_PESSOA",
                                                                             GetCodeTipoPessoaCliente(dto.TipoPessoa)),
                                                         new SqliteParameter("@HAS_PENDENCIA", dto.HasPendencia),
                                                         new SqliteParameter("@NOME_FANTASIA", dto.NomeFantasia),
                                                         new SqliteParameter("@IS_ATIVO_ADM", dto.HasRota),
                                                         new SqliteParameter("@USUARIO", dto.Usuario.Id),
                                                         new SqliteParameter("@ENDERECORUA", dto.EnderecoRua),
                                                         new SqliteParameter("@ENDERECONUMERO", dto.EnderecoNumero),
                                                         new SqliteParameter("@ENDERECOBAIRRO", dto.Bairro),
                                                         new SqliteParameter("@ENDERECOCEP", dto.Cep),
                                                         new SqliteParameter("@ENDERECOCIDADE", dto.Cidade),
				new SqliteParameter("@PERMITIDOBOLETO", dto.IsPermitidoBoleto)
                );

            if (ExecuteNonQuery(commandText.ToString(), parameters))
            {
                dto.Id = newClienteId;

                // Salvar pend¨ºncia
                PendenciaDal pendenciaDal = new PendenciaDal(this.Connection);
                foreach (var pendencia in dto.Pendencias)
                {
                    pendencia.Cliente = dto;
                    pendencia.ClienteId = dto.Id;
                    pendenciaDal.Save(pendencia);
                }
            }
        }

        protected override void Update(ClienteDto dto)
        {
            var commandText = new StringBuilder();
            commandText.AppendFormat("UPDATE {0} SET ", GetTableName(String.Empty));
            commandText.Append(PrefixUpdateValues);
            commandText.Append(",RazaoSocial = @RAZAO");
            commandText.Append(",Documento = @DOCUMENTO");
            commandText.Append(",TipoPessoa = @TIPO_PESSOA");
            commandText.Append(",HasPendencia = @HAS_PENDENCIA");
            commandText.Append(",NomeFantasia = @NOME_FANTASIA");
            commandText.Append(",IsAtivoAdm = @IS_ATIVO_ADM");
            commandText.Append(",UsuarioId = @USUARIO");
            commandText.Append(",EnderecoRua = @ENDERECORUA");
            commandText.Append(",EnderecoNumero = @ENDERECONUMERO");
            commandText.Append(",EnderecoBairro = @ENDERECOBAIRRO");
            commandText.Append(",EnderecoCep = @ENDERECOCEP");
            commandText.Append(",EnderecoCidade = @ENDERECOCIDADE");
			commandText.Append(",PermiteBoleto = @PERMITIDOBOLETO");
            commandText.AppendFormat(" WHERE {0} = @ID", PrimaryKey);

            SqliteParameter[] parameters = GetParameters(dto, dto.Id,
                                                         new SqliteParameter("@RAZAO", dto.RazaoSocial),
                                                         new SqliteParameter("@DOCUMENTO", dto.Documento),
                                                         new SqliteParameter("@TIPO_PESSOA",
                                                                             GetCodeTipoPessoaCliente(dto.TipoPessoa)),
                                                         new SqliteParameter("@HAS_PENDENCIA", dto.HasPendencia),
                                                         new SqliteParameter("@NOME_FANTASIA", dto.NomeFantasia),
                                                         new SqliteParameter("@IS_ATIVO_ADM", dto.HasRota),
                                                         new SqliteParameter("@USUARIO", dto.Usuario.Id),
                                                         new SqliteParameter("@ENDERECORUA", dto.EnderecoRua),
                                                         new SqliteParameter("@ENDERECONUMERO", dto.EnderecoNumero),
                                                         new SqliteParameter("@ENDERECOBAIRRO", dto.Bairro),
                                                         new SqliteParameter("@ENDERECOCEP", dto.Cep),
                                                         new SqliteParameter("@ENDERECOCIDADE", dto.Cidade),
				new SqliteParameter("@PERMITIDOBOLETO", dto.IsPermitidoBoleto)
                );

            ExecuteNonQuery(commandText.ToString(), parameters);

            // Pendencias
            // Salvar pend¨ºncia
            PendenciaDal pendenciaDal = new PendenciaDal(this.Connection);
            foreach (var pendencia in dto.Pendencias)
            {
                var pendenciaLocal = pendenciaDal.FindByCodigo(pendencia.Codigo);
                pendencia.Cliente = dto;
                pendencia.ClienteId = dto.Id;
                if (pendenciaLocal != null)
                {
                    pendencia.Id = pendenciaLocal.Id;
                }
                pendenciaDal.Save(pendencia);
            }
        }

        private TipoPessoaEnum GetTipoPessoaCliente(char codigo)
        {
            if (codigo.Equals('F'))
            {
                return TipoPessoaEnum.Fisica;
            }
            if (codigo.Equals('J'))
            {
                return TipoPessoaEnum.Juridica;
            }
            return TipoPessoaEnum.Indefinido;
        }

        private char GetCodeTipoPessoaCliente(TipoPessoaEnum tipo)
        {
            switch (tipo)
            {
                case TipoPessoaEnum.Juridica:
                    return 'J';
                case TipoPessoaEnum.Fisica:
                    return 'F';
                default:
                    throw new InvalidOperationException();
            }
        }

        #region Implementation of IDb<ClienteDto>

        public IEnumerable<ClienteDto> GetAll(UsuarioDto usuario)
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

        public IEnumerable<ClienteDto> GetClientes(RotaDto rota)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0}, R.Ordem, R.IsAtivoRoteiro, R.HasPedido, R.IsPermitidoForaDia ",
                               GetQueryField("C"));
            query.AppendFormat("FROM {0} ", GetTableName("C"));
            query.Append("JOIN TRotaCliente R ON C.ClienteId = R.ClienteId ");
            query.Append("WHERE R.RotaId = @ROTA ");
            var parameters = new[]
                                 {
                                     new SqliteParameter("@ROTA", rota.Id),
                                 };
            return GetList(query.ToString(), parameters, c =>
                                                             {
                                                                 c.RoteiroCorrente = new RotaDto
                                                                                         {
                                                                                             Id = rota.Id,
                                                                                             Dia = rota.Dia,
                                                                                             Usuario = rota.Usuario,
                                                                                             IndicePasta =
                                                                                                 rota.IndicePasta,
                                                                                         };
                                                                 return true;
                                                             });
        }

        public ClienteDto FindClienteByDocumento(string documento)
        {
            var query = new StringBuilder();
            query.AppendFormat("SELECT {0} ", GetQueryField(String.Empty));
            query.AppendFormat("FROM {0} ", GetTableName(String.Empty));
            query.Append("WHERE Documento = @DOCUMENTO ");
            var parameters = new[] {new SqliteParameter("@DOCUMENTO", documento),};
            return GetObject(query.ToString(), parameters);
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