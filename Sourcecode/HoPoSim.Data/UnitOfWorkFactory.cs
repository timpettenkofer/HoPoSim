using HoPoSim.Data;
using HoPoSim.Data.Interfaces;
using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using System.ComponentModel.Composition;

namespace HoPoSim.Model
{
	[Export(typeof(IUnitOfWorkFactory))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class UnitOfWorkFactory : IUnitOfWorkFactory
	{
		[ImportingConstructor]
		public UnitOfWorkFactory(IInteractionService interaction)
		{
			_interaction = interaction;
		}
		IInteractionService _interaction;

		[Import]
		public IGlobalConfigService Settings;

		public IUnitOfWork Create()
		{
			var context = new Context(Settings.DatabaseDirectory, _interaction);
			return new UnitOfWork(context);
		}
	}
}
