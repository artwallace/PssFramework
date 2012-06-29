using System;

namespace PsmFramework
{
	public interface IDisposablePlus : IDisposable
	{
		Boolean IsDisposed { get; }
	}
}

