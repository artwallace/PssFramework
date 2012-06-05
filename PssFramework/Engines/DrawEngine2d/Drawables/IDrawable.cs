using System;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public interface IDrawable : IDisposable
	{
		void Render();
	}
}

