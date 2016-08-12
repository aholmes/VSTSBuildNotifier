using System;

namespace VSTS
{
	public class BuildChangedEventArgs : EventArgs
	{
		public BuildChangedEventArgs(Contract.Build oldBuild, Contract.Build newBuild)
		{
			OldBuild = oldBuild;
			NewBuild = newBuild;
		}

		public virtual Contract.Build OldBuild { get; }
		public virtual Contract.Build NewBuild { get; }
	}
}
