
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
using Android.Webkit;
using Android.Print;
using INetSales.AndroidUi.Helper;
using INetSales.AndroidUi.Controls;
using INetSales.ViewController.Lib;
using INetSales.Objects.DbInterfaces;

namespace INetSales.AndroidUi.Activities
{
	[Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme")]
	[IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.PrintDisplay })]
	public class PrintActivity : BaseActivity
	{
		private string _systemSsid;
		private string _documentPath;

		#region implemented abstract members of BaseActivity
		protected override void OnBeginView (Bundle bundle)
		{
			SetContentView(Resource.Layout.Print);
			string documentPath = Intent.GetExtra<string>(ActivityFlags.TextoParam);
			var btnAbrirDoc = FindViewById<Button>(Resource.Id.btnAbrirDoc);
			var btnSelectPrinter = FindViewById<Button>(Resource.Id.btnSelectPrinter);
			var wvDocument = FindViewById<WebView>(Resource.Id.wvDocument);
			var settings = wvDocument.Settings;
			_documentPath = documentPath;
			_systemSsid = Wifi.CurrentSsid;
			Wifi.DisconnectCurrent ();
			btnAbrirDoc.Click += AbrirDocClick;
			btnSelectPrinter.Click += SelectPrinterClick;
			settings.JavaScriptEnabled = true;
			settings.AllowFileAccessFromFileURLs = true;
			settings.AllowUniversalAccessFromFileURLs = true;
			settings.BuiltInZoomControls = true;
			wvDocument.SetWebChromeClient(new WebChromeClient());

			wvDocument.LoadUrl("file:///android_asset/pdfviewer/index.html?file=" + documentPath);

			var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
			if (String.IsNullOrEmpty (configuracao.CodePrinterDefault)) {
				ShowWifiPoints ();
			} else {
				ConectarSsid (configuracao.CodePrinterDefault);
			}
		}
		#endregion

		public override void OnBackPressed()
		{
			Wifi.DisconnectCurrent ();
			Wifi.Connect (_systemSsid);
			base.OnBackPressed ();
		}

		private void AbrirDocClick (object sender, EventArgs e)
		{
//			var wvDocument = FindViewById<WebView>(Resource.Id.wvDocument);
//			PrintManager printManager = (PrintManager)this.GetSystemService (Context.PrintService);
//			string jobName = "SaveDocument";
//			var adapter = wvDocument.CreatePrintDocumentAdapter ();
//
//			PrintJob printJob = printManager.Print(jobName, adapter,
//				new PrintAttributes.Builder().Build());
			ActivityHelper.AbrirPdf(this, _documentPath);
		}

		private void SelectPrinterClick (object sender, EventArgs e)
		{
			ShowWifiPoints ();
		}

		private void ShowWifiPoints()
		{
			this.ShowDialog("Selecionar Impressora Wifi", dialog =>
				{
					var layout = BuildLayout.Create(this, Orientation.Vertical);
					foreach(string ssid in Wifi.AvailableNetworks)
					{
						layout.SetButton(ssid.Trim('"', '\\'), 10, 10, 10, 10,
							b =>
							{
								b.Gravity = GravityFlags.CenterHorizontal;
								b.Click +=
									(sender, e) =>
								{
									dialog.Dismiss();
									this.MakeQuestion("Deseja manter essa conexão padrão da impressora?",
										() => SelecionarSsidPrinterDefault(ssid),
										() => ConectarSsid(ssid));
								};
							});
					}
					dialog.SetCancelable(true);
					return layout.Build();
				});
		}

		private void SelecionarSsidPrinterDefault(string ssid)
		{
			var configuracaoDb = DbHelper.GetOffline<IOfflineConfiguracaoDb>();
			var configuracao = configuracaoDb.GetConfiguracaoAtiva ();
			configuracao.CodePrinterDefault = ssid;
			configuracaoDb.Save (configuracao);
			ConectarSsid (ssid);
		}

		private void ConectarSsid(string ssid)
		{
			Wifi.Connect (ssid);
		}

	}
}

