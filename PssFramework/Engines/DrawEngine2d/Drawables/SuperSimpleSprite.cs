using System;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	//This is not a Drawable, the group is.
	public sealed class SuperSimpleSprite : IDisposable
	{
		#region Constructor, Dispose
		
		public SuperSimpleSprite(SuperSimpleSpriteGroup spriteGroup)
		{
			Initialize(spriteGroup);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(SuperSimpleSpriteGroup spriteGroup)
		{
			InitializeSpriteGroup(spriteGroup);
		}
		
		private void Cleanup()
		{
		}
		
		#endregion
		
		#region Update, Render
		
		public void Update()
		{
		}
		
		public void Render()
		{
		}
		
		#endregion
		
		#region SpriteGroup
		
		private void InitializeSpriteGroup(SuperSimpleSpriteGroup spriteGroup)
		{
			SpriteGroup = spriteGroup;
			SpriteGroup.AddSprite(this);
		}
		
		private void CleanupSpriteGroup()
		{
			SpriteGroup = null;
			SpriteGroup.RemoveSprite(this);
		}
		
		private SuperSimpleSpriteGroup SpriteGroup;
		
		#endregion
		
		#region Position
		#endregion
	}
}

