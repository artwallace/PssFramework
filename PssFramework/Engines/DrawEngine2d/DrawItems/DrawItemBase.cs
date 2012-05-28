using System;

namespace PssFramework.Engines.DrawEngine2d.DrawItems
{
	public abstract class DrawItemBase : IDisposable
	{
		#region Constructor, Dispose
		
		public DrawItemBase(DrawEngine2d drawEngine2d)
		{
			InitializeInternal(drawEngine2d);
		}
		
		public virtual void Dispose()
		{
			CleanupInternal();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		public void InitializeInternal(DrawEngine2d drawEngine2d)
		{
			InitializeDrawEngine2d(drawEngine2d);
		}
		
		public void CleanupInternal()
		{
			CleanupDrawEngine2d();
		}
		
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
