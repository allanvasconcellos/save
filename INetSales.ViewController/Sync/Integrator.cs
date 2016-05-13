using System;
using System.Linq;
using System.Collections.Generic;
using INetSales.Objects;
using INetSales.Objects.Dtos;

namespace INetSales.ViewController.Sync
{
    public class ItemErrorIntegrator
    {
        public int Ordem { get; set; }
        public IDto Dto { get; set; }
        public Exception Ex { get; set; }
        public string MessageOnline { get; set; }
    }

    public abstract class Integrator
    {
        protected readonly List<ItemErrorIntegrator> _errors;

        protected Integrator()
        {
            _errors = new List<ItemErrorIntegrator>();
        }
        
        public bool HasError { get; protected set; }

        public IEnumerable<ItemErrorIntegrator> Errors { get { return _errors; } }

        public bool IsTimeout { get; private set; }

        public bool IsSemConexao { get; private set; }

        public string MessageOnline { get; private set; }

        public OnlineException GetOnlineError(int index)
        {
			if (index < 0 || _errors == null || _errors.Count < 1) {
				return null;
			}
			Logger.Info(false, String.Format("GetOnlineError - Index: {0}", index));
            int indexEncontrado = _errors.FindIndex(index, 1, e => e.Ex is OnlineException);
            return _errors[indexEncontrado].Ex as OnlineException;
        }

        public void Execute(UsuarioDto usuario = null)
        {
            try
            {
                DoExecute(usuario);
            }
            catch(Exception ex)
            {
                ExceptionPolicy.Handle(ex);
                _errors.Add(new ItemErrorIntegrator(){ Ordem = 0, Ex = ex,});
            }
            finally
            {
                if(_errors.Count > 0)
                {
                    var ex = GetOnlineError(0);
                    HasError = true;
                    IsTimeout = ex != null && ex.ReturnType == OnlineReturnType.Timeout;
                    IsSemConexao = ex != null && ex.ReturnType == OnlineReturnType.SemConexao;

                    MessageOnline = _errors
                        .Max(i => i.MessageOnline);
                }
            }
        }

        public abstract void DoExecute(UsuarioDto usuario = null);

        protected void RegisterError(Exception ex, int ordem)
        {
            string messageOnline = String.Empty;
            if(ex is OnlineException)
            {
                messageOnline = ((OnlineException) ex).ReturnType == OnlineReturnType.ErpMessage
                                    ? ((OnlineException) ex).Message
                                    : String.Empty;
            }
            _errors.Add(new ItemErrorIntegrator() { Ordem = ordem, Ex = ex, MessageOnline = messageOnline});
        }
    }
}