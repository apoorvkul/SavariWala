using System;
using Thrift;

namespace SavariWala.Common
{
	public abstract class ErrorTranslator
	{
		public abstract Tuple<string, string> GetErrMessage (TException ex);
	}
}

