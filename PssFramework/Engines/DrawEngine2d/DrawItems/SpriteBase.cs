using System;

namespace PssFramework.Engines.DrawEngine2d.DrawItems
{
	public abstract class SpriteBase : DrawItemBase
	{
		#region Constructor, Dispose
		
		public SpriteBase(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
	}
}

