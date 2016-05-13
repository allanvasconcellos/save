using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IProdutoDb
    {
        #region Implementation of IDb<ProdutoDto>

        IEnumerable<ProdutoDto> IDb<ProdutoDto>.GetAll(UsuarioDto usuario)
        {
            var produtos = new List<ProdutoDto>();

            TratarInvokeWeb("GetAllProduto", 
            () =>
                {
                    string sendInfo = String.Format("<produtos chave=\"{0}\"><id_tabela_preco>{1}</id_tabela_preco></produtos>",
                        _configuracao.ChaveIntegracao ?? ChaveIntegracao, _configuracao.CodigoTabelaPreco ?? CodigoTabelaPreco);
                    if(usuario != null)
                    {
                        sendInfo = String.Format("<produtos chave=\"{0}\"><login_vendedor>{1}</login_vendedor><id_tabela_preco>{2}</id_tabela_preco></produtos>",
                                        _configuracao.ChaveIntegracao ?? ChaveIntegracao, usuario.Username, _configuracao.CodigoTabelaPreco ?? CodigoTabelaPreco);   
                    }
                    return _comp.getProduto(sendInfo);
                },
            (doc, xml) =>
                {
                    var codigoNodes = doc.GetElementsByTagName("id_produto");
                    var nomeNodes = doc.GetElementsByTagName("nm_produto");
                    var valorNodes = doc.GetElementsByTagName("vl_venda");
                    var saldoNodes = doc.GetElementsByTagName("saldo");
                    var categoriaNodes = doc.GetElementsByTagName("id_categoria");
                    for (int i = 0; i < codigoNodes.Count; ++i)
                    {
                        //var numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                        var numberFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat;
                        numberFormat.NumberDecimalSeparator = ".";
                        var produtoRetornado = new ProdutoDto
                        {
                            Codigo = codigoNodes[i].InnerText,
                            Nome = nomeNodes[i].InnerText,
                            ValorUnitario = Double.Parse(valorNodes[i].InnerText, numberFormat),
                            QuantidadeDisponivel = Math.Abs(Convert.ToInt32(Decimal.Parse(saldoNodes[i].InnerText, numberFormat))),
                        };
                        if (!String.IsNullOrEmpty(categoriaNodes[i].InnerText.Trim()))
                        {
                            produtoRetornado.Grupo = new GrupoDto() { Codigo = categoriaNodes[i].InnerText.Trim(), };
                        }
                        produtos.Add(produtoRetornado);
                    }
                    return String.Empty;
                });

            foreach (var produtoGroup in produtos.GroupBy(p => p.Codigo))
            {
                if(produtoGroup.Count() > 1)
                {
                    var produtoDuplicado = produtos.FirstOrDefault(p => p.Codigo.Equals(produtoGroup.Key));
                    decimal somatorioQuantidade = produtos
                            .Where(p => p.Codigo.Equals(produtoGroup.Key))
                            .Sum(p => p.QuantidadeDisponivel);
                    produtoDuplicado.QuantidadeDisponivel = somatorioQuantidade;
                    produtos.RemoveAll(p => p.Codigo.Equals(produtoGroup.Key));
                    produtos.Add(produtoDuplicado);
                }
            }
            return produtos;
        }

        public void Save(ProdutoDto dto)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}