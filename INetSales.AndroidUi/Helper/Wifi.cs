using System;
using Android.Content;
using Android.Net.Wifi;
using System.Collections.Generic;
using Android.Net;

namespace INetSales.AndroidUi.Helper
{
	public class Wifi
	{
		private static Context _context = null;
		private static WifiManager _wifi;
		private static WifiReceiver _wifiReceiver;

		public static List<string> Networks {
			get;
			private set;
		}

		public static List<string> AvailableNetworks {
			get;
			private set;
		}

		public static string CurrentSsid {
			get{
				WifiInfo connectionInfo = _wifi.ConnectionInfo;
				if (connectionInfo != null && !String.IsNullOrEmpty(connectionInfo.SSID)) {
					return connectionInfo.SSID;
				}
				return String.Empty;
			}
		}

		public static bool IsConnected
		{
			get{
				ConnectivityManager connManager = (ConnectivityManager) _context.GetSystemService(Context.ConnectivityService);
				NetworkInfo info = connManager.GetNetworkInfo(ConnectivityType.Wifi);
				return info.IsConnected;
			}
		}

		public static void Initialize(Context ctx)
		{
			_context = ctx;
			Networks = new List<string>();
			AvailableNetworks = new List<string>();
			// Get a handle to the Wifi
			_wifi = (WifiManager)_context.GetSystemService(Context.WifiService);

			//foreach(var config in _wifi.ConfiguredNetworks)
			//{
			//	AvailableNetworks.Add(config.Ssid);
			//}
			// Start a scan and register the Broadcast receiver to get the list of Wifi Networks
			_wifiReceiver = new WifiReceiver();
			_context.RegisterReceiver(_wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
			bool result = _wifi.StartScan();
		}

		public static void Connect(string ssid)
		{
			//WifiConfiguration wifiConfig = new WifiConfiguration();
			//wifiConfig.Ssid = string.Format("{0}", ssid);

			// Use ID
			//int netId = _wifi.UpdateNetwork(wifiConfig);
			//_wifi.Disconnect();
			ssid = ssid.Trim('"', '\\');
			foreach (var config in _wifi.ConfiguredNetworks) {
				if (config.Ssid != null ) {
					string configSsid =	config.Ssid.Trim('"', '\\');
					if (configSsid.Equals (ssid)) {
						_wifi.Disconnect ();
						_wifi.EnableNetwork (config.NetworkId, true);
						_wifi.Reconnect ();
					}
				}
			}

		}

		public static void DisconnectCurrent()
		{
			_wifi.Disconnect ();
		}

		class WifiReceiver : BroadcastReceiver
		{
			public override void OnReceive(Context context, Intent intent)
			{
				Networks.Clear ();
				AvailableNetworks.Clear ();
				if (_wifi.WifiState == WifiState.Enabled) {
					foreach (ScanResult scan in _wifi.ScanResults) {
						
						Networks.Add (scan.Ssid);
						int level = WifiManager.CalculateSignalLevel(scan.Level, 5);
						if (level > 2) {
							AvailableNetworks.Add (scan.Ssid);
						}
					}
				}
			}
		}
	}
}

