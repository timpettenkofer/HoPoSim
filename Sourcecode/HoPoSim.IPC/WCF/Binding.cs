using System.ServiceModel;

namespace HoPoSim.IPC.WCF
{
	public class Binding
	{
		private static int MaxSize = 2147483647;

		public static System.ServiceModel.Channels.Binding Create()
		{
			var binding = new NetTcpBinding(SecurityMode.None)
			{
				MaxBufferPoolSize = MaxSize,
				MaxBufferSize = MaxSize,
				MaxReceivedMessageSize = MaxSize
			};
			binding.ReaderQuotas.MaxDepth = MaxSize;
			binding.ReaderQuotas.MaxArrayLength = MaxSize;
			binding.ReaderQuotas.MaxBytesPerRead = MaxSize;
			binding.ReaderQuotas.MaxStringContentLength = MaxSize;
			return binding;
		}
	}
}
