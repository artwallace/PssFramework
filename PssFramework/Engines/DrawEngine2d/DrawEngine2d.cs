using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d
{
	public sealed class DrawEngine2d : IDisposable
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
			InitializeTiledTextureManager();
			InitializeDebugRuler();
		}
		
		private void Cleanup()
		{
			CleanupDebugRuler();
			CleanupTiledTextureManager();
			CleanupRenderRequiredFlag();
			CleanupLayers();
			CleanupClearColor();
			CleanupGraphicsContext();
		}
		
		#endregion
		
		#region Update, Render
		
		public void Update()
		{
			//foreach(Layer layer in Layers.Values)
				//layer.Update();
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
		
		#region Clear Color
		
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
			Layer[] layers = new Layer[Layers.Values.Count];
			Layers.Values.CopyTo(layers, 0);
			
			foreach(Layer layer in layers)
				layer.Dispose();
			Layers.Clear();
			
			Layers = null;
		}
		
		private SortedList<Int32, Layer> Layers { get; set; }
		
		public Layer GetOrCreateLayer(Int32 zIndex)
		{
			if(Layers.ContainsKey(zIndex))
				return Layers[zIndex];
			else
				return new Layer(this, zIndex);
		}
		
		internal void AddLayer(Layer layer, Int32 zIndex)
		{
			if(layer == null)
				throw new ArgumentNullException();
			
			if(Layers.ContainsValue(layer))
				throw new ArgumentException("Duplicate layer added to DrawEngine2d.");
			
			SetRenderRequired();
			Layers.Add(zIndex, layer);
		}
		
		public void RemoveLayer(Layer layer)
		{
			if(layer == null)
				throw new ArgumentNullException();
			
			if(!Layers.ContainsValue(layer))
				throw new ArgumentException("Unknown layer removal requested from DrawEngine2d.");
			
			SetRenderRequired();
			Int32 valueLocation = Layers.IndexOfValue(layer);
			Int32 zIndex = Layers.Keys[valueLocation];
			Layers.Remove(zIndex);
		}
		
		public Boolean CheckIfLayerExists(Int32 zIndex)
		{
			return Layers.ContainsKey(zIndex);
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
		
		public void SetRenderRequired()
		{
			RenderRequired = true;
		}
		
		private void ResetRenderRequired()
		{
			RenderRequired = false;
		}
		
		#endregion
		
		#region Debug Ruler
		
		private void InitializeDebugRuler()
		{
//			EnableDebugRuler = false;
//			DebugRulerAxisColor = Colors.Black;
//			DebugRulerAxisThickness = 1.0f;
//			DebugRulerGridColor = Colors.Grey60;
//			DebugRulerGridThickness = 1.0f;
		}
		
		private void CleanupDebugRuler()
		{
		}
		
//		private Boolean _EnableDebugRuler;
//		public Boolean EnableDebugRuler
//		{
//			get { return _EnableDebugRuler; }
//			set
//			{
//				_EnableDebugRuler = value;
//				SetRenderRequired();
//			}
//		}
//		
//		private Color _DebugRulerAxisColor;
//		public Color DebugRulerAxisColor
//		{
//			get { return _DebugRulerAxisColor; }
//			set
//			{
//				_DebugRulerAxisColor = value;
//				SetRenderRequired();
//			}
//		}
//		
//		private Single DebugRulerAxisThickness;
//		
//		public Color _DebugRulerGridColor;
//		public Color DebugRulerGridColor
//		{
//			get { return _DebugRulerGridColor; }
//			set
//			{
//				_DebugRulerGridColor = value;
//				SetRenderRequired();
//			}
//		}
//		
//		private Single DebugRulerGridThickness;
//		
//		private void DrawDebugRulers()
//		{
//			//GraphicsContext.SetLineWidth(DebugRulerAxisThickness);
//			
//			//GraphicsContext.SetLineWidth(1.0f);
//		}
		
		#endregion
		
		#region TiledTexture Manager
		
		private void InitializeTiledTextureManager()
		{
			TiledTextureList = new List<TiledTexture>();
			TiledTextureUsers = new Dictionary<IDrawable, TiledTexture>();
		}
		
		private void CleanupTiledTextureManager()
		{
			TiledTextureUsers.Clear();
			TiledTextureUsers = null;
			
			foreach(TiledTexture t in TiledTextureList)
				t.Dispose();
			TiledTextureList.Clear();
			TiledTextureList = null;
		}
		
		private List<TiledTexture> TiledTextureList;
		
		private Dictionary<IDrawable, TiledTexture> TiledTextureUsers;
		
		public TiledTexture GetOrCreateTiledTexture(IDrawable user, String path, Int32 columns = 1, Int32 rows = 1)
		{
			return GetOrCreateTiledTextureHelper(user, path, columns, rows, RectangularArea2i.Zero);
		}
		
		public TiledTexture GetOrCreateTiledTexture(IDrawable user, String path, Int32 columns, Int32 rows, RectangularArea2i sourceArea)
		{
			return GetOrCreateTiledTextureHelper(user, path, columns, rows, sourceArea);
		}
		
		private TiledTexture GetOrCreateTiledTextureHelper(IDrawable user, String path, Int32 columns, Int32 rows, RectangularArea2i sourceArea)
		{
			if(user == null)
				throw new ArgumentNullException();
			
			if(String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			if(columns < 1 || rows < 1)
				throw new ArgumentException();
			
			foreach(TiledTexture t in TiledTextureList)
			{
				if(
					t.Path == path &&
					t.Columns == columns &&
					t.Rows == rows &&
					t.SourceArea == sourceArea
					)
				{
					RegisterTiledTextureUser(user, t);
					return t;
				}
			}
			
			TiledTexture tt = new TiledTexture(this, path, columns, rows, sourceArea);
			RegisterTiledTextureUser(user, tt);
			return tt;
		}
		
		public void RemoveTiledTexture(IDrawable user, TiledTexture tiledTexture)
		{
			if(user == null)
				throw new ArgumentNullException();
			
			if(tiledTexture == null)
				throw new ArgumentNullException();
			
			UnregisterTiledTextureUser(user, tiledTexture);
		}
		
		private void RegisterTiledTextureUser(IDrawable user, TiledTexture tiledTexture)
		{
			if(user == null)
				throw new ArgumentNullException();
			
			if(tiledTexture == null)
				throw new ArgumentNullException();
			
			//If user is unregistered, add is as a user of this texture.
			if(!TiledTextureUsers.ContainsKey(user))
			{
				TiledTextureUsers.Add(user, tiledTexture);
				return;
			}
			//If the user is already registered, ensure the path is what was requested.
			else if(TiledTextureUsers[user] == tiledTexture)
			{
				//Everything is OK, exit.
				return;
			}
			else
			{
				//User is registered with another texture, this is an error.
				throw new NotSupportedException();
			}
		}
		
		private void UnregisterTiledTextureUser(IDrawable user, TiledTexture tiledTexture)
		{
			throw new NotImplementedException();
		}
		
		#endregion
		
		#region Texture2D Manager
		
		private void InitializeTexture2DManager()
		{
			Texture2DList = new Dictionary<String,Texture2D>();
			Texture2DUsers = new Dictionary<TiledTexture, String>();
		}
		
		private void CleanupTexture2DManager()
		{
			Texture2DUsers.Clear();
			Texture2DUsers = null;
			
			foreach(Texture2D t in Texture2DList.Values)
				t.Dispose();
			Texture2DList.Clear();
			Texture2DList = null;
		}
		
		private Dictionary<String,Texture2D> Texture2DList;
		
		private Dictionary<TiledTexture, String> Texture2DUsers;
		
		internal Texture2D GetOrCreateTexture2D(TiledTexture user, String path)
		{
			if(user == null)
				throw new ArgumentNullException();
			
			if(String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			//Ensure this user is registered.
			RegisterTexture2DUser(user, path);
			
			//Check if texture already exists and return it.
			if(Texture2DList.ContainsKey(path))
			{
				return Texture2DList[path];
			}
			else
			{
				//Otherwise, create the new texture.
				Texture2D t = new Texture2D(path, false);
				Texture2DList.Add(path, t);
				return t;
			}
		}
		
		internal void RemoveTexture2D(TiledTexture user, String path)
		{
			if(user == null)
				throw new ArgumentNullException();
			
			if(String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			UnregisterTexture2DUser(user, path);
		}
		
		private void RegisterTexture2DUser(TiledTexture user, String path)
		{
			if(user == null)
				throw new ArgumentNullException();
			
			if(String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			//If user is unregistered, add is as a user of this texture.
			if(!Texture2DUsers.ContainsKey(user))
			{
				Texture2DUsers.Add(user, path);
				return;
			}
			//If the user is already registered, ensure the path is what was requested.
			else if(Texture2DUsers[user] == path)
			{
				//Everything is OK, exit.
				return;
			}
			else
			{
				//User is registered with another texture, this is an error.
				throw new NotSupportedException();
			}
		}
		
		private void UnregisterTexture2DUser(TiledTexture user, String path)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}
