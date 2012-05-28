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
			InitializeRenderRequiredFlag();
		}
		
		private void Cleanup()
		{
			CleanupRenderRequiredFlag();
			CleanupClearColor();
			CleanupLayers();
			CleanupGraphicsContext();
		}
		
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
			if(!RenderRequired)
				return;
			
			GraphicsContext.Clear();
			
			foreach(Layer layer in Layers.Values)
				layer.Render();
			
			GraphicsContext.SwapBuffers();
			
			ResetRenderRequired();
		}
		
		#endregion
		
		#region GraphicsContext
		
		private void InitializeGraphicsContext(GraphicsContext graphicsContext)
		{
			GraphicsContext = graphicsContext;
			
			ScreenWidth = GraphicsContext.Screen.Rectangle.Width;
			ScreenHeight = GraphicsContext.Screen.Rectangle.Height;
		}
		
		private void CleanupGraphicsContext()
		{
			GraphicsContext = null;
			
			ScreenWidth = 0;
			ScreenHeight = 0;
		}
		
		internal GraphicsContext GraphicsContext { get; private set; }
		
		public Single ScreenWidth { get; private set; }
		public Single ScreenHeight { get; private set; }
		
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
				SetRenderRequired();
			}
		}
		
		#endregion
		
		#region Camera
		
		public Camera Camera { get; private set; }
		
		#endregion
		
		#region Layers
		
		private void InitializeLayers()
		{
			Layers = new SortedList<Int32, Layer>();
		}
		
		private void CleanupLayers()
		{
			//Int32[] layerKeys = new Int32[0];
			//Layers.Keys.CopyTo(layerKeys, 0);
			
			foreach(Int32 zIndex in layerKeys)
			{
				Layer layer = Layers[zIndex];
				RemoveLayer(zIndex);
				layer.Dispose();
			}
			Layers.Clear();
			Layers = null;
		}
		
		private SortedList<Int32, Layer> Layers { get; set; }
		
		public Layer CreateLayer(Int32 zIndex)
		{
			SetRenderRequired();
			
			Layer layer = new Layer();
			Layers.Add(zIndex, layer);
			return layer;
		}
		
		public void RemoveLayer(Int32 zIndex)
		{
			SetRenderRequired();
			
			Layers.Remove(zIndex);
		}
		
		public Layer GetLayer(Int32 zIndex)
		{
			return Layers[zIndex];
		}
		
		#endregion
		
		#region Render Required
		
		//TODO: Need a better name. dirty?
		
		private void InitializeRenderRequiredFlag()
		{
			//Ensure first past is rendered.
			RenderRequired = true;
		}
		
		private void CleanupRenderRequiredFlag()
		{
		}
		
		private Boolean RenderRequired;
		
		private void SetRenderRequired()
		{
			RenderRequired = true;
		}
		
		private void ResetRenderRequired()
		{
			RenderRequired = false;
		}
		
		#endregion
	}
}
