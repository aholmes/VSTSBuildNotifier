using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WebApi = Microsoft.TeamFoundation.Build.WebApi;
using System.Reflection;

namespace VSTS.Configuration
{
	public static class AutoMapperConfiguration
	{
		public static void Initialize() { }

		static AutoMapperConfiguration()
		{
			var mappingExpressions = new List<object>();
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<WebApi.Build, Contract.Build>()
                    .ForMember(contractBuild => contractBuild.RequestedFor, expression => expression.MapFrom(webApiBuild => webApiBuild.RequestedFor.DisplayName));
				cfg.CreateMap<WebApi.BuildStatus?, Contract.BuildStatus?>();
				cfg.CreateMap<WebApi.BuildResult?, Contract.BuildResult?>();
			});
		}
	}
}