using System;
using System.Collections.Generic;
using Sce.Pss.Core.Graphics;

namespace PssFramework.Engines.DrawEngine2d
{
	public class DrawEngine2d : IDisposable
	{
		#region Constructor, Dispose
		
		public DrawEngine2d(GraphicsContext graphicsContext)
		{
			if (graphicsContext == null)
				throw new ArgumentNullException();
			
			GraphicsContext = graphicsContext;
			
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
			
			GraphicsContext = null;
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize()
		{
			InitializeLayers();
		}
		
		private void Cleanup()
		{
			CleanupLayers();
		}
		
		#endregion
		
		#region GraphicsContext
		
		//internal GraphicsContext GraphicsContext;
		internal GraphicsContext GraphicsContext { get; private set; }
		
		#endregion
		
		#region Camera
		
		public Camera Camera { get; private set; }
		
		#endregion
		
		#region Update, Render
		
		public void Update()
		{
			foreach(Layer layer in Layers.Values)
				layer.Update();
		}
		
		public void Render()
		{
			//GraphicsContext.Clear();
			
			foreach(Layer layer in Layers.Values)
				layer.Render();
		}
		
		#endregion
		
		#region Layers
		
		private void InitializeLayers()
		{
			Layers = new SortedDictionary<Int32, Layer>();
		}
		
		private void CleanupLayers()
		{
			foreach(Int32 zIndex in Layers.Keys)
			{
				Layer layer = Layers[zIndex];
				RemoveLayer(zIndex);
				layer.Dispose();
			}
			
			Layers = null;
		}
		
		private SortedDictionary<Int32, Layer> Layers { get; set; }
		
		public Layer CreateLayer(Int32 zIndex)
		{
			Layer layer = new Layer();
			Layers.Add(zIndex, layer);
			return layer;
		}
		
		public void RemoveLayer(Int32 zIndex)
		{
			Layers.Remove(zIndex);
		}
		
		#endregion
	}
}

