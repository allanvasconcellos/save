using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IPesquisaDb
    {
        public IEnumerable<ProdutoDto> GetProdutosPrecoPesquisa(DateTime lastQuery)
        {
            //var produtos = new List<ProdutoDto>();
            ////_comp.getProdutoPesquisa()
            //produtos.Add(new ProdutoDto { Codigo = "108510" });
            //produtos.Add(new ProdutoDto { Codigo = "108511" });
            //produtos.Add(new ProdutoDto { Codigo = "108518" });
            //produtos.Add(new ProdutoDto { Codigo = "108520" });
            //return produtos;

            var produtos = new List<ProdutoDto>();

            TratarInvokeWeb("GetProdutosPrecoPesquisa",
            () =>
            {
                string sendInfo = String.Format("<produtos chave=\"{0}\"></produtos>", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                return _comp.getProdutoPesquisa(sendInfo);
            },
            (xml, r) =>
            {
                var idNodes = xml.GetElementsByTagName("id_produto");
                for (int i = 0; i < idNodes.Count; ++i)
                {
                    var produto = new ProdutoDto
                    {
                        Codigo = idNodes[i].InnerText,
                    };
                    produtos.Add(produto);
                }
                return String.Empty;
            });

            return produtos;
        }

        public void EnviarPesquisa(UsuarioDto usuario, ClienteDto cliente, Dictionary<TipoPesquisaPergunta, bool> perguntas, Dictionary<string, bool> categorias, Dictionary<string, double> precos)
        {
            Logger.Info(false, "Iniciando envio de visita - usuário {0} - cliente {1}", usuario.Username, cliente.NomeFantasia);

            TratarInvokeWeb("EnvioVisita",
            () =>
            {
                var builder = new StringBuilder();
                var numberFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat;
                numberFormat.NumberDecimalSeparator = ".";

                builder.AppendFormat("<produtos chave=\"{0}\">", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                builder.AppendFormat("<login_vendedor>{0}</login_vendedor>", usuario.Codigo);
                builder.Append("      <visita>");
                builder.AppendFormat("  <id_clifor>{0}</id_clifor>", cliente.Codigo);
                builder.AppendFormat("  <st_expositor_modelez>{0}</st_expositor_modelez>", GetTextSimNaoPesquisa(perguntas[TipoPesquisaPergunta.ExpositorModelez]));
                builder.AppendFormat("  <st_expositor_30_cm>{0}</st_expositor_30_cm>", GetTextSimNaoPesquisa(perguntas[TipoPesquisaPergunta.Expositor50cm]));
                builder.AppendFormat("  <st_cliente_tem_promocao>{0}</st_cliente_tem_promocao>", GetTextSimNaoPesquisa(perguntas[TipoPesquisaPergunta.ClientePromocao]));
                builder.AppendFormat("  <st_cliente_tem_planograma>{0}</st_cliente_tem_planograma>", GetTextSimNaoPesquisa(perguntas[TipoPesquisaPergunta.TemPlanograma]));
                builder.Append("        <pesquisa_categoria>");
                foreach (var categoriaResposta in categorias)
                {
                    builder.Append("        <categoria>");
                    builder.AppendFormat("      <codigo>{0}</codigo>", categoriaResposta.Key);
                    builder.AppendFormat("      <resposta>{0}</resposta>", GetTextSimNaoPesquisa(categoriaResposta.Value));
                    builder.Append("        </categoria>");
                }
                builder.Append("        </pesquisa_categoria>");
                builder.Append("        <pesquisa_preco>");
                foreach (var precoResposta in precos)
                {
                    builder.Append("        <produto>");
                    builder.AppendFormat("      <codigo>{0}</codigo>", precoResposta.Key);
                    builder.AppendFormat("      <resposta>{0}</resposta>", Convert.ToString(precoResposta.Value, numberFormat));
                    builder.Append("        </produto>");
                }
                builder.Append("        </pesquisa_preco>");
                builder.Append("     </visita>");
                builder.Append("     </produtos>");
                Logger.Info(false, "Enviado a visita - usuário {0} - cliente {1} - xml {2}", usuario.Username, cliente.NomeFantasia, builder.ToString());
                string retorno = _comp.setVisita(builder.ToString());
                //string retorno = "ok";
                if (!String.IsNullOrEmpty(retorno))
                {
                    if (retorno.ToLower().Equals("ok"))
                    {
                        Logger.Warn(false, "Pesquisa - Retorno \"ok\" - usuário {0} - cliente {1}", usuario.Username, cliente.NomeFantasia);
                        return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "001", retorno);
                    }
                    if (!retorno.ToLower().Contains("<retorno><codigo>"))
                    {
                        Logger.Warn(false, "Pesquisa - Retorno sem formato xml: {0} - usuário {1} - cliente {2}", retorno, usuario.Username, cliente.NomeFantasia);
                        return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "099", retorno);
                    }
                }
                return retorno;
            },
            (doc, r) =>
            {
                return String.Empty;
            });

        }

        private string GetTextSimNaoPesquisa(bool resposta)
        {
            return resposta ? "S" : "N";
        }
    }
}