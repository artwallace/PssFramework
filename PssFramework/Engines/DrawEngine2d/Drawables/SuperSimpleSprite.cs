using System;
using PsmFramework.Engines.DrawEngine2d.Support;

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
			InitializePosition();
			InitializeScale();
			InitializeRotation();
		}
		
		private void Cleanup()
		{
			CleanupPosition();
			CleanupScale();
			CleanupRotation();
			CleanupSpriteGroup();
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
		
		private void InitializePosition()
		{
			Position = Coordinate2.X0Y0;
		}
		
		private void CleanupPosition()
		{
		}
		
		public Coordinate2 Position;
		
		#endregion
		
		#region Scale
		
		private void InitializeScale()
		{
			Scale = 1.0f;
		}
		
		private void CleanupScale()
		{
		}
		
		public Single Scale;
		
		#endregion
		
		#region Rotation
		
		private void InitializeRotation()
		{
			Rotation = 0.0f;
		}
		
		private void CleanupRotation()
		{
		}
		
		public Single Rotation;
		
		#endregion
	}
}

