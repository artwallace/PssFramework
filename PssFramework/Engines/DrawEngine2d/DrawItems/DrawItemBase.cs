using System;

namespace PssFramework.Engines.DrawEngine2d.DrawItems
{
	public abstract class DrawItemBase : IDisposable
	{
		#region Constructor, Dispose
		
		public DrawItemBase(DrawEngine2d drawEngine2d)
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
