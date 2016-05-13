using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Support.V4.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.DbInterfaces;
using INetSales.ViewController.Lib;

namespace INetSales.AndroidUi.Activities.Pesquisa
{
    public class PrecoPesquisaFragment : Fragment
    {
        private readonly Dictionary<string, double> _mapPrecoResposta;

        public PrecoPesquisaFragment()
        {
            _mapPrecoResposta = new Dictionary<string, double>();
        }

        public Dictionary<string, double> MapRespostas
        {
            get { return _mapPrecoResposta; }
        }

        public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
        {
            var offline = DbHelper.GetOffline<IOfflinePesquisaDb>();
            var produtos = offline.ObterProdutosPrecoPesquisa();

            // Carregar map respostas
            foreach (string produtoCode in produtos.Select(p => p.Codigo))
            {
                if (!_mapPrecoResposta.Keys.Contains(produtoCode))
                {
                    _mapPrecoResposta.Add(produtoCode, 0);
                }
            }

            var layoutFragment = BuildLayout.Create(Activity, Orientation.Vertical)
                .SetText("Pesquisa de preços de produtos", 10, 20, 0, 10, 
                    t => t.SetTextSize(ComplexUnitType.Px, 14))
                .SetList(produtos, 0, 0, 
                    (p, item) => // Render
                    {
                        var layoutList = BuildRelativeLayout.Create(Activity)
                                .SetText(item.Nome, 10, 10, 0, 10,
                                    (b, lparam) =>
                                    {
                                        lparam.AddRule(LayoutRules.CenterVertical);
                                    })
                                .SetEdit(String.Empty, 10, 0, 15, 0, 
                                    (control, lparam) =>
                                    {
                                        lparam.AddRule(LayoutRules.CenterVertical);
                                        lparam.AddRule(LayoutRules.AlignParentRight);
                                        control.TextChanged += 
                                            (sender, e) =>
                                            {
                                                if(!String.IsNullOrEmpty(control.Text))
                                                {
                                                    double valor = 0;
                                                    _mapPrecoResposta[item.Codigo] = 0;
                                                    var numberFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat;
                                                    numberFormat.NumberDecimalSeparator = ".";
                                                    if (Double.TryParse(control.Text, NumberStyles.Currency, numberFormat, out valor))
                                                    {
                                                        _mapPrecoResposta[item.Codigo] = valor;
                                                    }
                                                }
                                            };
                                        control.InputType = InputTypes.NumberFlagDecimal | InputTypes.ClassNumber;
                                        control.SetWidth(100);
                                        //control.ImeOptions = ImeAction.Done;
                                        control.FocusChange +=
                                            (sender, e) =>
                                            {
                                                if (e.HasFocus)
                                                {
                                                    var inputMethodManager = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
                                                    inputMethodManager.ToggleSoftInputFromWindow(control.WindowToken, ShowSoftInputFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                                                }
                                            };
                                    })
                                .Build();
                        return layoutList;
                    },
                    control =>
                    {
                        control.DescendantFocusability = DescendantFocusability.AfterDescendants;
                    })
                .Build();
            return layoutFragment;
        }
    }
}