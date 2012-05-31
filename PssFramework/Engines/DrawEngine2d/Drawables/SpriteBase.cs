using System;

namespace PssFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class SpriteBase : DrawableBase
	{
		#region Constructor, Dispose
		
		public SpriteBase(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
	}
}

