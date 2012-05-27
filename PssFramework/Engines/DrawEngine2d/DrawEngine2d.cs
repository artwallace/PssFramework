using System;
using System.Collections.Generic;
using PssFramework.Engines.DrawEngine2d.Support;
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
			
			Initialize(graphicsContext);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(GraphicsContext graphicsContext)
		{
			InitializeGraphicsContext(graphicsContext);
			InitializeClearColor();
			InitializeLayers();
		}
		
		private void Cleanup()
		{
			CleanupClearColor();
			CleanupLayers();
			CleanupGraphicsContext();
		}
		
		#endregion
		
		#region GraphicsContext
		
		private void InitializeGraphicsContext(GraphicsContext graphicsContext)
		{
			GraphicsContext = graphicsContext;
		}
		
		private void CleanupGraphicsContext()
		{
			GraphicsContext = null;
		}
		
		internal GraphicsContext GraphicsContext { get; private set; }
		
		#endregion
		
		#region ClearColor
		
		private void InitializeClearColor()
		{
			ClearColor = Colors.Black;
		}
		
		private void CleanupClearColor()
		{
			ClearColor = Colors.Black;
		}
		
		private Color _ClearColor;
		public Color ClearColor
		{
			get { return _ClearColor; }
			set
			{
				_ClearColor = value;
				GraphicsContext.SetClearColor(_ClearColor.AsVector4);
			}
		}
		
		#endregion
		
		#region Camera
		
		public Camera Camera { get; private set; }
		
		#endregion
		
		#region Update, Render
		
		//TODO: Is update needed just to draw?
		public void Update()
		{
			foreach(Layer layer in Layers.Values)
				layer.Update();
		}
		
		public void Render()
		{
			GraphicsContext.Clear();
			
			foreach(Layer layer in Layers.Values)
				layer.Render();
			
			GraphicsContext.SwapBuffers();
		}
		
		#endregion
		
		#region Layers
		
		private void InitializeLayers()
		{
			Layers = new SortedList<Int32, Layer>();
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
		
		private SortedList<Int32, Layer> Layers { get; set; }
		
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
