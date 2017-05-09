using System.Collections.Generic;
using CommandLine;

namespace LiteCoreServ
{
	public sealed class Options
	{
		[Option('p', "port", Default = 59840, 
		        HelpText="Sets the port that LiteCoreServ should listen on")]
		public int Port { get; set; }

		[Option('c', "create", Default = false,
		       HelpText = "Creates the database if it does not exist, otherwise fails")]
		public bool Create { get; set; }

		[Option('r', "readonly", Default = false,
		       HelpText = "Opens the database as read-only, disallowing any changes")]
		public bool ReadOnly { get; set; }

		[Option('d', "dir", HelpText = "The directory to serve databases from (cannot be combined with --paths)",
		       SetName = "dir")]
		public string Directory { get; set; }

		[Option('p', "paths", HelpText = "The paths of databases to serve (cannot be combined with --dir)",
		        SetName = "paths")]
		public IEnumerable<string> Paths { get; set; }
	}
}