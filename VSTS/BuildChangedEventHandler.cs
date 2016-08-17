namespace VSTS
{
	/// <summary>
	/// Used with INotifyBuildChanged objects, and is executed when a build changes.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void BuildChangedEventHandler(object sender, BuildChangedEventArgs e);
}