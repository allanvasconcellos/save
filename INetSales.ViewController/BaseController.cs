using System;

namespace INetSales.ViewController
{
    public abstract class BaseController<TView> where TView : IView
    {
        protected IApplication Application { get; set; }
        protected TView View { get; set; }
        protected IService Service { get; set; }

        protected string CurrentTitle
        {
            get
            {
                string title = String.Empty;
                if (Session.UsuarioLogado != null)
                {
                    title += String.Format("Vendedor: {0} - Hora Login: {1:dd/MM/yy hh:mm}", Session.UsuarioLogado.Nome, Session.HoraLogin);
                }
                return title;
            }
        }

        protected BaseController(TView listView, IApplication application, IService service)
        {
            Application = application;
            View = listView;
            Service = service;
        }

        public void Close(Action actionToClose)
        {
            DoClose(actionToClose);
        }

        protected virtual void DoClose(Action actionToClose) { }

        protected void ExecuteOnBackgroundView(Action execute, Action<Exception> error = null)
        {
	#if !DEBUG_NO_THREAD
            View.ExecuteOnBackground(() =>
            {
	#endif
                try
                {
                    execute();
                }
                catch (Exception ex)
                {
                    if (ExceptionPolicy.Handle(ex))
                    {
                        throw;
                    }
                    if(error != null) error(ex);
                }
	#if !DEBUG_NO_THREAD
            });
	#endif
        }
    }
}