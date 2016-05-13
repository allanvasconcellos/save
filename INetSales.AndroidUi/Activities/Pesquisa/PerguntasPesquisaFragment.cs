using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects;

namespace INetSales.AndroidUi.Activities.Pesquisa
{
    public class PerguntasPesquisaFragment : Fragment
    {
        private readonly Dictionary<TipoPesquisaPergunta, bool> _mapPerguntaResposta;

        public PerguntasPesquisaFragment()
        {
            _mapPerguntaResposta = new Dictionary<TipoPesquisaPergunta, bool>();
        }

        public Dictionary<TipoPesquisaPergunta, bool> MapRespostas
        {
            get { return _mapPerguntaResposta; }
        }

        public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
        {
            var perguntas = new List<ControlItem>
                                {
                                    new ControlItem {Id = (int)TipoPesquisaPergunta.ExpositorModelez, Descricao = "Esse cliente possui expositor modelez?"},
                                    new ControlItem {Id = (int)TipoPesquisaPergunta.Expositor50cm, Descricao = "Esse cliente possui expositor a 50 cm do produto?"},
                                    new ControlItem {Id = (int)TipoPesquisaPergunta.ClientePromocao, Descricao = "Esse cliente tem promoção?"},
                                    new ControlItem {Id = (int)TipoPesquisaPergunta.TemPlanograma, Descricao = "Esse cliente tem planograma?"},
                                };

            var respostas = new List<ControlItem>
                                {
                                    new ControlItem {Id = 1, Descricao = "Sim"},
                                    new ControlItem {Id = 2, Descricao = "Não", IsDefault = true,},
                                };

            // Carregar map respostas
            foreach (TipoPesquisaPergunta perguntaId in perguntas.Select(p => (TipoPesquisaPergunta)p.Id))
            {
                if (!_mapPerguntaResposta.Keys.Contains(perguntaId))
                {
                    _mapPerguntaResposta.Add(perguntaId, false);
                }
            }

            var layoutFragment = BuildLayout.Create(Activity, Orientation.Vertical)
                .SetList(perguntas, 0, 0, (p, item) =>
                {
                    var layoutList =
                        BuildLayout.Create(Activity, Orientation.Horizontal)
                            .SetText(item.Descricao, 10, 10, 0, 10)
                            .SetRadio(respostas, 10, 0, 30, 0, control =>
                            {
                                control.CheckedChange += (sender, e) =>
                                {
                                    if (e.CheckedId == 1)
                                    {
                                        _mapPerguntaResposta[(TipoPesquisaPergunta)item.Id] = true;
                                        return;
                                    }
                                    _mapPerguntaResposta[(TipoPesquisaPergunta)item.Id] = false;
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