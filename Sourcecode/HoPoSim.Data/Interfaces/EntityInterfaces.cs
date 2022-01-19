namespace HoPoSim.Data.Interfaces
{
	public interface IHaveIdProperty
	{
		int Id { get; set; }
	}

	public interface IHaveNameProperty
	{
		string Name { get; set; }
	}

	public interface IHaveActiveProperty
	{
		bool Active { get; set; }
	}

	public interface IHaveDisplayNameProperty
	{
		string DisplayName { get; }
	}
}
