using System;
using Android.Content;
using Android.OS;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Sync;
using INetSales.ViewController.Sync.Integrators;

namespace INetSales.AndroidUi
{
    [BroadcastReceiver]
    public class ExecuteSyncReceiver : BroadcastReceiver
    {
        private readonly Handler _handler;

        public ExecuteSyncReceiver()
        {
        }

        public ExecuteSyncReceiver(Handler handler)
            : this()
        {
            _handler = handler;
        }

        #region Overrides of BroadcastReceiver

        public override void OnReceive(Context context, Intent intent)
        {
            DateTime inicio = DateTime.Now;
            //using (var session = DbHelper.GetOfflineDbSession())
            //{
                //var configuracao = DbHelper.GetOfflineConfiguracaoAtiva(session);
                //var manager = new IntegratorManager();
                //manager.Enqueue(new RotaSync(configuracao, session));
                //manager.Execute(new ProgressCompleteManager(null));
            //}
            SendFinishMessage(inicio, DateTime.Now);
        }

        #endregion

        private void SendFinishMessage(DateTime inicio, DateTime fim)
        {
            Message msg = _handler.ObtainMessage();
            var bundle = new Bundle();
            bundle.PutStringArray(SyncService.EXECUTE_FINISH_KEY, new [] { inicio.ToString(), fim.ToString() });
            msg.Data = bundle;
            _handler.SendMessage(msg);
        }
    }
}