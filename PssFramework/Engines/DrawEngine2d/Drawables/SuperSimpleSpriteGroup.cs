using System;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class SuperSimpleSpriteGroup : IDisposable, IDrawable
	{
		#region Constructor, Dispose
		
		public SuperSimpleSpriteGroup(Layer layer)
		{
			Initialize(layer);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(Layer layer)
		{
			InitializeLayer(layer);
			InitializeTiledTexture();
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
			Layer.Items.Add(this);
		}
		
		private void CleanupLayer()
		{
			Layer.Items.Remove(this);
			Layer = null;
		}
		
		protected Layer Layer;
		
		#endregion
		
		#region TiledTexture
		
		private void InitializeTiledTexture()
		{
		}
		
		private void CleanupTiledTexture()
		{
		}
		
		#endregion
		
		#region Color
		#endregion
		
		#region Blend Mode
		#endregion
		
		#region Drawables
		#endregion
	}
}

