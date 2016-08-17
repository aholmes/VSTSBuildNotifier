namespace VSTS
{
	/// <summary>
	/// Contract for notifier objects of changes to Contract.Build objects.
	/// </summary>
	public interface INotifyBuildChanged
	{
		event BuildChangedEventHandler BuildChanged;
	}
}
