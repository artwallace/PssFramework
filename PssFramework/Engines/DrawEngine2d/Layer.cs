using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Drawables;

namespace PsmFramework.Engines.DrawEngine2d
{
	public class Layer : IDisposable
	{
		#region Constructor, Dispose
		
		public Layer()
		{
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize()
		{
			InitializeDrawables();
		}
		
		private void Cleanup()
		{
			CleanupDrawables();
		}
		
		#endregion
		
		#region Update, Render
		
		public void Update()
		{
		}
		
		public void Render()
		{
			foreach(DrawableBase item in Items)
				item.Render();
		}
		
		#endregion
		
		#region Drawables
		
		private void InitializeDrawables()
		{
			Items = new List<IDrawable>();
		}
		
		private void CleanupDrawables()
		{
			foreach(DrawableBase item in Items)
				item.Dispose();
			Items.Clear();
			
			Items = null;
		}
		
		public List<IDrawable> Items { get; private set; }
		
		#endregion
	}
}
