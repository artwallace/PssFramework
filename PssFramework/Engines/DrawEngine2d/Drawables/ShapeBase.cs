using System;

namespace PssFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class ShapeBase : DrawableBase
	{
		#region Constructor, Dispose
		
		public ShapeBase(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
	}
}

