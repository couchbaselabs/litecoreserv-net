using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommandLine;

namespace LiteCoreServ
{
	public sealed class Program
	{
		public static int Main(string[] args)
		{
			var parser = new Parser(with =>
			{
				with.IgnoreUnknownArguments = false;
			});

			var parsed = parser.ParseArguments<Options>(args);
			var result = parsed.MapResult(opts =>
			{
				LiteCoreServFactory.Start(opts);
				return 0;
			}, err =>
			{
				return 1;
			});

			if(result == 0) {
				Console.WriteLine("Ctrl+C to exit...");
				var mre = new ManualResetEventSlim();
				Console.CancelKeyPress += (sender, a) => mre.Set();
				mre.Wait();
			} else {
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
			}

			return result;
		}
	}
}
