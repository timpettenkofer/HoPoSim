using HoPoSim.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace HoPoSim.Data.Domain
{
	public interface IEntity : IHaveIdProperty
	{ }

	public interface IActivableEntity : IEntity, IHaveActiveProperty
	{ }

	public class BaseActivableEntity : BaseEntity, IActivableEntity
	{
		public BaseActivableEntity() { }

		public BaseActivableEntity(BaseActivableEntity entity)
		{
			Active = entity.Active;
		}

		public bool Active
		{
			get { return m_active; }
			set { m_active = value; }
		}
		private bool m_active = true;
	}

	public class BaseEntity : IEntity
	{
		[Key]
		public int Id { get; set; }
	}
}
