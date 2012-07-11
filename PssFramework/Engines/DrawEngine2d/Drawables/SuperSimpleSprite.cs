using System;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Engines.DrawEngine2d.Textures;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	//This is not a Drawable, the group is.
	public sealed class SuperSimpleSprite : IDisposable
	{
		//TODO: Convert to a struct once we get it working properly.
		
		#region Constructor, Dispose
		
		public SuperSimpleSprite(SuperSimpleSpriteGroup spriteGroup, TiledTextureIndex textureIndex)
		{
			Initialize(spriteGroup, textureIndex);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(SuperSimpleSpriteGroup spriteGroup, TiledTextureIndex textureIndex)
		{
			InitializeSpriteGroup(spriteGroup);
			InitializeTextureIndex(textureIndex);
			InitializePosition();
			InitializeScale();
			InitializeRotation();
		}
		
		private void Cleanup()
		{
			CleanupRotation();
			CleanupScale();
			CleanupPosition();
			CleanupTextureIndex();
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
			SpriteGroup.RemoveSprite(this);
			SpriteGroup = null;
		}
		
		private SuperSimpleSpriteGroup SpriteGroup;
		
		private void InformSpriteGroupThatUpdateIsRequired()
		{
		}
		
		#endregion
		
		#region TextureIndex
		
		private void InitializeTextureIndex(TiledTextureIndex textureIndex)
		{
			TextureIndex = textureIndex;
		}
		
		private void CleanupTextureIndex()
		{
		}
		
		private TiledTextureIndex _TextureIndex;
		public TiledTextureIndex TextureIndex
		{
			get { return _TextureIndex; }
			set
			{
				//if(_TextureIndex == value)
					//return;
				
				_TextureIndex = value;
				
				
				
				SpriteGroup.Layer.DrawEngine2d.SetRenderRequired();
			}
		}
		
		private Single[] CachedTextureCoordinates;
		
		public Int32 TileWidth { get; private set; }
		
		public Int32 TileHeight { get; private set; }
		
		private void UpdateCachedTextureCoordinates(TiledTextureIndex index)
		{
			Int32 width, height;
			
			CachedTextureCoordinates = SpriteGroup.GetTiledTextureCoordinates(_TextureIndex, out width, out height);
			TileWidth = width;
			TileHeight = height;
		}
		
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
			Single x = position.X - TileWidth / 2;
			Single y = position.Y - TileHeight / 2;
			
			Position = new Coordinate2(x, y);
		}
		
		public void SetPositionFromCenter(Single x, Single y)
		{
			Single xx = x - TileWidth / 2;
			Single yy = y - TileHeight / 2;
			
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
