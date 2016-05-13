using System;
using Android.App;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Widget;
using INetSales.Objects.Dtos;

namespace INetSales.AndroidUi.Activities.Cliente
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme")]
    public class ClienteDetailGeralActivity : BaseActivity
    {
        #region TipoPessoa
        private RadioButton _rbPessoaFisica;
        private RadioButton _rbPessoaJuridica;
        public TipoPessoaEnum TipoPessoa
        {
            get
            {
                return _rbPessoaFisica.Checked ? TipoPessoaEnum.Fisica : TipoPessoaEnum.Juridica;
            }
            set
            {
                switch (value)
                {
                    case TipoPessoaEnum.Fisica:
                        _rbPessoaFisica.Checked = true;
                        _lbNomeFantasiaCliente.Text = "Nome:";
                        _lbDocumentoCliente.Text = "CPF:";
                        break;
                    case TipoPessoaEnum.Juridica:
                        _rbPessoaJuridica.Checked = true;
                        _lbNomeFantasiaCliente.Text = "Nome Fantasia:";
                        _lbDocumentoCliente.Text = "CNPJ:";
                        break;
                }
            }
        }
        #endregion

        #region Raz鉶 Social
        private TextView _lbRazaoSocialCliente;
        private EditText _txtRazaoSocialCliente;
        public string RazaoSocial 
        { 
            get { return _txtRazaoSocialCliente.Text.Trim(); }
            set { _txtRazaoSocialCliente.Text = value; }
        }
        #endregion

        #region Nome Fantasia
        private TextView _lbNomeFantasiaCliente;
        private EditText _txtNomeFantasiaCliente;
        public string NomeFantasia
        {
            get { return _txtNomeFantasiaCliente.Text.Trim(); }
            set { _txtNomeFantasiaCliente.Text = value; }
        }
        #endregion

        #region Documento
        private TextView _lbDocumentoCliente;
        private EditText _txtDocumentoCliente;
        public string Documento
        {
            get { return _txtDocumentoCliente.Text.Trim(); }
            set { _txtDocumentoCliente.Text = value; }
        }
        #endregion

        #region Email
        private EditText _txtEmailCliente;
        public string Email
        {
            get { return _txtEmailCliente.Text.Trim(); }
            set { _txtEmailCliente.Text = value; }
        }
        #endregion

        #region Telefone
        private EditText _txtTelefone;
        public string Telefone
        {
            get { return _txtTelefone.Text.Trim(); }
            set { _txtTelefone.Text = value; }
        }
        #endregion

        public void Desabilitar()
        {
            _rbPessoaFisica.Enabled =
            _rbPessoaJuridica.Enabled =
            _txtRazaoSocialCliente.Enabled =
            _txtNomeFantasiaCliente.Enabled =
            _txtEmailCliente.Enabled =
            _txtDocumentoCliente.Enabled =
            _txtTelefone.Enabled =
            false;
        }

        public void Habilitar()
        {
            _rbPessoaFisica.Enabled =
            _rbPessoaJuridica.Enabled =
            _txtRazaoSocialCliente.Enabled =
            _txtNomeFantasiaCliente.Enabled =
            _txtEmailCliente.Enabled =
            _txtDocumentoCliente.Enabled =
            _txtTelefone.Enabled =
            true;
        }

        public bool Validar()
        {
            return true;
        }

		protected override void OnBeginView(Bundle bundle)
        {
            SetContentView(Resource.Layout.ClienteDetailGeral);

            IniciarActivity();
        }

        private void IniciarActivity()
        {
            var tabParent = Parent as ClienteDetailActivity;
            tabParent.RegistrarGeralActivity(this);

            _rbPessoaFisica = this.FindViewById<RadioButton>(Resource.Id.rbPessoaFisica);
            _rbPessoaJuridica = this.FindViewById<RadioButton>(Resource.Id.rbPessoaJuridica);
            _lbRazaoSocialCliente = this.FindViewById<TextView>(Resource.Id.lbRazaoSocialCliente);
            _txtRazaoSocialCliente = this.FindViewById<EditText>(Resource.Id.txtRazaoSocialCliente);
            _lbNomeFantasiaCliente = this.FindViewById<TextView>(Resource.Id.lbNomeFantasiaCliente);
            _txtNomeFantasiaCliente = this.FindViewById<EditText>(Resource.Id.txtNomeFantasiaCliente);
            _lbDocumentoCliente = this.FindViewById<TextView>(Resource.Id.lbDocumentoCliente);
            _txtDocumentoCliente = this.FindViewById<EditText>(Resource.Id.txtDocumentoCliente);
            _txtEmailCliente = this.FindViewById<EditText>(Resource.Id.txtEmailCliente);
            _txtTelefone = this.FindViewById<EditText>(Resource.Id.txtTelefone);

            _txtDocumentoCliente.InputType = InputTypes.ClassNumber;
            _txtTelefone.InputType = InputTypes.ClassNumber;

            _rbPessoaFisica.SetTextSize(ComplexUnitType.Px, 18);
            _rbPessoaJuridica.SetTextSize(ComplexUnitType.Px, 18);
            _rbPessoaFisica.Checked = true;

            _lbNomeFantasiaCliente.Text = "Nome:";
            _lbDocumentoCliente.Text = "CPF:";
            _lbRazaoSocialCliente.Enabled = false;
            _txtRazaoSocialCliente.Enabled = false;
            _txtRazaoSocialCliente.Text = String.Empty;

            _rbPessoaFisica.Click += delegate
            {
                _lbNomeFantasiaCliente.Text = "Nome:";
                _lbDocumentoCliente.Text = "CPF:";
                _lbRazaoSocialCliente.Enabled = false;
                _txtRazaoSocialCliente.Enabled = false;
                _txtRazaoSocialCliente.Text = String.Empty;
            };

            _rbPessoaJuridica.Click += delegate
            {
                _lbNomeFantasiaCliente.Text = "Nome Fantasia:";
                _lbDocumentoCliente.Text = "CNPJ:";
                _lbRazaoSocialCliente.Enabled = true;
                _txtRazaoSocialCliente.Enabled = true;
            };
        }
    }
}