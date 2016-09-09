using AutoMapper;
using WebApi = Microsoft.TeamFoundation.Build.WebApi;
using System.Text.RegularExpressions;

namespace VSTS.Configuration
{
	/// <summary>
	/// Configures AutoMapper.
	/// </summary>
	public static class AutoMapperConfiguration
	{
		// The regex is a lazy test for the rules defined here https://support.microsoft.com/en-us/kb/909264
		private static Regex _userNameReplacementRegex = new Regex(@"^[^.][^\/:*?""<>|~!@#$%&'.(){} ]+\\");

		/// <summary>
		/// Causes static class to initialize.
		/// </summary>
		public static void Initialize() { }

		static AutoMapperConfiguration()
		{
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<WebApi.Build, Contract.Build>()
					.ForMember(
						contractBuild => contractBuild.RequestedFor,
						expression => expression.MapFrom(
							webApiBuild => _userNameReplacementRegex.Replace(webApiBuild.RequestedFor.UniqueName, "")
						)
					);
			});
		}
	}
}