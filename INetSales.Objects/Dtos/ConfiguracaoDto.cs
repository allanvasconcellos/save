using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Configuracao")]
    public class ConfiguracaoDto : Dto<ConfiguracaoDto>
    {
        public string UrlWebService { get; set; }

        public string ChaveIntegracao { get; set; }

        public string CodigoTabelaPreco { get; set; }

        public string UrlSiteERP { get; set; }

        public string CurrentVersion { get; set; }

        public bool IsPrimeiroAcesso { get; set; }

        public string CnpjEmpresa { get; set; }

        public string CampoEspecie { get; set; }

        public string CampoMarca { get; set; }

		public string UsernameDebug { get; set; }

		public string PasswordDebug { get; set; }

		public string CodePrinterDefault { get; set; }

        #region Rota

        /// <summary>
        /// Indica o indice que irá iniciar a rota no dia.
        /// </summary>
        public int IndiceInicialDia { get; set; }

        /// <summary>
        /// Indica que o IndiceInicialDia foi modificado.
        /// </summary>
        public bool IsIndiceInicialDiaModificado { get; set; }

        /// <summary>
        /// Indica o indice que foi inserido no inicio do bloco de rotas. Está relacionado ao DiaBlocoInicial.
        /// </summary>
		[Ignore]
        public int IndiceBlocoInicial { get; set; }

        /// <summary>
        /// Indica o indice que foi inserido no final do bloco de rotas. Está relacionado ao DiaBlocoFinal.
        /// </summary>
		[Ignore]
        public int IndiceBlocoFinal { get; set; }

        /// <summary>
        /// Indica o dia que foi inserido no inicio do bloco de rotas. Está relacionado ao IndiceBlocoInicial.
        /// </summary>
		[Ignore]
        public DateTime DiaBlocoInicial { get; set; }

        /// <summary>
        /// Indica o dia que foi inserido no final do bloco de rotas. Está relacionado ao IndiceBlocoFinal.
        /// </summary>
		[Ignore]
        public DateTime DiaBlocoFinal { get; set; }

		/// <summary>
		/// Indica que o tablet é pré-venda
		/// </summary>
		public bool IsPreVenda { get; set; }

        #endregion

        #region Intervalos Sincronização

        public TimeSpan IntervaloSyncUsuario { get; set; }

        public TimeSpan IntervaloSyncGrupo { get; set; }

        public TimeSpan IntervaloSyncRamo { get; set; }

        public TimeSpan IntervaloSyncProduto { get; set; }

        public TimeSpan IntervaloSyncCliente { get; set; }

        public TimeSpan IntervaloSyncRemessa { get; set; }

        public TimeSpan IntervaloSyncRota { get; set; }

        #endregion
    }
}