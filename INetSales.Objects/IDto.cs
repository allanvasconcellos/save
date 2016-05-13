using System;
using SQLite;

namespace INetSales.Objects
{
    public interface IDto
    {
        int Id { get; set; }

        string Codigo { get; set; }

        DateTime DataCriacao { get; set; }

        DateTime? DataAlteracao { get; set; }

        bool IsDesabilitado { get; set; }
    }

    public class Dto<TDto> : IDto, IEquatable<TDto>
        where TDto : IDto
    {
		[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
		[Indexed]
        public string Codigo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool IsDesabilitado { get; set; }
        public virtual bool Equals(TDto other)
        {
            return Id.Equals(other.Id) || Codigo.Equals(other.Codigo);
        }
    }
}