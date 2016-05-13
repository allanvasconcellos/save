using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using Android.Webkit;

namespace INetSales.AndroidUi
{
	public static class ActivityHelper
	{
		public static ActivityInfo GetActivity (this Activity activity, string categoryFilter)
		{
			var query = new Intent (Intent.ActionMain, null);
			query.AddCategory (categoryFilter);

			var list = activity.PackageManager.QueryIntentActivities (query, 0);

			if (list == null || list.Count <= 0) {
				throw new ArgumentException (String.Format ("Categoria {0} não encontrada", categoryFilter));
			}
			var resolve = list [0];
			var category = resolve.LoadLabel (activity.PackageManager);

			// Get the data we'll need to launch the activity
			string type = string.Format ("{0}:{1}", resolve.ActivityInfo.ApplicationInfo.PackageName, resolve.ActivityInfo.Name);

			//if (string.IsNullOrWhiteSpace(prefix) || category.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
			return new ActivityInfo (String.Empty, category, type);
		}

		public static void LaunchActivity (this Activity activity, ActivityInfo item)
		{
			var result = new Intent ();
			result.SetClassName (item.Package, item.Component);

			activity.StartActivity (result);
		}

		//public static void LaunchMainActivity(this Activity activity)
		//{
		//    var intent = new Intent(Intent.ActionMain);
		//    intent.SetFlags(Intent.);
		//    startActivity(intent);
		//}

		public static void LaunchActivity (this Activity activity, string categoryFilter)
		{
			var item = GetActivity (activity, categoryFilter);
			LaunchActivity (activity, item);
		}

		public static void LaunchActivity (this Activity activity, string categoryFilter, Dictionary<string, object> parameters)
		{
			var item = GetActivity (activity, categoryFilter);
			var result = new Intent ();
			result.SetClassName (item.Package, item.Component);
			result.PutExtras (parameters);
			activity.StartActivity (result);
			//LaunchActivity(activity, item);
		}

		public static void PutExtras (this Intent intent, Dictionary<string, object> parameters)
		{
			foreach (var parameter in parameters) {
				var serializer = new DataContractJsonSerializer (parameter.Value.GetType ());
				var writer = new MemoryStream ();
				var reader = new StreamReader (writer);
				serializer.WriteObject (writer, parameter.Value);
				writer.Position = 0;
				intent.PutExtra (parameter.Key, reader.ReadToEnd ());
			}
		}

		public static void PutExtra<TObject> (this Intent intent, string key, TObject obj)
		{
			PutExtras (intent, new Dictionary<string, object> { { key, obj } });
		}

		public static TObject GetExtra<TObject> (this Intent intent, string key)
		{
			var serializer = new DataContractJsonSerializer (typeof(TObject));
			var extra = intent.GetStringExtra (key);
			if (extra != null) {
				var memoObj = new MemoryStream (Encoding.ASCII.GetBytes (extra));
				return (TObject)serializer.ReadObject (memoObj);
			}
			return default(TObject);
		}

		public static void ShowMessageBox (Context ctx, string message, string title, Action<object, DialogClickEventArgs> actionOk)
		{
			var dialog = new AlertDialog.Builder (ctx);
			dialog
                .SetTitle (title)
                .SetMessage (message)
				.SetPositiveButton (Resource.String.alert_dialog_ok, (sender, e) => actionOk (sender, e) )
                .Show ();
		}

		public static void ShowDialog (this Context ctx, string title, View view, Action close = null, Action cancel = null)
		{
			var dialog = new Dialog (ctx, Resource.Style.DialogModal);
            
			if (close != null) {
				// BuildLayout
				var layoutWithClose = BuildLayout.Create (ctx, Orientation.Vertical)
                    .SetView (view)
                    .SetButton ("Fechar", 10, 0, 0, 0, b => {
					b.Gravity = GravityFlags.CenterHorizontal;
					b.Click += (sender, e) => {
						close ();
						dialog.Cancel ();
					};
				})
                    .Build ();
				dialog.SetContentView (layoutWithClose);
			} else {
				dialog.SetContentView (view);
			}

			dialog.SetTitle (title);
			dialog.Show ();
			if (cancel != null) {
				dialog.SetCancelable (true);
				dialog.CancelEvent += (sender, e) => cancel ();
			}
		}

		public static void ShowDialog (this Context ctx, string title, Func<Dialog, View> preRender)
		{
			var dialog = new Dialog (ctx, Resource.Style.DialogModal);
			dialog.SetTitle (title);
			dialog.SetContentView (preRender (dialog));
			//BuildLayout.Create(ctx, Orientation.Vertical)
			//    .SetView()
			dialog.Show ();
		}

