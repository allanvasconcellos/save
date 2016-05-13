using System;
using System.IO;
using System.Net;
using Android.Content;
using Android.Preferences;
using Android.Telephony;
using INetSales.ViewController;
using Environment = System.Environment;
using INetSales.AndroidUi.Helper;

namespace INetSales.AndroidUi
{
	public class MonodroidApplication : IApplication
    {
        private readonly ISharedPreferences _prefs;
        private readonly Context _context;

        public MonodroidApplication(Context context, ISharedPreferences preferences)
        {
            _context = context;
            _prefs = preferences;

			Wifi.Initialize (_context);
        }

        #region Implementation of IApplication

        public void SaveStreamOnApplicationDisk(string fileName, Stream stream, out string fullpath)
        {
            string tempFullpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
            using (var newFile = File.Open(tempFullpath, FileMode.Create, FileAccess.ReadWrite))
            {
                var buffer = new byte[1024];
                int r;
                stream.Position = 0;
                while ((r = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    newFile.Write(buffer, 0, r);
                }
                fullpath = tempFullpath;
            }
        }

		public void SetPermissionForAll (string path)
		{
			LinuxUtils.chmod(path, LinuxUtils.S_IRWXU | LinuxUtils.S_IRWXG | LinuxUtils.S_IRWXO);
		}

        public Stream Download(string url)
        {
            string urlRequest = url.IndexOf(@"http") > -1 ? url : @"http://" + url;
            WebRequest request = WebRequest.Create(urlRequest);
            var memory = new MemoryStream();
            using (WebResponse response = request.GetResponse())
            {
                var buffer = new byte[16 * 1024];
                using(Stream stream = response.GetResponseStream())
                {
                    int r;
                    while ((r = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memory.Write(buffer, 0, r);
                    }
                }
            }
            return memory;
        }

        public string GetDevideId()
        {
            var tMgr = (TelephonyManager)_context.GetSystemService(Context.TelephonyService);
            return tMgr.DeviceId;
        }

        public string GetValue(string key)
        {
            return _prefs.GetString(key, String.Empty);
        }

        public bool SetValue(string key, string value)
        {
            var edit = _prefs.Edit();
            edit.PutString(key, value);
            return edit.Commit();
        }

        #endregion
    }
}