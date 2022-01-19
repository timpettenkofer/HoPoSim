using System;

namespace HoPoSim.Framework.Interfaces
{
	public interface IExceptionMessageService
	{
		string Translate(Exception e);
	}
}
