using Android.Support.V4.View;
using INetSales.AndroidUi.Controls;

namespace INetSales.AndroidUi.Activities.Main
{
    partial class RoteiroActivity
    {
        private void InitializeFragment()
        {
            var adapter = new MenuRoteiroFragmentAdapter(SupportFragmentManager, _controller);

            var pager = FindViewById<ViewPager>(Resource.Id.pager);
            pager.Adapter = adapter;

            var indicator = FindViewById<CirclePageIndicator>(Resource.Id.indicator);
            indicator.SetViewPager(pager);
        }
    }
}