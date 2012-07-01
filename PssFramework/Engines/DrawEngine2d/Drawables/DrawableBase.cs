using System;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class DrawableBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public DrawableBase(Layer layer)
		{
			InitializeInternal(layer);
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
			CleanupInternal();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal(Layer layer)
		{
			InitializeLayer(layer);
			InitializeDrawEngine2d();
		}
		
		private void CleanupInternal()
		{
			CleanupDrawEngine2d();
			CleanupLayer();
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
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
		
		public Layer Layer;
		
		#endregion
		
		#region DrawEngine2d
		
		private void InitializeDrawEngine2d()
		{
			DrawEngine2d = Layer.DrawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d = null;
		}
		
		public DrawEngine2d DrawEngine2d { get; private set; }
		
		#endregion
		
		#region Render
		
		public abstract void Render();
		
		#endregion
	}
}

