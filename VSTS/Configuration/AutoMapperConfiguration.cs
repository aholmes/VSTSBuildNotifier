using AutoMapper;
using WebApi = Microsoft.TeamFoundation.Build.WebApi;

namespace VSTS.Configuration
{
	/// <summary>
	/// Configures AutoMapper.
	/// </summary>
	public static class AutoMapperConfiguration
	{
		/// <summary>
		/// Causes static class to initialize.
		/// </summary>
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