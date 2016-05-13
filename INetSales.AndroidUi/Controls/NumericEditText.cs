using System;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Globalization;
using Android.Views;
using Android.Views.InputMethods;

namespace INetSales.AndroidUi.Controls
{
    public class NumericEditText : LinearLayout
    {
        private Button _btnIncrement;
        private Button _btnDecrement;
        private EditText _etNumeric;

        protected NumericEditText(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public NumericEditText(Context context)
            : base(context)
        {
            InitializeContext(context);
        }

        public NumericEditText(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            InitializeContext(context);
        }

        private void InitializeContext(Context context)
        {
            _btnIncrement = new Button(context) { Text = "+" };
            _btnIncrement.SetWidth(50);
            _btnIncrement.Gravity = GravityFlags.CenterVertical;
            _btnIncrement.Click += IncrementClick;
            _btnDecrement = new Button(context) { Text = "-" };
            _btnDecrement.SetWidth(50);
            _btnDecrement.Gravity = GravityFlags.CenterVertical;
            _btnDecrement.Click += DecrementClick;
			_etNumeric = new EditText(context) { InputType = InputTypes.ClassNumber | InputTypes.NumberFlagDecimal | InputTypes.NumberFlagSigned };
            _etNumeric.SetWidth(70);
            _etNumeric.Gravity = GravityFlags.CenterVertical;
            _etNumeric.BeforeTextChanged += NumericBeforeTextChanged;
            _etNumeric.TextChanged += NumericTextChanged;

            this.Orientation = Orientation.Horizontal;

            this.AddView(_btnDecrement, 0);
            this.AddView(_etNumeric, 1);
            this.AddView(_btnIncrement, 2);
        }

        private void DecrementClick(object sender, EventArgs e)
        {
            decimal newValue = Value - 1;
            Value = newValue;
			_etNumeric.RequestFocus ();
			int textLength = _etNumeric.Text.Length;
			_etNumeric.SetSelection(textLength, textLength);
			var inputMethodManager = (InputMethodManager)this.Context.GetSystemService(Context.InputMethodService);
			inputMethodManager.ToggleSoftInputFromWindow(this.WindowToken, ShowSoftInputFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        private void IncrementClick(object sender, EventArgs e)
        {
            decimal newValue = Value + 1;
            Value = newValue;
			_etNumeric.RequestFocus ();
			int textLength = _etNumeric.Text.Length;
			_etNumeric.SetSelection(textLength, textLength);
			var inputMethodManager = (InputMethodManager)this.Context.GetSystemService(Context.InputMethodService);
			inputMethodManager.ToggleSoftInputFromWindow(this.WindowToken, ShowSoftInputFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        private decimal _beforeValue;
        private void NumericBeforeTextChanged(object sender, TextChangedEventArgs e)
        {
            _beforeValue = Value;
        }

        private void NumericTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Value >= 0 && Value <= MaximumValue)
            {
                if (BindingValue != null)
                {
                    BindingValue(Value);
                }
            }
            else
            {
                Value = _beforeValue;
            }
        }

        public Action<decimal> BindingValue { get; set; }

        public decimal MaximumValue { get; set; }

        public decimal Value
        {
            get
            {
                if (!String.IsNullOrEmpty(_etNumeric.Text))
                {
					var numberFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat;
					numberFormat.NumberDecimalSeparator = ".";
                    return Convert.ToDecimal(_etNumeric.Text, numberFormat);
                }
                return 0;
            }
            set
            {
				var numberFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat;
				numberFormat.NumberDecimalSeparator = ".";
				_etNumeric.Text = value.ToString(numberFormat);
            }
        }
    }
}