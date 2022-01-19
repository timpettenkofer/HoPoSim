using HoPoSim.Data.Domain;
using HoPoSim.Data.Model;

namespace HoPoSim.IO.Interfaces
{
	public interface IStammlisteWriter
	{
		void WriteStammdaten(string filepath, GeneratorData input, Stammdaten data);
	}
}
