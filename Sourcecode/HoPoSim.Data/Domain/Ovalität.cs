namespace HoPoSim.Data.Domain
{
	public class Ovalität : Range<double>
	{
		public Ovalität()
		{ }

		public Ovalität(Ovalität copyThis)
			: base(copyThis)
		{
		}

		public override object Clone()
		{
			return new Ovalität(this);
		}
	}
}
