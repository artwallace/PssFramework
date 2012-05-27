using System;
using System.Collections.Generic;
using PssFramework.Engines.DrawEngine2d.DrawItems;

namespace PssFramework.Engines.DrawEngine2d
{
	public class Layer : IDisposable
	{
		#region Constructor, Dispose
		
		public Layer()
		{
		}
		
		public void Dispose()
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize()
		{
			InitializeDrawItems();
		}
		
		private void Cleanup()
		{
			CleanupDrawItems();
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
		
		#region DrawItems
		
		private void InitializeDrawItems()
		{
			Items = new List<DrawItemBase>();
		}
		
		private void CleanupDrawItems()
		{
			foreach(DrawItemBase item in Items)
				item.Dispose();
			Items.Clear();
			
			Items = null;
		}
		
		public List<DrawItemBase> Items { get; private set; }
		
		#endregion
	}
}
