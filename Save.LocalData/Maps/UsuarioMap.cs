using System;
using SQLite;

namespace Save.LocalData.Maps
{
	public class UsuarioMap
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[Indexed]
		public string Codigo { get; set; }

		public DateTime DataCriacao { get; set; }

		public DateTime? DataAlteracao { get; set; }

		public bool IsDesabilitado { get; set; }

		public string CodigoSecundario { get; set; }

		public string Username { get; set; }

		public string SenhaHash { get; set; }

		public string Nome { get; set; }

		public string PlacaVeiculo { get; set; }

		public bool IsAdm { get; set; }
	}
}

