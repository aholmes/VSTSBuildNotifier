using System;

namespace VSTS
{
	/// <summary>
	/// Used with INotifyBuildChanged objects and BuildChangedEventHandler events.
	/// </summary>
	public class BuildChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Obtain a new instance.
		/// 
		/// Warning: assigns OldBuild and NewBuild.
		/// </summary>
		/// <param name="oldBuild"></param>
		/// <param name="newBuild"></param>
		public BuildChangedEventArgs(Contract.Build oldBuild, Contract.Build newBuild)
		{
			OldBuild = oldBuild;
			NewBuild = newBuild;
		}

		/// <summary>
		/// The build as it was before the change event.
		/// </summary>
		public virtual Contract.Build OldBuild { get; }

		/// <summary>
		/// The build that triggered the change event.
		/// </summary>
		public virtual Contract.Build NewBuild { get; }
	}
}
