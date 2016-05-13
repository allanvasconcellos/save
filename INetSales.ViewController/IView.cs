using System;
using INetSales.ViewController.Views;

namespace INetSales.ViewController
{
    public interface IView
    {
        void MakeQuestion(string question, Action ok, Action cancel);

        void UpdateViewTitle(params string[] titleLines);

        void ShowMessage(string message);

        void ShowModalMessage(string title, string message, Action ok = null);

        void Next();

        void CloseView();

        IProgressView ShowProgressView(string title = null);

        IConsoleView GetConsoleView(string title = null);

        IConfiguracaoChildView GetConfiguracaoView(string title = null);

        void ExecuteOnBackground(Action execute);

        void ExecuteOnUI(Action execute);
    }
}
