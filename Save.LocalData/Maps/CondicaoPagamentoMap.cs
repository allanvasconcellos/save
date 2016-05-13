using System;
using SQLite;

namespace Save.LocalData.Maps
{
	public class CondicaoPagamentoMap
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[Indexed]
		public string Codigo { get; set; }

		public DateTime DataCriacao { get; set; }

		public DateTime? DataAlteracao { get; set; }

		public bool IsDesabilitado { get; set; }

		public string Descricao { get; set; }

		public bool IsDefault { get; set; }

		public bool IsCheque { get; set; }

		public bool IsBoleto { get; set; }

		public bool IsPublica { get; set; }
	}
}

