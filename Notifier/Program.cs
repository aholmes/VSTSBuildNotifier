using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSTS.Contract;

namespace Notifier
{
	class Program
	{
		static void Main(string[] args)
		{
            _config           = System.Configuration.ConfigurationManager.AppSettings;
            _VSTSUri          = _config["VSTSUri"];
            _VSTSCollection   = _config["VSTSCollection"];
            _VSTSProject      = _config["VSTSProject"];
            _twilioAccountSid =  _config["TwilioAccountSid"];
            _twilioAuthToken  =  _config["TwilioAuthToken"];
            _twilioFromNumber =  _config["TwilioFromNumber"];
            _twilioToNumber   = _config["TwilioToNumber"];

			Console.WriteLine("Starting build monitor.");

			Task.WaitAll(new[] { Exec() });
		}

        private static NameValueCollection _config;
		private static string _VSTSUri;
		private static string _VSTSCollection;
		private static string _VSTSProject;
        private static string _twilioAccountSid;
        private static string _twilioAuthToken;
        private static string _twilioFromNumber;
        private static string _twilioToNumber;


		static Task Exec()
		{
			//var last = build.GetPendingBuildIds("aholmes", "DevNextRelease").Result;
			Console.WriteLine("Running ...");
            return Task.Run(async () =>
            {
                var buildService = new VSTS.Build(_VSTSUri, _VSTSCollection, _VSTSProject);
                buildService.BuildChanged += (sender, args) =>
                {
                    Console.WriteLine("Done - old: " + args.OldBuild.Id + ": " + args.OldBuild.Status + " " + args.OldBuild.Result);
                    Console.WriteLine("Done - new: " + args.NewBuild.Id + ": " + args.NewBuild.Status + " " + args.NewBuild.Result);

                    var message = args.NewBuild.Status == BuildStatus.InProgress
                        ? string.Format("Build ID {0} started at {1}", args.NewBuild.Id, DateTime.UtcNow)
                        : string.Format("Build ID {0} finished at {1} with status \"{2}\" and result \"{3}\"", args.NewBuild.Id, DateTime.UtcNow, args.NewBuild.Status, args.NewBuild.Result);

                    SendTwilioMessage(message);
                };

                while (true)
                {
                    Console.WriteLine("\n" + DateTime.Now);
                    var builds = await buildService.GetBuilds("DevRelease");

                    var pendingBuilds = builds.Where(build => (build.Status.Value & (BuildStatus.InProgress | BuildStatus.Cancelling | BuildStatus.Postponed | BuildStatus.NotStarted)) != BuildStatus.None);
                    foreach(var build in pendingBuilds)
                    {
                        Console.WriteLine("Monitoring: " + build.Id + ": " + build.Status + " " + build.Result);
                    }

					await Task.Delay(3000);
				}
			});
		}

        static Task SendTwilioMessage(string message)
        {
			var twilio = new Twilio.Sender(_twilioAccountSid, _twilioAuthToken);

			return Task.FromResult(twilio.Send(_twilioFromNumber, _twilioToNumber, message));
        }
	}
}