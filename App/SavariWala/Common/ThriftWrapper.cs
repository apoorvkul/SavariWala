using System;
using Thrift.Transport;
using Thrift.Protocol;
using Thrift;

namespace SavariWala.Common
{
	public class ThriftWrapper<T> : IDisposable where T : class
	{
		TSocket _transport;
		public T Client { get; private set; }

		public ThriftWrapper ()
		{
			_transport = new TSocket(AppCommon.Inst.ServerAddr, AppCommon.Inst.ServerPort);
			Client = new T(new TBinaryProtocol(_transport));
			_transport.Open();
		}

		public void Dispose ()
		{
			_transport.Close ();	
		}
	}
}

