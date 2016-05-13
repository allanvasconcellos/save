using System;
using Android.Support.V4.App;
using INetSales.AndroidUi.Library;

namespace INetSales.AndroidUi.Controls
{
    public class FragmentAdapter : FragmentPagerAdapter, ITitleProvider
    {
        private readonly string[] _contents;
        private int _mCount;
        //private FragmentManager _fm;

        public FragmentAdapter(FragmentManager fm, string[] tabTitles)
            : base(fm)
        {
            //_fm = fm;
            _contents = tabTitles;
        }

        public Func<int, Fragment> BindingGetItem { get; set; }

        public override Fragment GetItem(int position)
        {

            //var fragment = new TestFragment(CONTENT[position % CONTENT.Count()]);
            //return new TestFragment(CONTENT[position % CONTENT.Count()]);
            return BindingGetItem(position);
        }

        public override int Count
        {
            get { return _mCount; }
        }

        public void SetCount(int count)
        {
            Console.WriteLine("Setting count to " + count);
            if (count > 0 && count <= 10)
            {
                _mCount = count;
                NotifyDataSetChanged();
            }
        }

        public string GetTitle(int position)
        {
            return _contents[position % _contents.Length].ToUpper();
        }

        public bool TryGetTitle(int position, out string title)
        {
            title = String.Empty;
            if(position <= _contents.Length - 1)
            {
                title = _contents[position].ToUpper();
                return true;
            }
            return false;
        }

        //public void UpdateFragment(Fragment fragment, Fragment newFragment)
        //{
        //    var tr = _fm.BeginTransaction();
        //    tr.Replace(fragment.Id, newFragment);
        //    tr.Commit();
            //_fm.BeginTransaction()
            //    .Remove(fragment)
            //    .Add(fragment, "ttt")
            //    .Commit();
        //}
    }
}