using System;
using System.Collections.Generic;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public sealed class SuperSimpleSpriteGroup : IDisposable, IDrawable
	{
		#region Constructor, Dispose
		
		public SuperSimpleSpriteGroup(Layer layer, TiledTexture tiledTexture)
		{
			Initialize(layer, tiledTexture);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(Layer layer, TiledTexture tiledTexture)
		{
			InitializeLayer(layer);
			InitializeTiledTexture(tiledTexture);
		}
		
		private void Cleanup()
		{
			CleanupTiledTexture();
			CleanupLayer();
		}
		
		#endregion
		
		#region Render
		
		public void Render()
		{
		}
		
		#endregion
		
		#region Layer
		
		private void InitializeLayer(Layer layer)
		{
			Layer = layer;
			Layer.AddDrawable(this);
		}
		
		private void CleanupLayer()
		{
			Layer.RemoveDrawable(this);
			Layer = null;
		}
		
		private Layer Layer;
		
		#endregion
		
		#region TiledTexture
		
		private void InitializeTiledTexture(TiledTexture tiledTexture)
		{
			if(tiledTexture == null)
				throw new ArgumentNullException();
			
			TiledTexture = tiledTexture;
		}
		
		private void CleanupTiledTexture()
		{
			
		}
		
		private TiledTexture TiledTexture;
		
		#endregion
		
		#region Color
		#endregion
		
		#region Blend Mode
		#endregion
		
		#region Sprites
		
		private void InitializeSprites()
		{
			Sprites = new List<SuperSimpleSprite>();
		}
		
		private void CleanupSprites()
		{
			SuperSimpleSprite[] sprites = Sprites.ToArray();
			
			foreach(SuperSimpleSprite sprite in sprites)
				sprite.Dispose();
			Sprites.Clear();
			
			Sprites = null;
		}
		
		private List<SuperSimpleSprite> Sprites;
		
		internal void AddSprite(SuperSimpleSprite sprite)
		{
			if(sprite == null)
				throw new ArgumentNullException();
			
			if(Sprites.Contains(sprite))
				throw new ArgumentException();
			
			Sprites.Add(sprite);
			Layer.DrawEngine2d.SetRenderRequired();
		}
		
		internal void RemoveSprite(SuperSimpleSprite sprite)
		{
			if(sprite == null)
				throw new ArgumentNullException();
			
			if(!Sprites.Contains(sprite))
				throw new ArgumentException();
			
			Sprites.Remove(sprite);
			Layer.DrawEngine2d.SetRenderRequired();
		}
		
		#endregion
	}
}

