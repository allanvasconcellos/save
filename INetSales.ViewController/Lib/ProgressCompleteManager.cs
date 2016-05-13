using INetSales.Objects;

namespace INetSales.ViewController.Lib
{
    /// <summary>
    /// Controla o progresso de 0 a 100.
    /// </summary>
    public class ProgressCompleteManager
    {
        private readonly IProgressView _progressView;
        private readonly object _lockProgress;
        private double _progressEnviado;
        private int _limitAnterior;
        private int _limitAtual;

        public ProgressCompleteManager(IProgressView view)
        {
            _progressView = view;
            _lockProgress = new object();
            UpdateLimitStatus(100);
        }

        public ProgressCompleteManager(IProgressView view, int limit)
            : this(view)
        {
            UpdateLimitStatus(limit);
        }

        public void UpdateLimitStatus(int limit)
        {
            lock (_lockProgress)
            {
                _limitAnterior = _limitAtual;
                _limitAtual = limit;
            }
        }

        public void UpdateProgressPercent(double percent)
        {
            lock (_lockProgress)
            {
                if (_progressView != null)
                {
                    //Logger.Debug("Percent: {0}", percent);
                    double progress = GetProgress(percent);
                    //Logger.Debug("Progresso: {0}", progress);
                    _progressView.UpdateStatus(progress);
                }
            }
        }

        private double GetProgress(double percent)
        {
            lock (_lockProgress)
            {
                int intervaloLimit = _limitAtual - _limitAnterior;
                //Logger.Debug("_limitAtual: {0} - _limitAnterior: {1} - intervaloLimit: {2}", _limitAtual, _limitAnterior, intervaloLimit);
                //Logger.Debug("_progressEnviado: {0}", _progressEnviado);
                if (_progressEnviado < _limitAtual)
                {
                    double percentUnit = percent/100;
                    _progressEnviado = intervaloLimit*percentUnit + _progressEnviado;
                }
                return _progressEnviado;
            }
        }
    }
}