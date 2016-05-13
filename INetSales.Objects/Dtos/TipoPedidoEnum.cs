using System.ComponentModel;

namespace INetSales.Objects.Dtos
{
    public enum TipoPedidoEnum
    {
        Indefinido = 0,
        [Description("Venda")]
        Venda,
        [Description("Bonificação")]
        Bonificacao,
        [Description("Remessa")]
        Remessa,
        [Description("SOS")]
        Sos,
    }
}