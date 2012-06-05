using System;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
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

