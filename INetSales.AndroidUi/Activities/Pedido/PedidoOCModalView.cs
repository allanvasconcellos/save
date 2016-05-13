using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Views;
using INetSales.Objects;

namespace INetSales.AndroidUi.Activities.Pedido
{
	public class OkOCEventArgs : EventArgs
	{
		public string OCText { get; set; }
	}

	public class PedidoOCModalView
    {
        private readonly Activity _activity;
        private readonly PedidoController _controller;

		public PedidoOCModalView(Activity activity, PedidoController controller)
        {
            _activity = activity;
            _controller = controller;
        }

        public void Show()
        {
			_activity.ShowDialog("OC", dialog =>
				{
					var view = _activity.LayoutInflater.Inflate(Resource.Layout.PedidoOC, null);
					var tvOc = view.FindViewById<TextView>(Resource.Id.tvOc);
					var btOk = view.FindViewById<Button>(Resource.Id.btnOkOc);
					var btCancel = view.FindViewById<Button>(Resource.Id.btnCancelarOc);
					tvOc.Text = _controller.PedidoCorrente.OrdemCompra;
					btOk.Click += (sender, e) => {
						_controller.InserirOC(tvOc.Text);
						dialog.Cancel(); 
						Utils.InvokeOnSelect(OnOk, this, new OkOCEventArgs() { OCText = tvOc.Text }); 
					};
					btCancel.Click += (sender, e) => { 
						dialog.Cancel(); 
						Utils.InvokeOnSelect(OnCancel, this, EventArgs.Empty); 
					};
					var layout = BuildLayout.Create(_activity, Orientation.Vertical)
						.SetView(view)
						.Build();
					dialog.SetCancelable(false);
					return layout;
				});
        }

		public event EventHandler<OkOCEventArgs> OnOk;
		public event EventHandler<EventArgs> OnCancel;
    }
}