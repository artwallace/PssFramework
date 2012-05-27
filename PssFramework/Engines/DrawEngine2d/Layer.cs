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
			//Items = new SortedList<Int32, DrawItemBase>();
			Items = new List<DrawItemBase>();
		}
		
		private void CleanupDrawItems()
		{
			//foreach(Int32 zIndex in Items.Keys)
			//{
				//DrawItemBase item = Items[zIndex];
				//Items.Remove(zIndex);
				//item.Dispose();
			//}
			
			foreach(DrawItemBase item in Items)
			{
				item.Dispose();
			}
			Items.Clear();
			
			Items = null;
		}
		
		//public SortedList<Int32, DrawItemBase> Items { get; private set; }
		public List<DrawItemBase> Items { get; private set; }
		
		#endregion
	}
}
