using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.DbInterfaces;
using INetSales.ViewController.Lib;

namespace INetSales.AndroidUi.Activities.Pesquisa
{
    public class CategoriaPesquisaFragment : Fragment
    {
        private readonly Dictionary<string, bool> _mapGrupoResposta;

        public CategoriaPesquisaFragment()
        {
            _mapGrupoResposta = new Dictionary<string, bool>();
        }

        public Dictionary<string, bool> MapRespostas
        {
            get { return _mapGrupoResposta; }
        }

        public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
        {
            var offline = DbHelper.GetOffline<IOfflinePesquisaDb>();
            var grupos = offline.ObterGruposPesquisa();
            var respostas = new List<ControlItem>
                                {
                                    new ControlItem {Id = 1, Descricao = "Sim"},
                                    new ControlItem {Id = 2, Descricao = "Não", IsDefault = true,},
                                };

            // Carregar map respostas
            foreach (string grupoCode in grupos.Select(g => g.Codigo))
            {
                if (!_mapGrupoResposta.Keys.Contains(grupoCode))
                {
                    _mapGrupoResposta.Add(grupoCode, false);
                }
            }

            var layoutFragment = BuildLayout.Create(Activity, Orientation.Vertical)
                .SetText("O cliente possui essas categorias?", 10, 20, 0, 10, t => t.SetTextSize(ComplexUnitType.Px, 14))
                .SetList(grupos, 0, 0, (p, item) =>
                {
                    var layoutList =
                        BuildLayout.Create(Activity, Orientation.Horizontal)
                            .SetText(item.Nome, 10, 10, 0, 10)
                            .SetRadio(respostas, 10, 0, 50, 0, control =>
                            {
                                control.CheckedChange += (sender, e) =>
                                {
                                    if (e.CheckedId == 1)
                                    {
                                        _mapGrupoResposta[item.Codigo] = true;
                                        return;
                                    }
                                    _mapGrupoResposta[item.Codigo] = false;
                                };
                                control.Orientation = Orientation.Horizontal;
                                control.SetGravity(GravityFlags.Right);
                            })
                            .Build();
                    return layoutList;
                })
                .Build();
            return layoutFragment;
        }

    }
}