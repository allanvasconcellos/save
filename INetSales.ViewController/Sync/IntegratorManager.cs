using System;
using System.Collections.Generic;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync
{
    public class IntegratorManager
    {
        private readonly Queue<Integrator> _queue;

        public IntegratorManager()
        {
            _queue = new Queue<Integrator>();
        }

        public bool HasErrors { get; private set; }

        public bool AllErrors { get; private set; }

        public bool HasTimeout { get; private set; }
        
        public bool IsSemConexao { get; private set; }

        public void Enqueue(Integrator integrator)
        {
            _queue.Enqueue(integrator);
        }

        public void Execute(ProgressCompleteManager progress, Action<Integrator> executed = null)
        {
            double percent = 100 / (_queue.Count > 0 ? _queue.Count : 1);
            while (_queue.Count > 0)
            {
                var integrator = _queue.Dequeue();
                integrator.Execute(Session.UsuarioLogado);
                progress.UpdateProgressPercent(percent);
                if(executed != null) executed(integrator);
                if (!HasErrors) HasErrors = integrator.HasError;
                AllErrors = integrator.HasError;
                if(integrator.IsSemConexao)
                {
                    IsSemConexao = true;
                    return;
                }
                if(!HasTimeout) HasTimeout = integrator.IsTimeout;
            }
            progress.UpdateProgressPercent(100);
        }
    }
}