using System;
using Android.Database.Sqlite;
using INetSales.Objects;
using System.IO;
using Android.Content;

namespace INetSales.OfflineInterface.AndroidDb
{
	public class DatabaseHelper : SQLiteOpenHelper
	{
		public DatabaseHelper(Context context, string databaseName, int version)
			: base(context, databaseName, null, version)
		{
		}

		public override void OnCreate(SQLiteDatabase db)
		{
			ExecuteVersion(db, 1, 1);
		}

		public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
		{
			ExecuteVersion(db, oldVersion + 1, newVersion);
		}

		private void ExecuteVersion(SQLiteDatabase db, int startVersion, int endVersion)
		{
			var thisExe = this.GetType().Assembly;

			Logger.Info(false, false, "Script version - start: {0} end: {1}", startVersion, endVersion);

			for (int currentVersion = startVersion; currentVersion <= endVersion; currentVersion++)
			{
				int indiceVersao = 1;
				while (true) // indice
				{
					string script = String.Format("INetSales.OfflineInterface.Scripts.{0:0000}_{1:00}.sql", currentVersion, indiceVersao);
					// DDL
					var file = thisExe.GetManifestResourceStream(script);
					if (file == null)
					{
						if (indiceVersao == 1)
						{
							return;
						}
						break;
					}
					Logger.Info(false, false, "Script a executar: {0}", script);
					ExecuteFile(db, file);
					file.Close();

					indiceVersao++;
				}

				endVersion++;
			}
		}

		private void ExecuteFile(SQLiteDatabase db, Stream file)
		{
			var reader = new StreamReader(file);
			while (!reader.EndOfStream)
			{
				string commandText = GetNextCommand(reader);
				if (!String.IsNullOrEmpty(commandText.Trim()))
				{
					try
					{
						db.ExecSQL(commandText.Trim());
						Logger.Info(false, false, commandText.Trim());
					}
					catch (Exception ex)
					{
						Logger.Error(ex, true, false);
					}
				}
			}
		}

		private string GetNextCommand(StreamReader reader)
		{
			string commandText = String.Empty;
			bool endOfCommand = false;
			while (!reader.EndOfStream && !endOfCommand)
			{
				string linha = reader.ReadLine();
				if (!String.IsNullOrEmpty(linha.Trim()))
				{
					if (linha.StartsWith("--"))
					{
						continue;
					}
					if (linha.EndsWith(";"))
					{
						endOfCommand = true;
					}
					commandText += linha.Trim() + " ";
				}
			}
			return commandText;
		}
	}
}

