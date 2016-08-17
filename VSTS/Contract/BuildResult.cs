using System;

namespace VSTS.Contract
{
	[Flags]
	public enum BuildResult
	{
		//
		// Summary:
		//     No result
		None = 0,
		//
		// Summary:
		//     The build completed successfully.
		Succeeded = 1 << 1,
		//
		// Summary:
		//     The build completed compilation successfully but had other errors.
		PartiallySucceeded = 1 << 2,
		//
		// Summary:
		//     The build completed unsuccessfully.
		Failed = 1 << 3,
		//
		// Summary:
		//     The build was canceled before starting.
		Canceled = 1 << 5
	}
}
