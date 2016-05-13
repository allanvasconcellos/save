using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace INetSales.AndroidUi.Controls
{
    public class BuildRelativeLayout
    {
        private readonly RelativeLayout _layoutMain;
        private readonly RelativeLayout _layoutCurrent;

        #region Static Members

        public static BuildRelativeLayout Use(RelativeLayout layout, Context context)
        {
            return new BuildRelativeLayout(layout);
        }

        public static BuildRelativeLayout Create(Context context)
        {
            var layout = new RelativeLayout(context);
            return new BuildRelativeLayout(layout);
        }

        #endregion

        private BuildRelativeLayout(RelativeLayout layoutMain)
        {
            _layoutMain = layoutMain;
            _layoutCurrent = _layoutMain;
        }

        public BuildRelativeLayout SetText(string text, int left = 0, int top = 0, int right = 0, int bottom = 0, Action<TextView, RelativeLayout.LayoutParams> controlText = null)
        {
            var control = new TextView(_layoutMain.Context);
            var lp = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                       ViewGroup.LayoutParams.WrapContent);
            control.Text = text;

            if (left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                lp.SetMargins(left, top, right, bottom);
            }
            control.LayoutParameters = lp;
            _layoutCurrent.AddView(control);

            if (controlText != null)
            {
                controlText(control, lp);
            }

            return this;
        }

        public BuildRelativeLayout SetImage(int imageId, int left = 0, int top = 0, int right = 0, int bottom = 0, Action<ImageView, RelativeLayout.LayoutParams> controlImage = null)
        {
            var context = _layoutMain.Context;
            var img = context.Resources.GetDrawable(imageId);
            var view = new ImageView(context);
            var lp = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            view.SetImageDrawable(img);
            if (left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                lp.SetMargins(left, top, right, bottom);
            }
            view.LayoutParameters = lp;
            _layoutCurrent.AddView(view);
            if (controlImage != null)
            {
                controlImage(view, lp);
            }
            return this;
        }

        public BuildRelativeLayout SetEdit(string text, int left = 0, int top = 0, int right = 0, int bottom = 0, Action<EditText, RelativeLayout.LayoutParams> controlText = null)
        {
            var control = new EditText(_layoutMain.Context);
            control.Text = text;

            var lp = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                       ViewGroup.LayoutParams.WrapContent);
            if (left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                lp.SetMargins(left, top, right, bottom);
            }
            control.LayoutParameters = lp;
            _layoutCurrent.AddView(control);

            if (controlText != null)
            {
                controlText(control, lp);
            }

            return this;
        }

        public RelativeLayout Build(Action<RelativeLayout> layoutControl = null)
        {
            if (layoutControl != null)
            {
                layoutControl(_layoutMain);
            }
            return _layoutMain;
        }
    }
}