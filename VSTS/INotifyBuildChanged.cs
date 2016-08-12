using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSTS
{
	public interface INotifyBuildChanged
	{
		event BuildChangedEventHandler BuildChanged;
	}
}
