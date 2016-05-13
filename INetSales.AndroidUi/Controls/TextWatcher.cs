using System;
using Java.Lang;
using Android.Text;
using Android.Widget;

namespace INetSales.AndroidUi.Controls
{
	public class TextWatcher : Java.Lang.Object, ITextWatcher 
	{
		Filter filter;

		public TextWatcher (Filter filter)
		{
			this.filter = filter;
		}

		#region ITextWatcher implementation

		public void AfterTextChanged (IEditable s)
		{
			filter.InvokeFilter(s.ToString().ToLower());
		}

		public void BeforeTextChanged (ICharSequence s, int start, int count, int after)
		{
		}

		public void OnTextChanged (ICharSequence s, int start, int before, int count)
		{
		}

		#endregion
	}
}

