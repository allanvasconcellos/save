using System;
using System.Collections.Generic;
using System.Linq;
using Android.Util;

namespace INetSales.Objects
{
    public interface ILog
    {
        Guid Id { get; set; }

        void WriteLog(string message, LogPriority priority);

        bool IsConsoleOutput { get; }
    }

    public static class Logger
    {
        private const string TAG = "inetsales";
        private static readonly List<ILog> _logs;

        static Logger()
        {
            _logs = new List<ILog>();
        }

        public static void AddLog(ILog log)
        {
            log.Id = Guid.NewGuid();
            _logs.Add(log);
        }

        public static void RemoveLog(ILog log)
        {
            int index = _logs.FindIndex(l => l.Id.Equals(log.Id));
            _logs.RemoveAt(index);
        }

        public static void Error(Exception ex, bool isConsoleOutput = false)
        {
            string message = GetFormatMessage(ex);
            Log.Error(TAG, message);
            if(isConsoleOutput)
            {
                WriteConsoleLog(message, LogPriority.Error);
            }
        }

        public static void Error(bool isConsoleOutput, string message, params object[] parameters)
        {
            Log.Error(TAG, message, parameters);
            if (isConsoleOutput)
            {
                WriteConsoleLog(String.Format(GetFormatMessage(message), parameters), LogPriority.Error);
            }
        }

        public static void Warn(bool isConsoleOutput, string message, params object[] parameters)
        {
            Log.Warn(TAG, message, parameters);
            if (isConsoleOutput)
            {
                WriteConsoleLog(String.Format(GetFormatMessage(message), parameters), LogPriority.Warn);
            }
        }

		public static void CriticalWarn(bool isConsoleOutput, string message, params object[] parameters)
		{
			Log.WriteLine(LogPriority.Warn, TAG + "_C", message, parameters);
			if (isConsoleOutput)
			{
				WriteConsoleLog(String.Format(GetFormatMessage(message), parameters), LogPriority.Warn);
			}
		}

        public static void Info(bool isConsoleOutput, string message, params object[] parameters)
        {
            Log.Info(TAG, message, parameters);
            if (isConsoleOutput)
            {
                WriteConsoleLog(String.Format(GetFormatMessage(message), parameters), LogPriority.Info);
            }
        }

        public static void Debug()
        {
#if DEBUG
            Log.Debug(TAG, GetFormatMessage(String.Empty));
#endif
        }

        public static void Debug(string message)
        {
#if DEBUG
            Log.Debug(TAG, GetFormatMessage(message));
#endif
        }

        public static void Debug(string message, params object[] parameters)
        {
#if DEBUG
            string msg = String.Format(message, parameters);
            Log.Debug(TAG, GetFormatMessage(msg));
#endif
        }

        private static string GetFormatMessage(string internalMessage)
        {
            // Internal Message - Time - Method - Line
            string message = String.Format("*{0} - {1} - {2}", internalMessage, Utils.GetCallMethodFullname(2), Utils.GetCallMethodLine(2));
            return message;
        }

        private static string GetFormatMessage(Exception ex)
        {
            return ex.ToString();
        }

        private static void WriteConsoleLog(string message, LogPriority priority)
        {
            foreach (var log in _logs.Where(l => l.IsConsoleOutput))
            {
                log.WriteLog(String.Format(" - {0}", message), priority);
            }
        }
    }
}