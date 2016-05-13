namespace INetSales.AndroidUi.Library
{
	public interface ITitleProvider
	{
		string GetTitle (int position);

        bool TryGetTitle(int position, out string title);
	}
}

