using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace INetSales.AndroidUi.Activities.Cliente
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme")]
    public class ClienteDetailEnderecoActivity : BaseActivity
    {
        #region Rua
        private EditText _txtRuaEndereco;
        public string Rua
        {
            get { return _txtRuaEndereco.Text.Trim(); }
            set { _txtRuaEndereco.Text = value; }
        }
        #endregion

        #region N鷐ero
        private EditText _txtNumeroEndereco;
        public int Numero
        {
            get
            {
                string text = _txtNumeroEndereco.Text.Trim();
                int result = 0;
                return Int32.TryParse(text, out result) ? result : 0;
            }
            set { _txtNumeroEndereco.Text = value.ToString(); }
        }
        #endregion

        #region Bairro
        private EditText _txtBairro;
        public string Bairro
        {
            get { return _txtBairro.Text.Trim(); }
            set { _txtBairro.Text = value; }
        }
        #endregion

        #region Cep
        private EditText _txtCep;
        public string Cep
        {
            get { return _txtCep.Text.Trim(); }
            set { _txtCep.Text = value; }
        }
        #endregion

        #region Cidade
        private EditText _txtCidade;
        public string Cidade
        {
            get { return _txtCidade.Text.Trim(); }
            set { _txtCidade.Text = value; }
        }
        #endregion

		protected override void OnBeginView(Bundle bundle)
        {
            SetContentView(Resource.Layout.ClienteDetailEndereco);

            IniciarActivity();
        }

        private void IniciarActivity()
        {
            var tabParent = Parent as ClienteDetailActivity;
            tabParent.RegistrarEnderecoActivity(this);

            _txtRuaEndereco = this.FindViewById<EditText>(Resource.Id.txtRuaEndereco);
            _txtNumeroEndereco = this.FindViewById<EditText>(Resource.Id.txtNumeroEndereco);
            _txtBairro = this.FindViewById<EditText>(Resource.Id.txtBairro);
            _txtCep = this.FindViewById<EditText>(Resource.Id.txtCep);
            _txtCidade = this.FindViewById<EditText>(Resource.Id.txtCidade);
        }

        public bool Validar()
        {
            return true;
        }

        public bool IsDesabilitado
        {
            get { return _txtRuaEndereco != null && !_txtRuaEndereco.Enabled; }
        }

        public void Desabilitar()
        {
            _txtRuaEndereco.Enabled =
            _txtNumeroEndereco.Enabled =
            _txtBairro.Enabled =
            _txtCep.Enabled =
            _txtCidade.Enabled =
            false;
        }
    }
}