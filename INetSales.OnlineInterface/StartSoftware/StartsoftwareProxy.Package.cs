using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using INetSales.Objects.DbInterfaces;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IPackageDb
    {
        #region Implementation of IPackageDb

        public bool TryGetUrlAndroidPackage(string lastVersion, out string urlPackage, out string version)
        {
            return _compINetSales.TryGetUrlAndroidPackage(lastVersion, out urlPackage, out version);
        }

        #endregion
    }
}