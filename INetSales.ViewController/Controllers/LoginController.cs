using System;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Sync;
using INetSales.ViewController.Views;
using INetSales.ViewController.Sync.Integrators;

namespace INetSales.ViewController.Controllers
{
    public class LoginController : BaseController<IView>
    {
        private readonly IAcessoChildView _acessoView;
        private readonly TimeSpan tempoExpiraLogin;
        private readonly TimeSpan tempoExpiraAdm;

        private const string paramUserLoginValue = "UserLogin";
        private const string paramPwdLoginValue = "PwdLogin";
        private const string paramHoraLoginValue = "HoraLogin";

        public LoginController(IView view, IAcessoChildView acessoView, IApplication application)
            : base(view, application, null)
        {
            _acessoView = acessoView;
#if DEBUG
            tempoExpiraLogin = TimeSpan.FromDays(30);
#else
            tempoExpiraLogin = TimeSpan.FromMinutes(30);
#endif
        }

        private bool Login(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                View.ShowMessage("Usuário / Senha não informado");
                return false;
            }

            UsuarioDto usuarioLogado;
            if (!TryVerificarUsuarioOffline(username.Trim(), password, out usuarioLogado))
            {
                View.ShowMessage("Usuário / Senha inválido");
                return false;
            }

            DateTime horaLogin = DateTime.Now;
            Application.SetValue(paramUserLoginValue, usuarioLogado.Username);
            Application.SetValue(paramPwdLoginValue, password);
            Application.SetValue(paramHoraLoginValue, horaLogin.ToString());
            Session.RegisterLogin(usuarioLogado, horaLogin);

			#if !DEBUG
            if (usuarioLogado.IsSyncPending) // Executa uma sincronização completa
            {
                var offline = DbHelper.GetOffline<IOfflineUsuarioDb>();
                var manager = new IntegratorManager();
                var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
                var progress = View.ShowProgressView("Carregando...");

                //manager.Enqueue(new UsuarioSync(configuracao));
                //manager.Enqueue(new CondicaoPagamentoSync(configuracao));
                manager.Enqueue(new PedidoUpload());
                manager.Enqueue(new GrupoSync(configuracao));
				manager.Enqueue(new ProdutoSync(configuracao));
                manager.Enqueue(new ProdutoSync(configuracao, true));
                //manager.Enqueue(new RamoSync(configuracao));
                manager.Enqueue(new ClienteSync(configuracao));
                manager.Enqueue(new RotaSync(configuracao));
                //manager.Enqueue(new PesquisaSync());
                ExecuteOnBackgroundView(() =>
                {
                    manager.Execute(new ProgressCompleteManager(progress));
                    progress.Close();
					usuarioLogado.IsSyncPending = false;
					offline.Save(usuarioLogado);
                    View.Next();
                });
                return true;
            }
			#endif
            View.Next();
            return true;
        }

        public bool LoginAdm(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                View.ShowMessage("Usuário / Senha não informado");
                return false;
            }

            if(Session.HasLoginAdm)
            {
                if((DateTime.Now - Session.HoraLoginAdm) <= tempoExpiraAdm)
                {
                    Session.UpdateHoraLoginAdm(DateTime.Now);
                    return true;
                }
            }

            UsuarioDto usuarioAdm;
            if (!TryVerificarUsuarioOffline(username.Trim(), password, out usuarioAdm))
            {
                View.ShowMessage("Usuário / Senha inválido");
                return false;
            }

            if(!usuarioAdm.IsAdm)
            {
                View.ShowMessage("Usuário não tem privilégios");
                return false;
            }

            Session.RegisterLoginAdm(usuarioAdm, DateTime.Now);
            return true;
        }

        private bool TryVerificarUsuarioOffline(string username, string senha, out UsuarioDto usuario)
        {
            usuario = null;
            var offline = DbHelper.GetOffline<IOfflineUsuarioDb>();
            try
            {
                var usuarioAcesso = offline.FindByCodigo(username.ToUpper());
                // Coloca o hash na senha informada.
                if (usuarioAcesso != null && usuarioAcesso.SenhaHash.Equals(Utils.HashString(senha)))
                {
                    usuario = usuarioAcesso;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                if (ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
                return false;
            }
        }

        internal void IniciarAcesso()
        {
            // Verificar se existe hora no cache.
            var ultimaHoraLogin = Application.GetValue(paramHoraLoginValue);
#if !DEBUG
            if (!String.IsNullOrEmpty(ultimaHoraLogin) && (DateTime.Now - Convert.ToDateTime(ultimaHoraLogin)) <= tempoExpiraLogin)
            {
#endif
            var ultimoUserLogin = Application.GetValue(paramUserLoginValue);
            var ultimoPwdLogin = Application.GetValue(paramPwdLoginValue);
#if DEBUG
			var configuracao = DbHelper.GetOfflineConfiguracaoAtiva();
			ultimoUserLogin = configuracao.UsernameDebug;
			ultimoPwdLogin = configuracao.PasswordDebug;
#endif
			if(!String.IsNullOrEmpty(ultimoUserLogin))
			{
	            if(Login(ultimoUserLogin, ultimoPwdLogin))
	            {
	                return;
	            }
			}
#if !DEBUG
            }
#endif

            _acessoView.ShowLogin(Login);
        }

        internal void IniciarAcessoAdm(Action iniciou)
        {
            // Já tem um acesso ADM.

            // Ainda não tem, ou expirou
            _acessoView.ShowLogin((username, password) =>
                                      {
                                          if(LoginAdm(username, password))
                                          {
                                              iniciou();
                                              return true;
                                          }
                                          return false;
                                      });
        }

        internal void Logoff()
        {
            Application.SetValue(paramUserLoginValue, null);
            Application.SetValue(paramPwdLoginValue, null);
            Application.SetValue(paramHoraLoginValue, null);
            View.CloseView();
        }
    }
}