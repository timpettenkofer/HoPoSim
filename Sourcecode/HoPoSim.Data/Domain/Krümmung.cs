namespace HoPoSim.Data.Domain
{
	public class Krümmung : Range<int>
	{
		public Krümmung()
		{ }

		public Krümmung(Krümmung copyThis)
			: base(copyThis)
		{
		}

		public override object Clone()
		{
			return new Krümmung(this);
		}
	}
}
