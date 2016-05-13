using System;
using Android.App;
using Android.Views;
using Android.Widget;

namespace INetSales.AndroidUi.Controls
{
    public class MultipleActionModalView
    {
        public class ActionInfo
        {
            public string Text { get; private set; }
            public Action Action { get; private set; }

            public ActionInfo(string text, Action action)
            {
                Text = text;
                Action = action;
            }
        }

        private readonly Activity _activity;

        public MultipleActionModalView(Activity activity)
        {
            _activity = activity;
        }

        public void Show(string title, params ActionInfo[] actions)
        {
            _activity.ShowDialog(title, dialog =>
            {
                var builder = BuildLayout.Create(_activity, Orientation.Vertical);
                foreach (var action in actions)
                {
                    builder = builder.SetButton(action.Text, 20, 10, 20, 10, button =>
                    {
                        //button.Background = 
                        var action1 = action;
                        button.Click += (sender, e) =>
                        {
                            dialog.Dismiss();
                            action1.Action();
                        };
                    });
                }
                return builder.Build();
            });
        }
    }
}