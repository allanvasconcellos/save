using System;
using System.Threading;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.ViewController;
using INetSales.ViewController.Views;
using Android.Runtime;
using INetSales.Objects;

namespace INetSales.AndroidUi
{
    public abstract class BaseActivity : FragmentActivity, IView
    {
        protected abstract void OnBeginView(Bundle bundle);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			RequestedOrientation = ScreenOrientation.Portrait;

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
			if (ok != null) {
				ActivityHelper.ShowMessageBox (this, message, title, (sender, e) => {
					ok ();
				});
			} else {
				ActivityHelper.ShowMessageBox(this, message, title, (sender, e) => { });
			}
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