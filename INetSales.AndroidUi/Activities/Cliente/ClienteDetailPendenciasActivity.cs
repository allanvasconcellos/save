
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using INetSales.Objects;
using INetSales.AndroidUi.Activities.Cliente;
using Android.Graphics;
using Android.Util;
using INetSales.ViewController;
using Environment = System.Environment;

namespace INetSales.AndroidUi
{
	[Activity(Label = "INet Sales", Theme = "@style/INetTheme")]
	public class ClienteDetailPendenciasActivity : BaseActivity
	{
		ListView _listPendencia;

		public ClienteDto Cliente { get; set; }

		#region implemented abstract members of BaseActivity

		protected override void OnBeginView (Bundle bundle)
		{
			SetContentView(Resource.Layout.ClienteDetailPendencia);

			var tabParent = Parent as ClienteDetailActivity;
			tabParent.RegistrarPendenciaActivity(this);

			_listPendencia = FindViewById<ListView>(Resource.Id.lvPendencia);

		}

		#endregion

		private void ShowNotaPendente(PendenciaDto pendencia)
		{
            string filename = String.Format("{0}_{1:ddMMyyyyHHmmss}_PENDENCIA.pdf", pendencia.Codigo, pendencia.DataCriacao);
            string fullPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
            if (!File.Exists(fullPath))
            {
				if (String.IsNullOrEmpty (pendencia.LinkPagamento)) {
					ShowMessage ("Link para a nota não existe");
					return;
				}
                BaixarNotaPendente(pendencia, filename);
                //ActivityHelper.AbrirPdf(this, fullPath);
				var parameters = new Dictionary<string, object> { { ActivityFlags.TextoParam, fullPath } };
				this.LaunchActivity(ActivityFlags.PrintDisplay, parameters);
            }
            else
            {
                ActivityHelper.ShowQuestion(this, "Desejar baixar novamente a nota pendente?", "Nota Pendente", 
                    () =>
                    { 
                        BaixarNotaPendente(pendencia, filename);
                        //ActivityHelper.AbrirPdf(this, fullPath);
						var parameters = new Dictionary<string, object> { { ActivityFlags.TextoParam, fullPath } };
						this.LaunchActivity(ActivityFlags.PrintDisplay, parameters);
                    }, 
                    () =>
                    {
                        //ActivityHelper.AbrirPdf(this, fullPath);
						var parameters = new Dictionary<string, object> { { ActivityFlags.TextoParam, fullPath } };
						this.LaunchActivity(ActivityFlags.PrintDisplay, parameters);
                    });
            }
		}

        private void BaixarNotaPendente(PendenciaDto pendencia, string filename)
        {
            string fullPath;
            var stream = ApplicationController.Application.Download(pendencia.LinkPagamento);
            ApplicationController.Application.SaveStreamOnApplicationDisk(filename,
                stream,
                out fullPath);
            stream.Close();
            ApplicationController.Application.SetPermissionForAll(fullPath);
        }

		public void CarregarClientePendente (ClienteDto cliente)
		{
			Cliente = cliente;

            BuildList.Use(_listPendencia)
                .Render(cliente.Pendencias, 
                    (p, pendencia) =>
                    {
                        var layout = BuildRelativeLayout.Create(this)
                            .SetText(String.Format("Documento: {0}", pendencia.Documento), 15, 5, 0, 0,
                                (t, l) =>
                                {
                                    t.SetTypeface(null, TypefaceStyle.Bold);
                                    t.SetTextSize(ComplexUnitType.Px, 14);
                                    //t.SetTextColor(textColor);
                                })
                            .SetText(String.Format("Valor total: {0:c}", pendencia.ValorTotal ), 15, 25, 0, 0, 
                                (t, l) =>
                                {
                                    t.SetTextSize(ComplexUnitType.Px, 13);
                                    //t.SetTextColor(textColor);
                                })
                            .SetText(String.Format("Valor em aberto: {0:c}", pendencia.ValorEmAberto), 15, 45, 0, 5,
                                (t, l) =>
                                {
                                    t.SetTextSize(ComplexUnitType.Px, 13);
                                    //t.SetTextColor(textColor);
                                })
							.SetText(String.Format("Emissão: {0}", pendencia.DataEmissao.HasValue ? String.Format("{0:dd/MM/yy}", pendencia.DataEmissao.Value) : String.Empty), 15, 65, 0, 5,
                                (t, l) =>
                                {
                                    t.SetTextSize(ComplexUnitType.Px, 13);
                                    //t.SetTextColor(textColor);
                                })
							.SetText(String.Format("Vencimento: {0}", pendencia.DataVencimento.HasValue ? String.Format("{0:dd/MM/yy}", pendencia.DataVencimento.Value) : String.Empty), 15, 85, 0, 5,
                                (t, l) =>
                                {
                                    t.SetTextSize(ComplexUnitType.Px, 13);
                                    //t.SetTextColor(textColor);
                                })
                            .SetImage(Resource.Drawable.img_money, 0, 10, 0, 0, 
                                (t, l) =>
                                {
                                    l.AddRule(LayoutRules.AlignParentRight);
                                    l.AddRule(LayoutRules.CenterVertical);
                                    l.Width = 80;
                                    t.Click += delegate { ShowNotaPendente(pendencia); };
                                    t.Focusable = false;
                                    t.FocusableInTouchMode = false;
                                })
                            .Build();
                        //layout.SetBackgroundColor(backColor);
                        return layout;
                    })
                .Build();
		}
	}
}