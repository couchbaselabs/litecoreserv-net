using System;
using System.IO;
using System.Linq;
using LiteCore.Interop;
using LiteCore.Util;

namespace LiteCoreServ
{
	public static unsafe class LiteCoreServFactory
	{
		private static C4RESTConfig RestConfig = new C4RESTConfig();
		private static C4RESTListener* Listener;
		private static C4DatabaseConfig DatabaseConfig = new C4DatabaseConfig
		{
			flags = C4DatabaseFlags.Bundled | C4DatabaseFlags.SharedKeys
		};

		private static string Directory;

		public static void Start(Options opts)
		{
			if(opts.Directory == null && !opts.Paths.Any()) {
				Fail("One of --paths or --dir must be specified!");
			}

			WebSocketTransport.RegisterWithC4();
			RestConfig.port = (ushort)opts.Port;
			RestConfig.allowCreateDBs = RestConfig.allowDeleteDBs = true;

			var restLog = Native.c4log_getDomain("REST", true);
			Native.c4log_setLevel(restLog, C4LogLevel.Info);

			if(opts.ReadOnly) {
				DatabaseConfig.flags |= C4DatabaseFlags.ReadOnly;
				RestConfig.allowCreateDBs = RestConfig.allowDeleteDBs = false;
			}

			if(opts.Create) {
				DatabaseConfig.flags |= C4DatabaseFlags.Create;
			}

			ShareDatabaseDir(opts.Directory);
			foreach(var path in opts.Paths) {
				var name = Native.c4rest_databaseNameFromPath(path);
				if(name == null) {
					Fail("Invalid Database Name");
				}

				Console.WriteLine($"Sharing database {name} from {path}");
				ShareDatabase(path, name);
			}

			Console.WriteLine($"LiteCoreServ is now listening at http://localhost:{RestConfig.port}/");

		}

		private static void Fail(string message)
		{
			Console.WriteLine($"Error: {message}");
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(true);
			Environment.Exit(1);
		}

		private static void ShareDatabaseDir(string directoryPath)
		{
			if(directoryPath == null) {
				return;
			}

			Directory = directoryPath;
			StartListener();
			Console.WriteLine($"Sharing all databases in {directoryPath}: ");
			var directory = new DirectoryInfo(directoryPath);
			foreach (var file in directory.EnumerateDirectories()) {
				if (file.Extension == ".cblite2") {
					var name = Native.c4rest_databaseNameFromPath(file.FullName);
					if(name != null) {
						Console.WriteLine($"\t{name}");
						ShareDatabase(file.FullName, name);
					}
				}
			}
		}

		private static void ShareDatabase(string path, string name)
		{
			StartListener();
			C4Error err;
			fixed (C4DatabaseConfig* config = &DatabaseConfig)
			{
				var db = Native.c4db_open(path, config, &err);
				if(db == null) {
					Fail($"Opening database {err}");
				}

				Native.c4rest_shareDB(Listener, name, db);
				Native.c4db_free(db);
			}
		}

		private static void StartListener()
		{
			if(Listener == null) {
				C4Error err;
				using(var directory_ = new C4String(Directory)) {
					fixed (C4RESTConfig* config = &RestConfig) {
						config->directory = directory_.AsC4Slice();
						Listener = Native.c4rest_start(config, &err);
					}
				}

				if(Listener == null) {
					Fail($"Starting REST listener {err}");
				}
			}
		}
	}
}
