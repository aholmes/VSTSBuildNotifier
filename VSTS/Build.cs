using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using WebApi = Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Client;
using AutoMapper;

namespace VSTS
{
	/// <summary>
	/// Utility class for querying builds through VSTS.
	/// </summary>
	public class Build : INotifyBuildChanged
	{
		private Uri _vstsUri;
		private string _projectName;

		private ObservableConcurrentDictionary<int, Contract.Build> _queriedBuilds = new ObservableConcurrentDictionary<int, Contract.Build>();

		/// <summary>
		/// Obtain new instance.
		/// </summary>
		/// <param name="vstsUri"></param>
		/// <param name="collectionName"></param>
		/// <param name="projectName"></param>
		public Build(string vstsUri, string collectionName, string projectName)
		{
			Configuration.AutoMapperConfiguration.Initialize();

			var uriBuilder  = new UriBuilder(vstsUri);
			uriBuilder.Path = string.Concat(uriBuilder.Path.TrimEnd('/'), '/', collectionName);
			_vstsUri        = uriBuilder.Uri;
			_projectName    = projectName;
		}

		/// <summary>
		/// Initialize this class by storing the last 100 builds
		/// </summary>
		/// <param name="requestedFor"></param>
		/// <param name="branchName"></param>
		/// <returns></returns>
		public Task Initialize(string requestedFor = default(string), string branchName = default(string))
		{
			return _getBuildsFromServer(requestedFor, branchName, 100);
		}

		/// <summary>
		/// Query VSTS for builds.
		/// </summary>
		/// <param name="requestedFor"></param>
		/// <param name="branchName"></param>
		/// <param name="top"></param>
		/// <returns></returns>
		private async Task<List<WebApi.Build>> _getBuildsFromServer(string requestedFor = default(string), string branchName = default(string), int? top = default(int?))
		{
			using (var client = await _getBuildHttpClient())
			{
				var builds = await client.GetBuildsAsync(
					project      : _projectName,
					requestedFor : requestedFor,
					branchName   : branchName,
					top          : top
				);

				foreach (var build in builds.Where(o => string.IsNullOrEmpty(o.SourceBranch)))
				{
					build.SourceBranch = branchName;
				}

				_updateBuildCache(builds);

				return builds;
			}
		}

		/// <summary>
		/// Query VSTS for a single build.
		/// </summary>
		/// <param name="buildId"></param>
		/// <returns></returns>
		private async Task<WebApi.Build> _getBuildFromServer(int buildId)
		{
			using (var client = await _getBuildHttpClient())
			{
				var build = await client.GetBuildAsync(_projectName, buildId);

				_updateBuildCache(build);

				return build;
			}
		}

		/// <summary>
		/// Obtain a new HTTP client to query VSTS.
		/// </summary>
		/// <returns></returns>
		private Task<BuildHttpClient> _getBuildHttpClient()
		{
			var connection = new VssConnection(_vstsUri, new VssAadCredential());
			return connection.GetClientAsync<BuildHttpClient>();
		}

		/// <summary>
		/// Updates the local build cache with a single build and signals build changes when there are any.
		/// </summary>
		/// <param name="build"></param>
		/// <returns></returns>
		private Contract.Build _updateBuildCache(WebApi.Build build)
		{
			var mappedBuild = Mapper.Map<Contract.Build>(build);

			return _queriedBuilds.AddOrUpdate(mappedBuild.Id, mappedBuild, (id, existing) =>
			{
				if (existing != mappedBuild)
				{
					// Clone the existing build for event notification
					_queriedBuilds[id] = mappedBuild;

					// Notify of the change
					OnBuildChanged(new BuildChangedEventArgs(
						existing,
						mappedBuild
					));
				}

				return existing;
			});
		}

		/// <summary>
		/// Updates the build cache with multiple builds.
		/// </summary>
		/// <param name="builds"></param>
		private void _updateBuildCache(List<WebApi.Build> builds)
		{
			builds.ForEach(build => _updateBuildCache(build));
		}

		/// <summary>
		/// Retrieve pending builds from the `builds` enumerable, optionally filtered by `requestedFor` and `branchName`.
		/// </summary>
		/// <param name="builds"></param>
		/// <param name="requestedFor"></param>
		/// <param name="branchName"></param>
		/// <returns></returns>
		private static IEnumerable<Contract.Build> _getPendingBuilds(IEnumerable<Contract.Build> builds, string requestedFor = default(string), string branchName = default(string))
		{
			builds = builds.Where(build =>
			   build.Status.HasValue
			   && (build.Status.Value & (Contract.BuildStatus.InProgress | Contract.BuildStatus.Cancelling | Contract.BuildStatus.Postponed | Contract.BuildStatus.NotStarted)) != Contract.BuildStatus.None
			);

			return _getBuilds(builds, requestedFor, branchName);
		}

		/// <summary>
		/// Retrieve builds from the `builds` enumerable, optionally filtered by `requestedFor` and `branchName`.
		/// </summary>
		/// <param name="builds"></param>
		/// <param name="requestedFor"></param>
		/// <param name="branchName"></param>
		/// <returns></returns>
		private static IEnumerable<Contract.Build> _getBuilds(IEnumerable<Contract.Build> builds, string requestedFor = default(string), string branchName = default(string))
		{
			if (!string.IsNullOrEmpty(requestedFor))
			{
				builds = builds.Where(o => o.Status.HasValue && o.RequestedFor == requestedFor);
			}

			if (!string.IsNullOrEmpty(branchName))
			{
				builds = builds.Where(o => o.Status.HasValue && o.SourceBranch == branchName);
			}

			return builds;
		}

		/// <summary>
		/// Retrieve pending builds from VSTS, filtered by `requestedFor` and `branchName`.
		/// </summary>
		/// <param name="requestedFor"></param>
		/// <param name="branchName"></param>
		/// <returns></returns>
		public Task<IEnumerable<Contract.Build>> GetPendingBuilds(string requestedFor = default(string), string branchName = default(string))
		{
			return Initialize(requestedFor, branchName)
				.ContinueWith(t => _getPendingBuilds(_queriedBuilds.Select(o => o.Value), requestedFor, branchName));
		}

		/// <summary>
		/// Retrieve builds from VSTS, optionally filtered by `requestedFor` and `branchName`.
		/// </summary>
		/// <param name="requestedFor"></param>
		/// <param name="branchName"></param>
		/// <returns></returns>
		public Task<IEnumerable<Contract.Build>> GetBuilds(string requestedFor = default(string), string branchName = default(string))
		{
			return Initialize(requestedFor, branchName)
				.ContinueWith(t => _getBuilds(_queriedBuilds.Select(o => o.Value), requestedFor, branchName));
		}

		/// <summary>
		/// Retrieve a single build from VSTS.
		/// </summary>
		/// <param name="buildId"></param>
		/// <returns></returns>
		public Task<Contract.Build> GetBuild(int buildId)
		{
			return _getBuildFromServer(buildId)
				.ContinueWith(t => Mapper.Map<Contract.Build>(t.Result));
		}

		/// <summary>
		/// Triggers when a previously tracked build changes in any way.
		/// </summary>
		public event BuildChangedEventHandler BuildChanged;

		/// <summary>
		/// Trigger BuildChanged.
		/// </summary>
		/// <param name="e"></param>
		private void OnBuildChanged(BuildChangedEventArgs e)
		{
			var handler = BuildChanged;

			handler?.Invoke(this, e);
		}
	}
}
