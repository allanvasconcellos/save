using System;
using System.Collections.Generic;
using SQLite;

namespace INetSales.Objects.Dtos
{
	[Table("Grupo")]
	public class GrupoDto : Dto<GrupoDto>
    {
        public string Nome { get; set; }

		[Ignore]
		public bool IsSubgrupo { get { return GrupoPai != null; } }

		public int GrupoPaiId { get; set; }
		[Ignore]
        public GrupoDto GrupoPai { get; set; }

		//[OneToMany(CascadeOperations = CascadeOperation.All)]
		[Ignore]
        public IEnumerable<ProdutoDto> Produtos { get; set; }
    }
}