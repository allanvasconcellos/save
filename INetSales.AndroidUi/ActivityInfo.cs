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
using Object = Java.Lang.Object;

namespace INetSales.AndroidUi
{
    public class ActivityInfo : Object
    {
        public string LaunchData { get; set; }
        public string Path { get; set; }
        public string Prefix { get; set; }

        public ActivityInfo(string prefix, string path, string launchData)
        {
            Prefix = prefix;
            Path = path;
            LaunchData = launchData;
        }

        public string Component
        {
            get { return LaunchData.Split(':')[1]; }
        }

        public string Name
        {
            get
            {
                return NoPrefixPath.Split('/')[0];
            }
        }

        public string NoPrefixPath
        {
            get { return Path.Substring(Prefix.Length).TrimStart('/'); }
        }

        public string Package
        {
            get { return LaunchData.Split(':')[0]; }
        }

        public override string ToString()
        {
            return Name;
        }

        public static ActivityInfo Empty
        {
            get { return null; }
        }

        public class NameComparer : IEqualityComparer<ActivityInfo>
        {
            #region IEqualityComparer<ActivityListItem> Members
            public bool Equals(ActivityInfo x, ActivityInfo y)
            {
                return String.Compare(x.Name, y.Name) == 0;
            }

            public int GetHashCode(ActivityInfo obj)
            {
                return obj.Name.GetHashCode();
            }
            #endregion
        }
    }
}