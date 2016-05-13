namespace INetSales.Objects.DbInterfaces
{
    public interface IPackageDb : IOnlineDb
    {
        bool TryGetUrlAndroidPackage(string lastVersion, out string urlPackage, out string version);
    }
}