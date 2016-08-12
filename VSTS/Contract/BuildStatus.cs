using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSTS.Contract
{
	[Flags]
	public enum BuildStatus
	{
		None = 0,
		//
		// Summary:
		//     The build is currently in progress.
		InProgress = 1,
		//
		// Summary:
		//     The build has completed.
		Completed = 1 << 1,
		//
		// Summary:
		//     The build is cancelling
		Cancelling = 1 << 2,
		//
		// Summary:
		//     The build is inactive in the queue.
		Postponed = 1 << 3,
		//
		// Summary:
		//     The build has not yet started.
		NotStarted = 1 << 5, // 5 is intentional
		//
		// Summary:
		//     All status.
		All = InProgress|Completed|Cancelling|Postponed|NotStarted
	}
}