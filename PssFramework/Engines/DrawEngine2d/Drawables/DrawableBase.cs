using System;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class DrawableBase : IDisposable
	{
		#region Constructor, Dispose
		
		public DrawableBase(DrawEngine2d drawEngine2d)
		{
			InitializeInternal(drawEngine2d);
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
			CleanupInternal();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal(DrawEngine2d drawEngine2d)
		{
			InitializeDrawEngine2d(drawEngine2d);
		}
		
		private void CleanupInternal()
		{
			CleanupDrawEngine2d();
		}
		
		protected abstract void Initialize();
		
		protected abstract void Cleanup();
		
		#endregion
		
		#region Update, Render
		
		//TODO: Update() probably isn't necessary but we'll leave it for now.
		public abstract void Update();
		
		public abstract void Render();
		
		#endregion
		
		#region DrawEngine
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d)
		{
			DrawEngine2d = drawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			//TODO: Possibly remove itself from SpriteList/Layer/TextureManager?
			DrawEngine2d = null;
		}
		
		protected DrawEngine2d DrawEngine2d;
		
		#endregion
	}
}
