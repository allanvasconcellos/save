using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using INetSales.AndroidUi.Library;
using Java.Lang;
using Android.Util;

namespace INetSales.AndroidUi.Controls
{
    public class PageIndicatorSelectedEventArgs : EventArgs
    {
        public int SelectedPosition { get; private set; }

        public PageIndicatorSelectedEventArgs(int selectedPosition)
        {
            SelectedPosition = selectedPosition;
        }
    }

    public sealed class TabPageIndicator : HorizontalScrollView, IPageIndicator
	{
		private readonly LinearLayout _mTabLayout;
		private ViewPager _mViewPager;
		private ViewPager.IOnPageChangeListener _mListener;
		private readonly LayoutInflater _mInflater;
		int _mMaxTabWidth;
		private int _mSelectedTabIndex;

        public event EventHandler<PageIndicatorSelectedEventArgs> PageSelected;

        public TabPageIndicator (Context context) : base(context, null)
		{
		}
	
		public TabPageIndicator (Context context, IAttributeSet attrs) : base(context, attrs)
		{
			HorizontalScrollBarEnabled = false;
			
			_mInflater = LayoutInflater.From (context);
	
			_mTabLayout = new LinearLayout (Context);
			AddView (_mTabLayout, new ViewGroup.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.FillParent));
		}
		
		protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
		{
			var widthMode = MeasureSpec.GetMode (widthMeasureSpec);
			var lockedExpanded = widthMode == MeasureSpecMode.Exactly;
			FillViewport = lockedExpanded;
	
			int childCount = _mTabLayout.ChildCount;
			if (childCount > 1 && (widthMode == MeasureSpecMode.Exactly || widthMode == MeasureSpecMode.AtMost)) {
				if (childCount > 2) {
					_mMaxTabWidth = (int)(MeasureSpec.GetSize (widthMeasureSpec) * 0.4f);
				} else {
					_mMaxTabWidth = MeasureSpec.GetSize (widthMeasureSpec) / 2;
				}
			} else {
				_mMaxTabWidth = -1;
			}
	
			int oldWidth = MeasuredWidth;
			base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
			int newWidth = MeasuredWidth;
	
			if (lockedExpanded && oldWidth != newWidth) {
				// Recenter the tab display if we're at a new (scrollable) size.
				SetCurrentItem (_mSelectedTabIndex);
			}
		}
		
		private void AnimateToTab (int position)
		{
			var tabView = _mTabLayout.GetChildAt (position);
	        
			// Do we not have any call backs because we're handling this with Post?
			/*if (mTabSelector != null) {
	            RemoveCallbacks(mTabSelector);
	        }*/
			
			Post (() => {
				var scrollPos = tabView.Left - (Width - tabView.Width) / 2;
				SmoothScrollTo (scrollPos, 0);
			});
		}
		
		public void SetCurrentItem (int item)
		{
			if (_mViewPager == null) {
				throw new IllegalStateException ("ViewPager has not been bound.");
			}
			_mSelectedTabIndex = item;
			var tabCount = _mTabLayout.ChildCount;
			for (int i = 0; i < tabCount; i++) {
				var child = _mTabLayout.GetChildAt (i);
				var isSelected = (i == item);
				child.Selected = isSelected;
				if (isSelected) {
					AnimateToTab (item);
				}
			}
		}
		
		protected override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			
			Console.WriteLine ("OnAttachedToWindow");
			/*
			 * 
			 *  super.onAttachedToWindow();
        if (mTabSelector != null) {
            // Re-post the selector we saved
            post(mTabSelector);
        }
*/
		}
		
		protected override void OnDetachedFromWindow ()
		{
			base.OnDetachedFromWindow ();
			
			Console.WriteLine ("OnDetachedFromWindow...");
//			super.onDetachedFromWindow();
//        if (mTabSelector != null) {
//            removeCallbacks(mTabSelector);
//        }
		}

        public Action<int> BindingTabClick { get; set; }
		
		private void AddTab (string text, int index)
		{
			//Workaround for not being able to pass a defStyle on pre-3.0
            var tabView = (TabView)_mInflater.Inflate(Resource.Layout.vpi__tab, null);
            tabView.Init(this, text, index);
            tabView.Focusable = true;
            tabView.Click += delegate(object sender, EventArgs e)
            {
                var tView = (TabView)sender;
                if (BindingTabClick != null)
                {
                    BindingTabClick(tView.GetIndex());
                }
                _mViewPager.CurrentItem = tView.GetIndex();
            };

            _mTabLayout.AddView(tabView, new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.FillParent, 1));
		}
		
		public void OnPageScrollStateChanged (int p0)
		{
			if (_mListener != null) {
				_mListener.OnPageScrollStateChanged (p0);
			}
		}
		
		public void OnPageScrolled (int p0, float p1, int p2)
		{
			if (_mListener != null) 
            {
				_mListener.OnPageScrolled (p0, p1, p2);
			}
		}
		
		public void OnPageSelected (int p0)
		{
			SetCurrentItem (p0);
            EventHandler<PageIndicatorSelectedEventArgs> handler = PageSelected;
            if (handler != null) handler(this, new PageIndicatorSelectedEventArgs(p0));
			if (_mListener != null) {
				_mListener.OnPageSelected (p0);
			}
		}
		
		public void SetViewPager (ViewPager view)
		{
			var adapter = view.Adapter;
			if (adapter == null) {
				throw new IllegalStateException ("ViewPager does not have adapter instance.");
			}
			if (!(adapter is ITitleProvider)) {
				throw new IllegalStateException ("ViewPager adapter must implement TitleProvider to be used with TitlePageIndicator.");
			}
			_mViewPager = view;
			view.SetOnPageChangeListener (this);
			NotifyDataSetChanged ();
		}


		public void NotifyDataSetChanged ()
		{
			_mTabLayout.RemoveAllViews ();
			var adapter = (ITitleProvider)_mViewPager.Adapter;
			int count = 0;
		    string title;
		    while (adapter.TryGetTitle(count++, out title))
		    {
                AddTab(title, count - 1);
		    }
			if (_mSelectedTabIndex > count) {
				_mSelectedTabIndex = count - 1;
			}
			SetCurrentItem (_mSelectedTabIndex);
			RequestLayout ();
		}
		
		public void SetViewPager (ViewPager view, int initialPosition)
		{
			SetViewPager (view);
			SetCurrentItem (initialPosition);
		}
		
		public void SetOnPageChangeListener (ViewPager.IOnPageChangeListener listener)
		{
			_mListener = listener;
		}
		
		public class TabView : LinearLayout
		{
			private TabPageIndicator mParent;
			private int mIndex;
	
			public TabView (Context context, IAttributeSet attrs) : base(context, attrs)
			{
			}
	
			public void Init (TabPageIndicator parent, string text, int index)
			{
				mParent = parent;
				mIndex = index;
	
				TextView textView = FindViewById<TextView> (Android.Resource.Id.Text1);
				textView.Text = text;
			}
			
			protected override void OnMeasure (int widthMeasureSpec, int heightMeasureSpec)
			{
				base.OnMeasure (widthMeasureSpec, heightMeasureSpec);
				
				// Re-measure if we went beyond our maximum size.
				if (mParent._mMaxTabWidth > 0 && MeasuredWidth > mParent._mMaxTabWidth) {
					base.OnMeasure (MeasureSpec.MakeMeasureSpec (mParent._mMaxTabWidth, MeasureSpecMode.Exactly), heightMeasureSpec);
				}
				
			}
	
			public int GetIndex ()
			{
				return mIndex;
			}
		}
	}
}

