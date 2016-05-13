using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects
{
    public class Pasta : IEquatable<Pasta>
    {
        public int Indice { get; set; }

        public string Codigo { get; set; }

        public string Nome { get; set; }

        public List<ClienteDto> Clientes { get; set; }

        #region Implementation of IEquatable<Pasta>

        public bool Equals(Pasta other)
        {
            return this.Indice.Equals(other.Indice);
        }

        #endregion
    }
}