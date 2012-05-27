using System;

namespace PssFramework.Engines.DrawEngine2d.DrawItems
{
	public abstract class DrawItemBase : IDisposable
	{
		#region Constructor, Dispose
		
		public DrawItemBase()
		{
		}
		
		public abstract void Dispose();
		
		#endregion
	}
}

