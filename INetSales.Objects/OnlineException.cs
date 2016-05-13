using System;

namespace INetSales.Objects
{
    public enum OnlineReturnType
    {
        Indefinido = 0,
        SemConexao,
        Timeout,
        ErpMessage,
    }

    public class OnlineException : ApplicationException
    {
        public bool IsMessageErp { get { return ReturnType == OnlineReturnType.ErpMessage; } }

        public OnlineReturnType ReturnType { get; private set; }

        public OnlineException(string message, OnlineReturnType returnType) : this(message, returnType, null)
        {
        }

        public OnlineException(string message, OnlineReturnType returnType, Exception inner)
            : base(message, inner)
        {
            ReturnType = returnType;
        }

        public override string ToString()
        {
            return String.Format("Message: {0}\nReturn Type: {1}\nInner: {2}", Message, ReturnType, InnerException);
        }
    }
}