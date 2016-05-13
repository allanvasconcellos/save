using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects;
using INetSales.ViewController;
using INetSales.ViewController.Sync;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi
{
	[Activity(Label = "SAVE", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/INetTheme", NoHistory = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreenActivity : BaseActivity, IFirstView, IServiceConnection, IAcessoChildView, IProgressView
    {
        private bool _firstInit;
        private ProgressBar _progressBar;

        protected override void OnBeginView(Bundle bundle)
        {
            SetContentView(Resource.Layout.SplashScreen);

            _progressBar = (ProgressBar)FindViewById(Resource.Id.pbInicio);
            _progressBar.Max = 100;

            ApplicationController.Initialize(new MonodroidApplication(this, GetPreferences(FileCreationMode.Private)));
            ApplicationController.Initialize(this);

            _firstInit = true;
        }

        protected override void OnDestroy()
        {
            Logger.Debug("Saindo da aplicacao");
            ApplicationController.Close();
            //var intent = new Intent(this, typeof(SyncService));
            //StopService(intent);
            base.OnDestroy();
        }

        protected override void OnResume()
        {
            if (!_firstInit)
            {
                ApplicationController.Resume(this);
            }
            _firstInit = false;
            base.OnResume();
        }

        public override void Next()
        {
            this.LaunchActivity(ActivityFlags.MainCategory);
        }

        public void PrepararServico(IntegratorManager manager)
        {
            //Logger.Debug("Preparando serviço");
            //var intent = new Intent(this, typeof(SyncService));
            //intent.PutExtra(SyncService.MANAGER_KEY, manager);
            //BindService(intent, this, Bind.AutoCreate);
        }

        public IAcessoChildView GetAcesso()
        {
            return this;
        }

        public IConfiguracaoChildView GetFirstConfiguracao()
        {
            return new ConfiguracaoModalView(this);
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            //Logger.Debug("Conectou no serviço");
            //var intent = new Intent(this, typeof(SyncService));
            //var binder = (SyncService.SyncBinder)service;
            //ApplicationController.Initialize(binder.Service);
            //StartService(intent);
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            //Logger.Debug("Desconectou do serviço");
            //var intent = new Intent(this, typeof(SyncService));
            //StopService(intent);
        }

        public override IProgressView ShowProgressView(string title)
        {
            if (_progressBar.Visibility == ViewStates.Visible)
            {
                return this;
            }
            return base.ShowProgressView(title);
        }

        #region Implementation of IAcessoChildView

        public void ShowLogin(Func<string, string, bool> ok)
        {
            var loginModal = new AcessoModalView(this);
            RunOnUiThread(() => loginModal.ShowLogin(ok));
        }

        #endregion

        #region Implementation of IProgressView

        public void Close()
        {
            RunOnUiThread(() => _progressBar.Visibility = ViewStates.Gone);
        }

        public void UpdateStatus(double progress)
        {
            RunOnUiThread(() => _progressBar.Progress = Convert.ToInt32(progress));
        }

        #endregion
    }
}