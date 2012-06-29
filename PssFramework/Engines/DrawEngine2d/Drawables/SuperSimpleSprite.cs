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
		
		private Coordinate2 _Position;
		public Coordinate2 Position
		{
			get { return _Position; }
			set
			{
				if(_Position == value)
					return;
				
				_Position = value;
				
				SpriteGroup.Layer.DrawEngine2d.SetRenderRequired();
			}
		}
		
		public void SetPositionFromCenter(Coordinate2 position)
		{
			Single x = position.X - SpriteGroup.TileWidth / 2;
			Single y = position.Y - SpriteGroup.TileHeight / 2;
			
			Position = new Coordinate2(x, y);
		}
		
		public void SetPositionFromCenter(Single x, Single y)
		{
			Single xx = x - SpriteGroup.TileWidth / 2;
			Single yy = y - SpriteGroup.TileHeight / 2;
			
			Position = new Coordinate2(xx, yy);
		}
		
		#endregion
		
		#region Scale
		
		private void InitializeScale()
		{
			Scale = 1.0f;
		}
		
		private void CleanupScale()
		{
		}
		
		private Single _Scale;
		public Single Scale
		{
			get { return _Scale; }
			set
			{
				if(_Scale == value)
					return;
				
				_Scale = value;
				
				SpriteGroup.Layer.DrawEngine2d.SetRenderRequired();
			}
		}
		
		#endregion
		
		#region Rotation
		
		private void InitializeRotation()
		{
			Rotation = 0.0f;
		}
		
		private void CleanupRotation()
		{
		}
		
		private Single _Rotation;
		public Single Rotation
		{
			get { return _Rotation; }
			set
			{
				if(_Rotation == value)
					return;
				
				_Rotation = value;
				
				SpriteGroup.Layer.DrawEngine2d.SetRenderRequired();
			}
		}
		
		#endregion
	}
}

