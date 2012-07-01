using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Drawables;

namespace PsmFramework.Engines.DrawEngine2d
{
	//TODO: Add scales with world
	//TODO: Add rotates with world
	public sealed class Layer : IDisposable
	{
		#region Constructor, Dispose
		
		public Layer(DrawEngine2d drawEngine2d, Int32 zIndex, LayerType type = LayerType.World)
		{
			Initialize(drawEngine2d, zIndex, type);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(DrawEngine2d drawEngine2d, Int32 zIndex, LayerType type)
		{
			InitializeZIndex(zIndex);
			InitializeType(type);
			InitializeDrawEngine2d(drawEngine2d);
			InitializeDrawables();
		}
		
		private void Cleanup()
		{
			CleanupDrawables();
			CleanupDrawEngine2d();
			CleanupType();
			CleanupZIndex();
		}
		
		#endregion
		
		#region Render
		
		public void Render()
		{
			foreach(DrawableBase item in Items)
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
		
		#region Type
		
		private void InitializeType(LayerType type)
		{
			Type = type;
		}
		
		private void CleanupType()
		{
		}
		
		public LayerType Type { get; private set; }
		
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
		
		internal DrawEngine2d DrawEngine2d;
		
		#endregion
		
		#region Drawables
		
		private void InitializeDrawables()
		{
			Items = new List<DrawableBase>();
		}
		
		private void CleanupDrawables()
		{
			DrawableBase[] items = Items.ToArray();
			
			foreach(DrawableBase item in items)
				item.Dispose();
			Items.Clear();
			
			Items = null;
		}
		
		private List<DrawableBase> Items { get; set; }
		
		internal void AddDrawable(DrawableBase item)
		{
			if(item == null)
				throw new ArgumentNullException();
			
			if(Items.Contains(item))
				throw new ArgumentException();
			
			Items.Add(item);
			DrawEngine2d.SetRenderRequired();
		}
		
		internal void RemoveDrawable(DrawableBase item)
		{
			if(item == null)
				throw new ArgumentNullException();
			
			if(!Items.Contains(item))
				throw new ArgumentException();
			
			Items.Remove(item);
			DrawEngine2d.SetRenderRequired();
		}
		
		#endregion
	}
}
