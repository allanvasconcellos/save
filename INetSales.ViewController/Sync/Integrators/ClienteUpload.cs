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
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class ClienteUpload : Upload<ClienteDto>
    {
        public ClienteUpload(params ClienteDto[] uploaders)
            : base(DbHelper.GetOnline<IClienteDb>(), DbHelper.GetOffline<IOfflineClienteDb>(), uploaders)
        {
        }
    }
}