namespace INetSales.ViewController
{
    public interface IProgressView
    {
        void Close();

        void UpdateStatus(double progress);
    }
}