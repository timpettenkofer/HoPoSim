namespace HoPoSim.Data.Domain
{
	public class Abholzigkeit : Range<int>
	{
		public Abholzigkeit()
		{ }

		public Abholzigkeit(Abholzigkeit copyThis)
			: base(copyThis)
		{
		}

		public override object Clone()
		{
			return new Abholzigkeit(this);
		}
	}
}
