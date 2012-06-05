using System;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	//This is not a Drawable, the group is.
	public class SuperSimpleSprite : IDisposable
	{
		#region Constructor, Dispose
		
		public SuperSimpleSprite()
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
		}
		
		private void Cleanup()
		{
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
	}
}

