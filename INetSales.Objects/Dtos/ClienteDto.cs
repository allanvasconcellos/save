using System;
using System.Collections.Generic;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Cliente")]
    public class ClienteDto : Dto<ClienteDto>, IUploader
    {
        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public TipoPessoaEnum TipoPessoa { get; set; }

        /// <summary>
        /// CPF / CNPJ
        /// </summary>
        public string Documento { get; set; }

        /// <summary>
        /// RG / IE (Inscrição Estadual)
        /// </summary>
        public string Identificacao { get; set; }

        public string Email { get; set; }

		public int UsuarioId { get; set; }
        /// <summary>
        /// Vendedor para este cliente.
        /// </summary>
		[Ignore]
        public UsuarioDto Usuario { get; set; }

        /// <summary>
        /// Indica que o cliente está em uma rota (antigo IsAtivoAdm).
        /// </summary>
        public bool HasRota { get; set; }

        /// <summary>
        /// Indica que o cliente tem pendências.
        /// </summary>
        public bool HasPendencia { get; set; }

		/// <summary>
		/// Indica se é permitir utilizar pedido com boleto para o cliente.
		/// </summary>
		public bool IsPermitidoBoleto { get; set; }

        #region Roteiro

        public int OrdemRoteiro { get; set; }

        /// <summary>
        /// Indica que o cliente está ativo no roteiro
        /// </summary>
        public bool IsAtivoRoteiro { get; set; }

        /// <summary>
        /// Indica que é permitido realizar pedido fora do dia corrente no roteiro.
        /// </summary>
        public bool IsPermitidoForaDia { get; set; }

        /// <summary>
        /// Indica que existe pedidos para esse cliente no roteiro.
        /// </summary>
        public bool HasPedidoRoteiro { get; set; }

		[Ignore]
        public RotaDto RoteiroCorrente { get; set; }

		[Ignore]
		public IEnumerable<ClienteRotaDto> Roteiros { get; set; }

        #endregion

        #region Endereço

        public string EnderecoRua { get; set; }

        public int EnderecoNumero { get; set; }

        public string Cep { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string Telefone { get; set; }

        #endregion

		#region Pendencia

		[Ignore]
		public IEnumerable<PendenciaDto> Pendencias { get; set; }

		#endregion

        //public override bool Equals(ClienteDto other)
        //{
        //    if(base.Equals(other))
        //    {
        //        return true;
        //    }
        //    return !String.IsNullOrEmpty(Documento) ? Documento.Equals(other.Documento) : false;
        //}

        #region Implementation of IUploader

        /// <summary>
        /// Indica que esta pendente de upload.
        /// </summary>
        public bool IsPendingUpload { get; set; }

        /// <summary>
        /// Data em foi feito o ultimo upload.
        /// </summary>
        public DateTime? DataLastUpload { get; set; }

        #endregion
    }
}