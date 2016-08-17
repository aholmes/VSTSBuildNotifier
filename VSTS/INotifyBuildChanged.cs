namespace VSTS
{
	public interface INotifyBuildChanged
	{
		event BuildChangedEventHandler BuildChanged;
	}
}
