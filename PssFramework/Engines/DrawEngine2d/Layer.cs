using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Drawables;

namespace PsmFramework.Engines.DrawEngine2d
{
	//TODO: Should layer be responsible for registering/unregistering itself with the DrawEngine?
	public class Layer : IDisposable
	{
		#region Constructor, Dispose
		
		public Layer(DrawEngine2d drawEngine2d, Int32 zIndex)
		{
			Initialize(drawEngine2d, zIndex);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(DrawEngine2d drawEngine2d, Int32 zIndex)
		{
			InitializeZIndex(zIndex);
			InitializeDrawEngine2d(drawEngine2d);
			InitializeDrawables();
		}
		
		private void Cleanup()
		{
			CleanupDrawables();
			CleanupDrawEngine2d();
			CleanupZIndex();
		}
		
		#endregion
		
		#region Render
		
		public void Render()
		{
			foreach(IDrawable item in Items)
				item.Render();
		}
		
		#endregion
		
		#region ZIndex
		
		private void InitializeZIndex(Int32 zIndex)
		{
			ZIndex = zIndex;
		}
		
		private void CleanupZIndex()
		{
		}
		
		public Int32 ZIndex { get; private set; }
		
		#endregion
		
		#region DrawEngine
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d)
		{
			DrawEngine2d = drawEngine2d;
			DrawEngine2d.AddLayer(this, ZIndex);
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d.RemoveLayer(this);
			DrawEngine2d = null;
		}
		
		public DrawEngine2d DrawEngine2d;
		
		#endregion
		
		#region Drawables
		
		private void InitializeDrawables()
		{
			Items = new List<IDrawable>();
		}
		
		private void CleanupDrawables()
		{
			foreach(IDrawable item in Items)
				item.Dispose();
			Items.Clear();
			
			Items = null;
		}
		
		public List<IDrawable> Items { get; private set; }
		
		#endregion
	}
}