		public static void ShowQuestion (Context ctx, string question, string title, Action simAction, Action naoAction)
		{
			var builder = new AlertDialog.Builder (ctx);

			builder
                .SetTitle (title)
                .SetMessage (question)
                .SetPositiveButton ("Sim", (sender, e) => simAction ())
                .SetNegativeButton ("Não", (sender, e) => {
				if (naoAction != null)
					naoAction ();
			})
                .Create ()
                .Show ();
		}

		public static void ShowListDialog (this Activity activity, string title, Action<ListView> render)
		{
			var view = activity.LayoutInflater.Inflate (Resource.Layout.ListModal, null);
			var listControl = view.FindViewById<ListView> (Resource.Id.lvModal);
			render (listControl);
			ShowDialog (activity, title, view);
		}

		public static void ShowTableDialog (this Activity activity, string title, Action<TableLayout, TableRow> render)
		{
			var view = activity.LayoutInflater.Inflate (Resource.Layout.TableModal, null);
			var table = view.FindViewById<TableLayout> (Resource.Id.tlModal);
			var trHeader = view.FindViewById<TableRow> (Resource.Id.trHeader);
			render (table, trHeader);
			ShowDialog (activity, title, view);
		}

		public static decimal GetTextDecimal (string text)
		{
			decimal valor;
			var numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone ();
			numberFormat.NumberDecimalSeparator = ".";
			if (!String.IsNullOrEmpty (text) && Decimal.TryParse (text, NumberStyles.Currency, numberFormat, out valor)) {
				return valor;
			}
			return 0;
		}

		public static void SetTitle (this Activity activity, string title, params string[] titleLines)
		{
			const int idLayout = 2012;
			var view = activity.Window.DecorView;
			var viewGroup = (ViewGroup)view.RootView;
			var firstChild = (ViewGroup)viewGroup.GetChildAt (0);
			var layoutTitle = activity.FindViewById (idLayout);
			if (layoutTitle != null) {
				firstChild.RemoveView (layoutTitle);
			}

			var layout = BuildLayout.Create (activity, Orientation.Vertical)
				.SetText ("SAVE - SISTEMA DE AUTOMACAO DE VENDAS", 0, 0, 0, 0, t => {
				var lp = (LinearLayout.LayoutParams)t.LayoutParameters;
				lp.Width = ViewGroup.LayoutParams.FillParent;
				t.Gravity = GravityFlags.CenterHorizontal;
				t.SetTextSize (ComplexUnitType.Px, 16);
				t.SetTextColor (Color.White);
				t.SetTypeface (null, TypefaceStyle.Bold);
				if (titleLines.Length <= 0) {
					lp.BottomMargin = 0;
				}
			})
                .SetText (String.Format ("Ver. {0}", GetStringVersion ()), 0, 0, 0, 7, t => {
				var lp = (LinearLayout.LayoutParams)t.LayoutParameters;
				lp.Width = ViewGroup.LayoutParams.FillParent;
				t.Gravity = GravityFlags.CenterHorizontal;
				t.SetTextSize (ComplexUnitType.Px, 8);
				t.SetTextColor (Color.White);
				if (titleLines.Length <= 0) {
					lp.BottomMargin = 0;
				}
			})
                .ForEach (titleLines, (line, b, position) => {
				if (position == titleLines.Length - 1) {
					b.SetText (line, 5, 0, 0, 5, t => t.SetTextColor (Color.White));
					return;
				}
				b.SetText (line, 5, 0, 0, 0, t => t.SetTextColor (Color.White));
			})
                .Build ();
			layout.Id = 2012;
			layout.SetBackgroundResource (Android.Resource.Drawable.TitleBar);
			firstChild.AddView (layout, 0);
		}

		public static string GetStringVersion ()
		{
			var version = Assembly.GetExecutingAssembly ().GetName ().Version;
			return String.Format ("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
		}

		public static void AbrirPdf (Activity activity, string urlPdf)
		{
			var intent = new Intent (Intent.ActionView);
			var file = new Java.IO.File (urlPdf);
			string extension = MimeTypeMap.GetFileExtensionFromUrl (file.AbsolutePath);
			string mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension (extension);

			intent.SetDataAndType (Android.Net.Uri.FromFile (file), mimeType);
			//intent.SetFlags (Android.Content.ActivityFlags.ClearTop);
			activity.StartActivity (intent);
		}
	}
}