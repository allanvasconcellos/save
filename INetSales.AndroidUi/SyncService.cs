using System;
using Android.App;
using Android.Content;
using Android.OS;
using INetSales.Objects;
using INetSales.ViewController;
using INetSales.ViewController.Sync;

namespace INetSales.AndroidUi
{
    [Service]
    public class SyncService : Service, IService, Handler.ICallback
    {
        public const string MANAGER_KEY = "MANAGER";
        public const string SERVICO_KEY = "SERVICO";
        public const string EXECUTE_BEGIN_KEY = "EXECUTE_BEGIN_KEY";
        public const string EXECUTE_FINISH_KEY = "EXECUTE_FINISH_KEY";
        public const string SEND_MESSAGE_KEY = "SEND_MESSAGE_KEY";
        public const string THREAD_NAME = "SYNC_SERVICE_THREAD";

        private readonly IBinder _binder;
        private TimeSpan _intervalo;
        private Handler _handler;

        public class SyncBinder : Binder
        {
            public SyncBinder(SyncService self)
            {
                Service = self;
            }

            public SyncService Service { get; private set; }
        }

        public SyncService()
        {
            _binder = new SyncBinder(this);
            //_intervalo = TimeSpan.FromMinutes(2);
            _intervalo = TimeSpan.FromSeconds(20);
        }

        public override IBinder OnBind(Intent intent)
        {
            return _binder;
        }

        public override void OnCreate()
        {
            Logger.Debug("Serviço criado");
            //base.OnCreate();

            //var handlerThread = new HandlerThread(THREAD_NAME);
            //handlerThread.Start();
            //// Now get the Looper from the HandlerThread so that we can create a Handler that is attached to
            ////  the HandlerThread
            //// NOTE: This call will block until the HandlerThread gets control and initializes its Looper
            //Looper looper = handlerThread.Looper;
            //// Create a handler for the service
            //// Register the broadcast receiver to run on the separate Thread
            //_handler = new Handler(looper, this);
            //var filter = new IntentFilter("broadcast.executesync");
            //var broadcast = new ExecuteSyncReceiver(_handler);
            //RegisterReceiver(broadcast, filter, null, _handler);
            //Pause();
        }

        public override void OnDestroy()
        {
            Logger.Debug("Serviço destruido");
            Pause();
            base.OnDestroy();
        }

        #region Implementation of IService

        public void Play()
        {
            Logger.Debug("Play serviço");
            var totalIntervalo = (long)_intervalo.TotalMilliseconds;
            DoPlay(totalIntervalo);
        }

        private void DoPlay(long trigger)
        {
            var intent = new Intent("broadcast.executesync");
            var source = PendingIntent.GetBroadcast(this, 0, intent, 0);
            var alarm = (AlarmManager)GetSystemService(AlarmService);

            Logger.Debug("Play - trigger: {0}", trigger);

            alarm.Set(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + trigger, source);
        }

        public void Pause()
        {
            Logger.Debug("Pause serviço");
            var broadcast = new Intent("broadcast.executesync");
            var source = PendingIntent.GetBroadcast(this, 0, broadcast, 0);

            var alarm = (AlarmManager)GetSystemService(AlarmService);
            alarm.Cancel(source);
        }

        #endregion

        #region Implementation of ICallback

        public bool HandleMessage(Message msg)
        {
            if (msg.Data.ContainsKey(EXECUTE_FINISH_KEY))
            {
                DateTime inicio = Convert.ToDateTime(msg.Data.GetStringArray(EXECUTE_FINISH_KEY)[0]);
                DateTime fim = Convert.ToDateTime(msg.Data.GetStringArray(EXECUTE_FINISH_KEY)[1]);
                Logger.Debug("HandleMessage (EXECUTE_FINISH_KEY) - Inicio: {0} | Fim: {1} | (fim - inicio): {2}", inicio, fim, (fim - inicio));
                if((fim - inicio) > _intervalo)
                {
                    DoPlay(0);
                }
                else
                {
                    Logger.Debug("HandleMessage (EXECUTE_FINISH_KEY) - Restante: {0}", (_intervalo - (fim - inicio)).TotalMilliseconds);
                    DoPlay((long)(_intervalo - (fim - inicio)).TotalMilliseconds);
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}