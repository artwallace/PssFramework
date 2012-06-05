using System;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class SuperSimpleSpriteGroup : IDisposable, IDrawable
	{
		#region Constructor, Dispose
		
		public SuperSimpleSpriteGroup()
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
		
		#region Texture
		
		#endregion
		
		#region Color
		#endregion
		
		#region Blend Mode
		#endregion
	}
}

