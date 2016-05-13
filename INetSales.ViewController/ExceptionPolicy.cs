using System;
using INetSales.Objects;

namespace INetSales.ViewController
{
    public static class ExceptionPolicy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Indica o rethrow</returns>
        public static bool Handle(Exception ex)
        {
            Logger.Error(ex);
            return false;
        }
    }
}