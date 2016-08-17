using AutoMapper;
using WebApi = Microsoft.TeamFoundation.Build.WebApi;

namespace VSTS.Configuration
{
	public static class AutoMapperConfiguration
	{
		public static void Initialize() { }

		static AutoMapperConfiguration()
		{
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<WebApi.Build, Contract.Build>()
					.ForMember(contractBuild => contractBuild.RequestedFor, expression => expression.MapFrom(webApiBuild => webApiBuild.RequestedFor.DisplayName));
			});
		}
	}
}