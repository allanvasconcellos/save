using System;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Usuario")]
    public class UsuarioDto : Dto<UsuarioDto>
    {
        public string CodigoSecundario { get; set; }

        public string Username { get; set; }

        public string SenhaHash { get; set; }

        public string Nome { get; set; }

        public string PlacaVeiculo { get; set; }

        public bool IsAdm { get; set; }

        /// <summary>
        /// Indica que uma sincronização completa está pendente para este usuário.
        /// </summary>
        public bool IsSyncPending { get; set; }

        public override bool Equals(UsuarioDto other)
        {
            return base.Equals(other) || Username.Equals(other.Username);
        }
    }
}
