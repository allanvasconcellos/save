using System;
using Android.App;
using Android.Views;
using Android.Widget;
using INetSales.Objects;
using INetSales.Objects.Dtos;
using INetSales.ViewController;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi.Controls
{
	public class ConfiguracaoModalView : IConfiguracaoChildView
    {
        private readonly Activity _activity;
        private EditText _txtChave;
        private EditText _txtTabelaPreco;
        private EditText _txtCnpjEmpresa;
        private EditText _txtUrlErp;
        private EditText _txtCampoEspecie;
        private EditText _txtCampoMarca;

        public ConfiguracaoModalView(Activity activity)
        {
            _activity = activity;
        }

        #region Implementation of IConfiguracaoChildView

		public void ShowSelecaoTipoVenda (Action<TipoVendaEnum> tipovendaSelecionadaAction, Action terminouSelecao)
		{
			_activity.RunOnUiThread(
				() =>
				{
					_activity.ShowDialog("Tipo Venda", dialog =>
						{
							var layout =
								BuildLayout.Create(_activity, Orientation.Vertical)
									.SetButton("Pré-venda", 10, 10, 10, 10,
										b =>
										{
											b.Gravity = GravityFlags.CenterHorizontal;
											b.Click +=
												(sender, e) =>
											{
												tipovendaSelecionadaAction(TipoVendaEnum.PreVenda);
												dialog.Dismiss();
												terminouSelecao();
											};
										})
									.SetButton("Pronta-entrega", 10, 10, 10, 10,
										b =>
										{
											b.Gravity = GravityFlags.CenterHorizontal;
											b.Click +=
												(sender, e) =>
											{
												tipovendaSelecionadaAction(TipoVendaEnum.ProntaEntrega);
												dialog.Dismiss();
												terminouSelecao();
											};
										})
									.Build();
							dialog.SetCancelable(false);
							return layout;
						});
				});
		}

        public void ShowSelecaoEmpresa(Action<EmpresaEnum> empresaSelecionadaAction, Action terminouSelecao)
        {
            _activity.RunOnUiThread(
                () =>
                {
                    _activity.ShowDialog("Configuração", dialog =>
                    {
                        var layout =
                            BuildLayout.Create(_activity, Orientation.Vertical)
	                                .SetButton("Teste", 10, 10, 10, 10,
		                                b =>
		                                {
		                                    b.Gravity = GravityFlags.CenterHorizontal;
		                                    b.Click +=
		                                        (sender, e) =>
		                                        {
		                                            empresaSelecionadaAction(EmpresaEnum.Teste);
		                                            dialog.Dismiss();
		                                            terminouSelecao();
		                                        };
		                                })
									.SetButton("Produção", 10, 10, 10, 10,
										b =>
										{
											b.Gravity = GravityFlags.CenterHorizontal;
											b.Click +=
												(sender, e) =>
											{
												empresaSelecionadaAction(EmpresaEnum.Producao);
												dialog.Dismiss();
												terminouSelecao();
											};
										})
									.SetButton("Outros", 10, 10, 10, 10,
										b =>
										{
											b.Gravity = GravityFlags.CenterHorizontal;
											b.Click +=
												(sender, e) =>
											{
												empresaSelecionadaAction(EmpresaEnum.Outros);
												dialog.Dismiss();
												terminouSelecao();
											};
										})
                                .Build();
                        dialog.SetCancelable(false);
                        return layout;
                    });
                });
        }

        public void Show(ConfiguracaoDto configuracao, Func<string, string, string, string, string, string, bool> ok, Action terminouConfiguracao)
        {
            _activity.RunOnUiThread(
                () =>
                {
                    _activity.ShowDialog("Configuração", dialog =>
                    {
                        var layout =
                            BuildLayout.Create(_activity, Orientation.Vertical)
                                .SetText("Chave de integração:", 10, 5)
                                .SetEdit(configuracao.ChaveIntegracao, 10, 5, 10, 0, 
                                    e =>
                                    {
                                        _txtChave = e;
                                        _txtChave.SetWidth(500);
                                    })
                                .SetText("Código tabela de preço:", 10, 5)
                                .SetEdit(configuracao.CodigoTabelaPreco, 10, 5, 10, 0, 
                                    e =>
                                    { 
                                        _txtTabelaPreco = e;
                                        _txtTabelaPreco.SetWidth(500);
                                    })
                                .SetText("Cnpj Empresa:", 10, 5)
                                .SetEdit(configuracao.CnpjEmpresa, 10, 5, 10, 0, 
                                    e => 
                                    { 
                                        _txtCnpjEmpresa = e;
                                        _txtCnpjEmpresa.SetWidth(500);
                                    })
                                .SetText("Espécie:", 10, 5)
                                .SetEdit(configuracao.CampoEspecie, 10, 5, 10, 0,
                                    e =>
                                    {
                                        _txtCampoEspecie = e;
                                        _txtCampoEspecie.SetWidth(500);
                                    })
                                .SetText("Marca:", 10, 5)
                                .SetEdit(configuracao.CampoMarca, 10, 5, 10, 0,
                                    e =>
                                    {
                                        _txtCampoMarca = e;
                                        _txtCampoMarca.SetWidth(500);
                                    })
                                .SetText("Endereço web integração:", 10, 5)
                                .SetEdit(configuracao.UrlWebService, 10, 5, 10, 0, 
                                    e =>
                                    {
                                        _txtUrlErp = e;
                                        _txtUrlErp.SetWidth(500);
                                    })
                                .SetButton("Salvar", 0, 10, 0, 0, 
                                    b =>
                                    {
                                        b.Gravity = GravityFlags.Center;
                                        b.SetWidth(200);
                                        b.Click += delegate
                                                    {
                                                        if (ValidarInfo() &&
                                                            ok(_txtChave.Text.Trim(),
                                                                _txtTabelaPreco.Text.Trim(),
                                                                _txtCnpjEmpresa.Text.Trim(),
                                                                _txtUrlErp.Text.Trim(),
                                                                _txtCampoMarca.Text.Trim(),
                                                                _txtCampoEspecie.Text.Trim()))
                                                        {
                                                            dialog.Dismiss();
                                                            terminouConfiguracao();
                                                        }
                                                    };
                                    })
                                .Build();
                        dialog.SetCancelable(false);
                        return layout;
                    });
                });
        }

        private bool ValidarInfo()
        {
            Guid chave = Guid.Empty;
            var view = (IView) _activity;
            if(!Guid.TryParse(_txtChave.Text, out chave))
            {
                view.ShowMessage("A chave de integração é inválida");
                _txtChave.RequestFocus();
                return false;
            }
            if(String.IsNullOrEmpty(_txtTabelaPreco.Text.Trim()))
            {
                view.ShowMessage("O código de tabela de preço é inválido");
                _txtTabelaPreco.RequestFocus();
                return false;
            }
            if (String.IsNullOrEmpty(_txtCnpjEmpresa.Text.Trim()) && Utils.ValidarCnpj(_txtCnpjEmpresa.Text.Trim()))
            {
                view.ShowMessage("O cnpj da empresa é inválido");
                _txtCnpjEmpresa.RequestFocus();
                return false;
            }
            if (String.IsNullOrEmpty(_txtCampoEspecie.Text.Trim()))
            {
                view.ShowMessage("O campo espécie é inválido");
                _txtCampoEspecie.RequestFocus();
                return false;
            }
            if (String.IsNullOrEmpty(_txtCampoMarca.Text.Trim()))
            {
                view.ShowMessage("O campo marca é inválido");
                _txtCampoMarca.RequestFocus();
                return false;
            }
            if (String.IsNullOrEmpty(_txtUrlErp.Text.Trim()))
            {
                view.ShowMessage("O endereço web é inválido");
                _txtUrlErp.RequestFocus();
                return false;
            }
            return true;
        }

        #endregion
    }
}