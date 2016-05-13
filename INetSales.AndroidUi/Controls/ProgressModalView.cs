using System;
using Android.App;
using INetSales.ViewController;

namespace INetSales.AndroidUi.Controls
{
    public class ProgressModalView : IProgressView
    {
        private readonly Activity _activity;
        private ProgressDialog _dialog;

        public ProgressModalView(Activity activity)
        {
            _activity = activity;
        }

        #region Implementation of IProgressView

        public void Show()
        {
            Show(String.Empty);
        }

        public void Show(string title)
        {
            _dialog = new ProgressDialog(_activity);
            _dialog.SetCancelable(false);
            if (!String.IsNullOrEmpty(title))
            {
                _dialog.SetMessage(title);
                _dialog.SetProgressStyle(ProgressDialogStyle.Horizontal);
            }
            else
            {
                _dialog.SetMessage("Processando");
                _dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            }
            // reset the bar to the default value of 0
            _dialog.Progress = 0;

            // get the maximum value
            _dialog.Max = 100;
            // display the progressbar
            _dialog.Show();
        }

        public void Close()
        {
            _dialog.Dismiss();
        }

        public void UpdateStatus(double progress)
        {
            _activity.RunOnUiThread(() => _dialog.Progress = Convert.ToInt32(progress));
        }

        #endregion
    }
}