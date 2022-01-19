using HoPoSim.Framework.Interfaces;
using System;
using System.ComponentModel.Composition;

namespace HoPoSim.Data
{
	[Export(typeof(IExceptionMessageService))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class ExceptionMessageService : IExceptionMessageService
	{
		public string Translate(Exception e)
		{
			var sqlException = e as Microsoft.Data.Sqlite.SqliteException;

			if (sqlException != null)
			{
				if (sqlException.SqliteErrorCode == 19)
				{
					if (sqlException.Message.Contains("FOREIGN KEY constraint failed"))
						return Properties.Resources.SqlError_19_ForeignKey_Constraint_Failed_Message;
					if (sqlException.Message.Contains("UNIQUE constraint failed"))
						return Properties.Resources.SqlError_19_Unique_Constraint_Failed_Message;
				}
				return sqlException.Message;
			}
			return e != null ? e.Message : string.Empty;
		}
	}
}
