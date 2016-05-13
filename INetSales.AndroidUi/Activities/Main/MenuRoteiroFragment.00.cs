using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using INetSales.Objects.Dtos;
using Android.Print;
using Android.Content;
using Android.Webkit;

namespace INetSales.AndroidUi.Activities.Main
{
    partial class MenuRoteiroFragment
    {
        private View CreateView00(LayoutInflater p0)
        {
            var view = p0.Inflate(Resource.Layout.MenuFragment_00, null);
            var btCliente = view.FindViewById<ImageButton>(Resource.Id.btCliente);
            var btPedidos = view.FindViewById<ImageButton>(Resource.Id.btPedidos);
            var btRemessa = view.FindViewById<ImageButton>(Resource.Id.btRemessa);
			//var btTestePrint = view.FindViewById<ImageButton>(Resource.Id.btTestePrint);
            var btLogoff = view.FindViewById<ImageButton>(Resource.Id.btLogoff);

            Activity.RegisterForContextMenu(btCliente);
#if !DEBUG && !DEBUG_NO_THREAD
            btCliente.Click += (sender, e) => Activity.LaunchActivity(ActivityFlags.ClienteCategory);
#else
            btCliente.Click += (sender, e) => OpenClienteMenu(sender);
#endif
            btPedidos.Click += (sender, e) => Activity.LaunchActivity(ActivityFlags.ListaPedidoCategory);
            btRemessa.Click += (sender, e) => Activity.LaunchActivity(ActivityFlags.PedidoCategory, new Dictionary<string, object> { { ActivityFlags.TipoPedidoParam, TipoPedidoEnum.Remessa } });
			//btTestePrint.Click += BtTestePrint_Click; ;
            btLogoff.Click += (sender, e) => LogoffClick();

            return view;
        }

        void BtTestePrint_Click (object sender, System.EventArgs e)
        {
//			PrintManager printManager = (PrintManager)Activity.GetSystemService (Context.PrintService);
//			string jobName = "SaveDocument";
//			var intent = new Intent (Intent.ActionView);
//			var file = new Java.IO.File ("/storage/emulated/0/Download/Teste.pdf");
//			string extension = MimeTypeMap.GetFileExtensionFromUrl (file.AbsolutePath);
//			string mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension (extension);
//
//			intent.SetDataAndType (Android.Net.Uri.FromFile (file), mimeType);
//			//intent.SetFlags (Android.Content.ActivityFlags.ClearTop);
//			Activity.StartActivity (intent);
			var parameters = new Dictionary<string, object> { { ActivityFlags.TextoParam, "/storage/emulated/0/Download/Teste.pdf" } };
			Activity.LaunchActivity(ActivityFlags.PrintDisplay, parameters);
        }
    }
}