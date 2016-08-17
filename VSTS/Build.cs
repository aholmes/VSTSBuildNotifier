using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.TeamFoundation.Build.WebApi;
using WebApi = Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Client;
using System.ComponentModel;
using System.Threading;
using AutoMapper;

namespace VSTS
{
    public class Build : INotifyBuildChanged
    {
        private Uri _vstsUri;
        private string _projectName;

        private ObservableConcurrentDictionary<int, Contract.Build> _queriedBuilds = new ObservableConcurrentDictionary<int, Contract.Build>();

        public Build(string vstsUri, string collectionName, string projectName)
        {
            Configuration.AutoMapperConfiguration.Initialize();

            var uriBuilder = new UriBuilder(vstsUri);
            uriBuilder.Path = string.Concat(uriBuilder.Path.TrimEnd('/'), '/', collectionName);
            _vstsUri = uriBuilder.Uri;
            _projectName = projectName;
        }

        /// <summary>
        /// Initialize this class by storing the last 100 builds
        /// </summary>
        /// <param name="requestedFor"></param>
        /// <param name="branchName"></param>
        /// <returns></returns>
        public Task Initialize(string requestedFor, string branchName)
        {
            return _getBuildsFromServer(requestedFor, branchName, 100);
        }

        public Task Initialize(string branchName)
        {
            return _getBuildsFromServer(branchName: branchName, top: 100);
        }

        public Task Initialize()
        {
            return _getBuildsFromServer(top: 100);
        }

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

        private Task<BuildHttpClient> _getBuildHttpClient()
        {
            var connection = new VssConnection(_vstsUri, new VssAadCredential());
            return connection.GetClientAsync<BuildHttpClient>();
        }

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

        private void _updateBuildCache(List<WebApi.Build> builds)
        {
            builds.ForEach(build => _updateBuildCache(build));
        }

        public Task<WebApi.Build> GetLastBuild(string requestedFor, string branchName)
        {
            Func<Task<List<WebApi.Build>>, Task<WebApi.Build>> continuationTask = async (t) => (await t).First();

            return _getBuildsFromServer(requestedFor, branchName, 1)
                .ContinueWith(continuationTask)
                .Unwrap();
        }

        public Task<Contract.BuildStatus?> GetLastBuildStatus(string requestedFor, string branchName)
        {
            Func<Task<WebApi.Build>, Task<Contract.BuildStatus?>> continuationTask = async (t) => (Contract.BuildStatus?)((await t).Status);

            return GetLastBuild(requestedFor, branchName)
                .ContinueWith(continuationTask)
                .Unwrap();
        }

        public Task<Contract.BuildResult?> GetLastBuildResult(string requestedFor, string branchName)
        {
            Func<Task<WebApi.Build>, Task<Contract.BuildResult?>> continuationTask = async (t) => (Contract.BuildResult?)((await t).Result);

            return GetLastBuild(requestedFor, branchName)
                .ContinueWith(continuationTask)
                .Unwrap();
        }

        private static IEnumerable<Contract.Build> _getPendingBuilds(string requestedFor, string branchName, IEnumerable<Contract.Build> builds)
        {
            return builds.Where(build =>
               build.Status.HasValue
               && (build.Status.Value & (Contract.BuildStatus.InProgress | Contract.BuildStatus.Cancelling | Contract.BuildStatus.Postponed | Contract.BuildStatus.NotStarted)) != Contract.BuildStatus.None
               && (string.IsNullOrEmpty(requestedFor) || build.RequestedFor == requestedFor)
               && (string.IsNullOrEmpty(branchName) || build.SourceBranch == branchName)
           );
        }

        private static IEnumerable<Contract.Build> _getPendingBuilds(string branchName, IEnumerable<Contract.Build> builds)
        {
            return builds.Where(build =>
               build.Status.HasValue
               && (build.Status.Value & (Contract.BuildStatus.InProgress | Contract.BuildStatus.Cancelling | Contract.BuildStatus.Postponed | Contract.BuildStatus.NotStarted)) != Contract.BuildStatus.None
               && (string.IsNullOrEmpty(branchName) || build.SourceBranch == branchName)
           );
        }

        private static IEnumerable<Contract.Build> _getPendingBuilds(IEnumerable<Contract.Build> builds)
        {
            return builds.Where(build =>
               build.Status.HasValue
               && (build.Status.Value & (Contract.BuildStatus.InProgress | Contract.BuildStatus.Cancelling | Contract.BuildStatus.Postponed | Contract.BuildStatus.NotStarted)) != Contract.BuildStatus.None
           );
        }

        private static IEnumerable<Contract.Build> _getBuilds(string requestedFor, string branchName, IEnumerable<Contract.Build> builds)
        {
            return builds.Where(build =>
               build.Status.HasValue
               && (string.IsNullOrEmpty(requestedFor) || build.RequestedFor == requestedFor)
               && (string.IsNullOrEmpty(branchName) || build.SourceBranch == branchName)
            );
        }

        private static IEnumerable<Contract.Build> _getBuilds(string branchName, IEnumerable<Contract.Build> builds)
        {
            return builds.Where(build =>
               build.Status.HasValue
               && (string.IsNullOrEmpty(branchName) || build.SourceBranch == branchName)
            );
        }

        public async Task<IEnumerable<Contract.Build>> GetBuilds(string requestedFor, string branchName)
        {
            await Initialize(requestedFor, branchName);

            return _getBuilds(requestedFor, branchName, _queriedBuilds.Select(o => o.Value));
        }

        public async Task<IEnumerable<Contract.Build>> GetBuilds(string branchName)
        {
            await Initialize(branchName);

            return _getBuilds(branchName, _queriedBuilds.Select(o => o.Value));
        }

        public async Task<IEnumerable<Contract.Build>> GetPendingBuilds(string requestedFor, string branchName)
        {
            await Initialize(requestedFor, branchName);

            return _getPendingBuilds(requestedFor, branchName, _queriedBuilds.Select(o => o.Value));
        }

        public async Task<IEnumerable<Contract.Build>> GetPendingBuilds(string branchName)
        {
            await Initialize(branchName);

            return _getPendingBuilds(branchName, _queriedBuilds.Select(o => o.Value));
        }

        public async Task<IEnumerable<Contract.Build>> GetPendingBuilds()
        {
            await Initialize();

            return _getPendingBuilds(_queriedBuilds.Select(o => o.Value));
        }

        public async Task<Contract.Build> GetBuild(int buildId)
        {
            using (var client = await _getBuildHttpClient())
            {
                var build = await client.GetBuildAsync(_projectName, buildId);

                _updateBuildCache(build);

                return Mapper.Map<Contract.Build>(build);
            }
        }

        public event BuildChangedEventHandler BuildChanged;

        private void OnBuildChanged(BuildChangedEventArgs e)
        {
            var handler = BuildChanged;

            handler?.Invoke(this, e);
        }
    }
}