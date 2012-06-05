using System;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class ShapeBase : IDrawable, IDisposable
	{
		#region Constructor, Dispose
		
		public ShapeBase(Layer layer)
		{
		}
		
		public void Dispose()
		{
		}
		
		#endregion
		
		#region Render
		
		public void Render()
		{
		}
		
		#endregion
	}
}

