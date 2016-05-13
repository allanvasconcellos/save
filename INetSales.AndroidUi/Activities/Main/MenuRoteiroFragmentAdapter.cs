using System;
using Android.Runtime;
using Android.Support.V4.App;
using INetSales.ViewController.Controllers;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace INetSales.AndroidUi.Activities.Main
{
    public class MenuRoteiroFragmentAdapter : FragmentPagerAdapter
    {
        private readonly RoteiroController _controller;
        private const int _count = 2;

        public MenuRoteiroFragmentAdapter(FragmentManager p0, RoteiroController controller) : base(p0)
        {
            _controller = controller;
        }

        #region Overrides of PagerAdapter

        public override int Count
        {
            get { return _count; }
        }

        #endregion

        #region Overrides of FragmentPagerAdapter

        public override Fragment GetItem(int p0)
        {
            return new MenuRoteiroFragment(p0, _controller);
        }

        #endregion
    }
}