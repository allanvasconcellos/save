using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.ViewController;
using Android.Graphics.Drawables;
using INetSales.ViewController.Views;
using INetSales.Objects;
using Android.Runtime;

namespace INetSales.AndroidUi
{
    public abstract class BaseTabActivity : TabActivity, IView
    {
        protected LinearLayout BottomLayout { get; private set; }

        protected abstract void OnBeginView(Bundle bundle);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			RequestedOrientation = ScreenOrientation.Portrait;
            
            // Carregar a tab
            SetContentView(Resource.Layout.TabLayout);

            BottomLayout = FindViewById<LinearLayout>(Resource.Id.tabBottomLayout);

			AppDomain.CurrentDomain.UnhandledException += (s,e)=>
			{
				Logger.Debug("AppDomain.CurrentDomain.UnhandledException: {0}. IsTerminating: {1}", e.ExceptionObject, e.IsTerminating);
			};

			AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
			{
				Logger.Debug("AndroidEnvironment.UnhandledExceptionRaiser: {0}. IsTerminating: {1}", e.Exception, e.Handled);
				e.Handled = true;
			};

            OnBeginView(bundle);
            TabHost.CurrentTab = 0;
            TabHost.TabChanged += TabChanged;
        }

        private void TabChanged(object sender, TabHost.TabChangeEventArgs e)
        {
            OnTabChange(e.TabId);
        }

        protected abstract void OnTabChange(string tag);

        protected void AddTab<TActivity>(string tag, string title, Drawable image = null) where TActivity : Activity
        {
            var intent = new Intent(this, typeof(TActivity));
            intent.AddFlags(Android.Content.ActivityFlags.NewTask);

            TabHost.TabSpec spec = TabHost.NewTabSpec(tag);
            if (image != null)
            {
                spec.SetIndicator(title, image);
            }
            else
            {
                spec.SetIndicator(title);
            }
            spec.SetContent(intent);
            TabHost.AddTab(spec);
        }

        #region Implementation of IView

        public void MakeQuestion(string question, Action ok, Action cancel)
        {
			ActivityHelper.ShowQuestion(this, question, "SAVE", ok, cancel);
        }

        public void UpdateViewTitle(params string[] titleLines)
        {
			this.SetTitle("SAVE", titleLines);
        }

        public void ShowMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        public void ShowModalMessage(string title, string message, Action ok)
        {
            ActivityHelper.ShowMessageBox(this, message, title, (sender, e) => { });
        }

        public virtual void Next()
        {
        }

        public void CloseView()
        {
            Finish();
        }

        public virtual IProgressView ShowProgressView(string title)
        {
            var progress = new ProgressModalView(this);
            progress.Show(title);
            return progress;
        }

        public IConsoleView GetConsoleView(string title)
        {
            return new ConsoleModalView(this);
        }

        public IConfiguracaoChildView GetConfiguracaoView(string title)
        {
            return new ConfiguracaoModalView(this);
        }

        public void ExecuteOnBackground(Action execute)
        {
#if DEBUG
            execute();
            return;
#endif
            new Thread(() => execute()).Start();
        }

        public void ExecuteOnUI(Action execute)
        {
            RunOnUiThread(execute);
        }

        #endregion
    }
}
