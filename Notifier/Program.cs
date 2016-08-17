using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using VSTS.Contract;

namespace Notifier
{
	class Program
	{
		static void Main(string[] args)
		{
			_config                       = System.Configuration.ConfigurationManager.AppSettings;
			_VSTSUri                      = _config["VSTSUri"];
			_VSTSCollection               = _config["VSTSCollection"];
			_VSTSProject                  = _config["VSTSProject"];
			_VSTSBranchName               = _config["VSTSBranchName"];
			_VSTSBuildRequestedByUserName = _config["VSTSBuildRequestedByUserName"];
			_twilioAccountSid             = _config["TwilioAccountSid"];
			_twilioAuthToken              = _config["TwilioAuthToken"];
			_twilioFromNumber             = _config["TwilioFromNumber"];
			_twilioToNumber               = _config["TwilioToNumber"];

			Console.WriteLine("Starting build monitor.");

			Task.WaitAll(new[] { Exec() });
		}

		private static NameValueCollection _config;
		private static string _VSTSUri;
		private static string _VSTSCollection;
		private static string _VSTSProject;
		private static string _VSTSBranchName;
		private static string _VSTSBuildRequestedByUserName;
		private static string _twilioAccountSid;
		private static string _twilioAuthToken;
		private static string _twilioFromNumber;
		private static string _twilioToNumber;

		/// <summary>
		/// Periodically queries for builds and monitors changes to those builds.
		/// </summary>
		/// <returns></returns>
		static async Task Exec()
		{
			var buildService = new VSTS.Build(_VSTSUri, _VSTSCollection, _VSTSProject);

			// When a build is changed, show the old and new status and result, then send a text message.
			buildService.BuildChanged += (sender, args) =>
			{
				Console.WriteLine("Done - old: " + args.OldBuild.Id + ": " + args.OldBuild.Status + " " + args.OldBuild.Result);
				Console.WriteLine("Done - new: " + args.NewBuild.Id + ": " + args.NewBuild.Status + " " + args.NewBuild.Result);

				var message = args.NewBuild.Status == BuildStatus.InProgress
					? string.Format("Build ID {0} started at {1}", args.NewBuild.Id, DateTime.UtcNow)
					: string.Format("Build ID {0} finished at {1} with status \"{2}\" and result \"{3}\"", args.NewBuild.Id, DateTime.UtcNow, args.NewBuild.Status, args.NewBuild.Result);

				SendTwilioMessage(message);
			};

			// Based on the configuration from App.config, set the function to call to query builds.
			System.Func<Task<IEnumerable<Build>>> getBuilds;
			if (!string.IsNullOrEmpty(_VSTSBuildRequestedByUserName))
			{
				getBuilds = () => buildService.GetBuilds(requestedFor: _VSTSBuildRequestedByUserName);
			}
			else if (!string.IsNullOrEmpty(_VSTSBranchName))
			{
				getBuilds = () => buildService.GetBuilds(branchName: _VSTSBranchName);
			}
			else
			{
				getBuilds = () => buildService.GetBuilds();
			}

			Console.WriteLine("Running ...");
			// Loop forever unless an exception occurs.
			while (true)
			{
				Console.WriteLine("\n" + DateTime.Now);

				// Although there is a method to retrieve just the pending builds,
				// we need to get all the builds in order to compare their status when they finish.
				var builds = await getBuilds();

				// Show which builds are being monitored.
				var pendingBuilds = builds.Where(build => (build.Status.Value & (BuildStatus.InProgress | BuildStatus.Cancelling | BuildStatus.Postponed | BuildStatus.NotStarted)) != BuildStatus.None);
				foreach (var build in pendingBuilds)
				{
					Console.WriteLine("Monitoring: " + build.Id + ": " + build.Status + " " + build.Result);
				}

				await Task.Delay(3000);
			}
		}

		/// <summary>
		/// Send a text message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		static Task SendTwilioMessage(string message)
		{
			var twilio = new Twilio.Sender(_twilioAccountSid, _twilioAuthToken);

			return Task.FromResult(twilio.Send(_twilioFromNumber, _twilioToNumber, message));
		}
	}
}