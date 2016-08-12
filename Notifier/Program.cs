using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Notifier
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Starting build monitor.");

			Task.WaitAll(new[] { Exec() });
		}

		static Task Exec()
		{
			var config         = System.Configuration.ConfigurationManager.AppSettings;
			var VSTSUri        = config["VSTSUri"];
			var VSTSCollection = config["VSTSCollection"];
			var VSTSProject    = config["VSTSProject"];

			var buildService  = new VSTS.Build(VSTSUri, VSTSCollection, VSTSProject);

			//var last = build.GetPendingBuildIds("aholmes", "DevNextRelease").Result;
			Console.WriteLine("Starting");
			return Task.Run(async () =>
			{

				Console.WriteLine("Running async");
				while(true)
				{
					Console.WriteLine("Loop");
					var pendingBuilds = await buildService.GetPendingBuilds("DevRelease");

					foreach(var build in pendingBuilds)
					{
						Console.WriteLine("Uh");
						var state = await buildService.GetBuild(build.Id);
						Console.WriteLine("Uh");
						Console.WriteLine(state.Id + " " + state.Status + " " + state.Result);
					}

					await Task.Delay(3000);
				}
			});

			//var accountSid = config["TwilioAccountSid"];
			//var authToken  = config["TwilioAuthToken"];
			//var fromNumber = config["TwilioFromNumber"];
			//var toNumber   = config["TwilioToNumber"];

			//var twilio = new Twilio.Sender(accountSid, authToken);
			//twilio.Send(fromNumber, toNumber, "Hello!");
		}
	}
}