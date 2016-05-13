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

namespace INetSales.AndroidUi.Activities.Main
{
    partial class MenuRoteiroFragment
    {
        private View CreateView01(LayoutInflater p0)
        {
            var view = p0.Inflate(Resource.Layout.MenuFragment_01, null);
            var btSincronizar = view.FindViewById<ImageButton>(Resource.Id.btSincronizar);
            var btRelatorio = view.FindViewById<ImageButton>(Resource.Id.btRelatorio);
            var btConfigurar = view.FindViewById<ImageButton>(Resource.Id.btConfigurar);
            var btVersao = view.FindViewById<ImageButton>(Resource.Id.btVersao);

            Activity.RegisterForContextMenu(btSincronizar);

            btConfigurar.Click += (sender, e) => ConfigurarClick();
            btSincronizar.Click += (sender, e) => OpenSincronizarMenu();
            btVersao.Click += VersaoClick;
            btRelatorio.Click += RelatorioClick;

            return view;
        }
    }
}