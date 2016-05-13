using System;
using System.Collections.Generic;
using Android.Util;
using INetSales.Objects;

namespace INetSales.ViewController
{
    public class LogConsoleSaver : ILog
    {
        private class LogItemSaver
        {
            public string Message { get; set; }
            public LogPriority Priority { get; set; }
        }

        private readonly IConsoleView _consoleView;
        private readonly List<LogItemSaver> _itensLog;

        public LogConsoleSaver(IConsoleView consoleView)
        {
            _consoleView = consoleView;
            _itensLog = new List<LogItemSaver>();
        }

        #region Implementation of ILog

        public Guid Id { get; set; }

        public void WriteLog(string message, LogPriority priority)
        {
            _itensLog.Add(new LogItemSaver() { Message = message, Priority = priority, });
            _consoleView.WriteLog(message, priority);
        }

        public bool IsConsoleOutput
        {
            get { return _consoleView.IsConsoleOutput; }
        }

        #endregion

        public void SendEmail(string title, string body, string from)
        {
            
        }
    }
}