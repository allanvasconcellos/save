using System;
using Android.App;
using Android.Widget;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi.Controls
{
    public class AcessoModalView : IAcessoChildView
    {
        private readonly Activity _activity;

        public AcessoModalView(Activity activity)
        {
            _activity = activity;
        }

        public void Close()
        {
            //_dialog.Dismiss();
            //Logger.Debug("Fechou progresso");
        }

        public void ShowLogin(Func<string, string, bool> ok)
        {
            _activity.ShowDialog("Acesso", dialog =>
            {
                var loginView = _activity.LayoutInflater.Inflate(Resource.Layout.Login, null);
                var txtUserName = loginView.FindViewById<EditText>(Resource.Id.txtUsuario);
                var txtPassword = loginView.FindViewById<TextView>(Resource.Id.txtSenha);
                var btnLogin = loginView.FindViewById<Button>(Resource.Id.btnLogin);
                btnLogin.Click += delegate
                {
                    if (ok(txtUserName.Text, txtPassword.Text))
                    {
                        dialog.Dismiss();
                    }
                };
                return loginView;
            });
        }

    }
}