using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoPoSim.Data.Validation
{
	public interface IValidable
	{
		bool ValidateInstance();
		string ValidationErrorsAsString();
	}
}
