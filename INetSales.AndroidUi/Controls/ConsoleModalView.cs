using System;
using System.Collections.Generic;
using Android.App;
using Android.Util;
using INetSales.ViewController;

namespace INetSales.AndroidUi.Controls
{
    public class ConsoleModalView : IConsoleView
    {
        private readonly Activity _activity;
        private readonly List<LogItemConsole> _itens;

        private class LogItemConsole
        {
            public string Message { get; set; }
            public LogPriority Priority { get; set; }
            public bool IsSection { get; set; }
        }

        public ConsoleModalView(Activity activity)
        {
            _activity = activity;
            _itens = new List<LogItemConsole>();
        }

        #region Implementation of ILog

        public Guid Id { get; set; }

        public void WriteLog(string message, LogPriority priority)
        {
            _itens.Add(new LogItemConsole() { Message = message, Priority = priority });
        }

        public bool IsConsoleOutput
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Implementation of IConsoleView

        public void WriteTitle(string title)
        {
            throw new NotImplementedException();
        }

        public void WriteSection(string section)
        {
            _itens.Add(new LogItemConsole() { Message = section, IsSection = true });
        }

        public void ShowConsole(Action<string, string, string> sendEmail)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}