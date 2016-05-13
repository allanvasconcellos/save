namespace INetSales.AndroidUi
{
    public enum TipoMenuEnum : int
    {
        Indefinido = 0,
        Sincronizar,
        Cliente,
        RoteiroCliente,
    }
    public interface IMenuActivity
    {
        void SetTipoMenu(TipoMenuEnum tipo);
    }
}