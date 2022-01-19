namespace HoPoSim.Data.Domain
{
	public abstract class Range<T> : BaseEntity
	{
		public Range()
		{ }

		public Range(Range<T> copyThis)
		{
			RangeId = copyThis.RangeId;
			MinValue = copyThis.MinValue;
			MaxValue = copyThis.MaxValue;
		}

		public abstract object Clone();

		public int RangeId { get; set; }

		public T MinValue
		{
			get;
			set;
		}

		public T MaxValue
		{
			get;
			set;
		}
	}
}
