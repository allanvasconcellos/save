using System.IO;

namespace INetSales.ViewController
{
    public interface IApplication
    {
        void SaveStreamOnApplicationDisk(string fileName, Stream stream, out string fullpath);

        Stream Download(string url);

        string GetDevideId();

        string GetValue(string key);

        bool SetValue(string key, string value);

		void SetPermissionForAll (string path);
    }
}