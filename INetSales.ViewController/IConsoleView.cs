using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using INetSales.Objects;

namespace INetSales.ViewController
{
    public interface IConsoleView : ILog
    {
        void WriteTitle(string title);

        void WriteSection(string section);

        void ShowConsole(Action<string, string, string> sendEmail = null);
    }
}