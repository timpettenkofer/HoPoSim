namespace HoPoSim.Data.Domain
{
	public class Durchmesser : Range<int>
	{
		public Durchmesser()
		{ }

		public Durchmesser(Durchmesser copyThis) 
			: base(copyThis)
		{
			Rindenstärke = copyThis.Rindenstärke;
		}

		public override object Clone()
		{
			return new Durchmesser(this);
		}

		public int Rindenstärke { get; set; }
	}
}
