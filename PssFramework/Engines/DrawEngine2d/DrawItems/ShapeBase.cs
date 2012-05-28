using System;

namespace PssFramework.Engines.DrawEngine2d.DrawItems
{
	public abstract class ShapeBase : DrawItemBase
	{
		#region Constructor, Dispose
		
		public ShapeBase(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
	}
}

