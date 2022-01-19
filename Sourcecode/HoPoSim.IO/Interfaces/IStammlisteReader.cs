using HoPoSim.Data.Model;

namespace HoPoSim.IO.Interfaces
{
	public interface IStammlisteReader
	{
		Stammdaten ReadStammdaten(string filepath);
	}
}
