using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core;
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
			InitializeGraphics();
			InitializeClearColor();
			InitializeCamera();
			InitializeLayers();
			InitializeRenderRequiredFlag();
			InitializeShaders();
			InitializeTexture2DManager();
			InitializeTiledTextureManager();
			InitializeDebugRuler();
			InitializeDebugFont();
			InitializePerformanceTracking();
		}
		
		private void Cleanup()
		{
			CleanupPerformanceTracking();
			CleanupDebugFont();
			CleanupDebugRuler();
			CleanupTiledTextureManager();
			CleanupTexture2DManager();
			CleanupShaders();
			CleanupRenderRequiredFlag();
			CleanupLayers();
			CleanupCamera();
			CleanupClearColor();
			CleanupGraphics();
			CleanupGraphicsContext();
		}
		
		#endregion
		
		#region Update, Render
		
		//TODO: Does DrawEngine2d really need an Update method?
		public void Update()
		{
			//foreach(Layer layer in Layers.Values)
				//layer.Update();
		}
		
		public void Render()
		{
			if(!RenderRequired)
				return;
			ResetRenderRequired();
			
			GraphicsContext.Clear();
			
			foreach(Layer layer in Layers.Values)
				layer.Render();
			
			GraphicsContext.SwapBuffers();
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
		
		//TODO: Make this private with wrappers for common functions.
		//Prevent drawables from doing crazy stuff.
		internal GraphicsContext GraphicsContext;
		
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
		
		public TiledTexture GetOrCreateTiledTexture(String path, Int32 columns = 1, Int32 rows = 1)
		{
			return GetOrCreateTiledTextureHelper(path, columns, rows, RectangularArea2i.Zero);
		}
		
		public TiledTexture GetOrCreateTiledTexture(String path, Int32 columns, Int32 rows, RectangularArea2i sourceArea)
		{
			return GetOrCreateTiledTextureHelper(path, columns, rows, sourceArea);
		}
		
		private TiledTexture GetOrCreateTiledTextureHelper(String path, Int32 columns, Int32 rows, RectangularArea2i sourceArea)
		{
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
					return t;
				}
			}
			
			TiledTexture tt = new TiledTexture(this, path, columns, rows, sourceArea);
			return tt;
		}
		
		internal void RegisterTiledTextureUser(IDrawable user, TiledTexture tiledTexture)
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
		
		internal void UnregisterTiledTextureUser(IDrawable user, TiledTexture tiledTexture)
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
		
		#region Camera
		
		private void InitializeCamera()
		{
			CameraPosition = Coordinate2.X0Y0;
			CameraZoom = 1.0f;
			CameraRotation = 0.0f;
		}
		
		private void CleanupCamera()
		{
			CameraPosition = Coordinate2.X0Y0;
			CameraZoom = 1.0f;
			CameraRotation = 0.0f;
		}
		
		private Coordinate2 CameraPosition;
		
		public void SetCameraPosition()
		{
		}
		
		//Switch to an enum instead of separate methods?
		public void SetCameraPositionFromBottomLeft()
		{
		}
		
		public void SetCameraPositionFromTopLeft()
		{
		}
		
		private Single CameraZoom;
		
		private Single CameraRotation;
		
		#endregion
		
		#region OpenGL Graphics
		
		//http://en.wikibooks.org/wiki/OpenGL_Programming/Modern_OpenGL_Tutorial_2D
		
		//Most OpenGL programs tend to use a perspective projection matrix
		// to transform the model-space coordinates of a cartesian model into
		// the "view coordinate" space of the screen.
		
		//One of the most common matrices used for orthographic projection
		// can be defined by a 6-tuple, (left, right, bottom, top, near, far),
		// which defines the clipping planes. These planes form a box with
		// the minimum corner at (left, bottom, near) and the maximum
		// corner at (right, top, far).
		//The box is translated so that its center is at the origin, then it is
		// scaled to the unit cube which is defined by having a minimum corner
		// at (-1,-1,-1) and a maximum corner at (1,1,1).
		
		//OpenGL has a special rule to draw fragments at the center of screen pixels,
		// called "diamond rule" [2] [3]. Consequently, it is recommended to add a
		// small translation in X,Y before drawing 2D sprite.
		// glm::translate(glm::mat4(1), glm::vec3(0.375, 0.375, 0.));
		//or
		// glMatrixMode (GL_MODELVIEW);
		// glLoadIdentity ();
		// glTranslatef (0.375, 0.375, 0.);
		
		//http://www.opengl.org/archives/resources/faq/technical/transformations.htm#tran0030
		
		//OpenGL works in the following way: You start of a local coordinate system
		// (of arbitrary units). This coordinate system is transformed to so called
		// eye space coordinates by the modelview matrix (it is called modelview
		// matrix, because it combinnes model and view transformations).
		//The eye space is then transformed to clip space by the projection matrix,
		// immediately followed by the perspective divide to obtain normalized
		// device coordinates ( NDC{x,y,z} = Clip{x,y,z}/Clip_w ).
		// The range [-1,1]^3 in NDC space is mapped to the viewport (x and y)
		// and the set depth range (z).
		//So if you leave your transformation matrices (modelview and projection)
		// identity, then indeed the coordinate ranges [-1,1] will map to the viewport.
		// However by choosing apropriate transformation and projection you can map
		// from modelspace units to viewport units arbitrarily.
		
		//Essentially, a projection matrix is matrix that projects a vertex on a 2D space.
		
		private void InitializeGraphics()
		{
			FrameBufferWidth = GraphicsContext.GetFrameBuffer().Width;
			FrameBufferWidthAsSingle = (Single)FrameBufferWidth;
			FrameBufferHeight = GraphicsContext.GetFrameBuffer().Height;
			FrameBufferHeightAsSingle = (Single)FrameBufferHeight;
			
			//TODO: Is this one pixel too tall and wide?
			ProjectionMatrixLeft = 0.0f;
			ProjectionMatrixRight = FrameBufferWidthAsSingle;
			ProjectionMatrixBottom = 0.0f;
			ProjectionMatrixTop = FrameBufferHeightAsSingle;
			ProjectionMatrixNear = -1.0f;
			ProjectionMatrixFar = 1.0f;
			
			ViewMatrixEye = new Vector3(0.0f, FrameBufferHeightAsSingle, 0.0f);
			ViewMatrixCenter = new Vector3(0.0f, FrameBufferHeightAsSingle, 1.0f);
			ViewMatrixUp = new Vector3(0.0f, -1.0f, 0.0f);
			
			ProjectionMatrix = Matrix4.Ortho(
				ProjectionMatrixLeft,
				ProjectionMatrixRight,
				ProjectionMatrixBottom,
				ProjectionMatrixTop,
				ProjectionMatrixNear,
				ProjectionMatrixFar
				);
			
			ModelViewMatrix = Matrix4.LookAt(
				ViewMatrixEye,
				ViewMatrixCenter,
				ViewMatrixUp
				);
		}
		
		private void CleanupGraphics()
		{
		}
		
		public Matrix4 ProjectionMatrix { get; private set; }
		public Matrix4 ModelViewMatrix { get; private set; }
		
		private Int32 FrameBufferWidth;
		private Int32 FrameBufferHeight;
		
		private Single FrameBufferWidthAsSingle;
		private Single FrameBufferHeightAsSingle;
		
		private Single ProjectionMatrixLeft;
		private Single ProjectionMatrixRight;
		private Single ProjectionMatrixBottom;
		private Single ProjectionMatrixTop;
		private Single ProjectionMatrixNear;
		private Single ProjectionMatrixFar;
		
		private Vector3 ViewMatrixEye;
		private Vector3 ViewMatrixCenter;
		private Vector3 ViewMatrixUp;
		
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
		
		#region Debug Font
		
		private void InitializeDebugFont()
		{
			DebugFont = new DebugFont();
		}
		
		private void CleanupDebugFont()
		{
			DebugFont.Dispose();
			DebugFont = null;
		}
		
		internal DebugFont DebugFont { get; private set; }
		
		#endregion
		
		#region Shaders
		
		private void InitializeShaders()
		{
		}
		
		private void CleanupShaders()
		{
		}
		
		
		
		#endregion
		
		#region Performance Tracking
		
		private void InitializePerformanceTracking()
		{
			ResetDrawArrayCallsCounter();
		}
		
		private void CleanupPerformanceTracking()
		{
		}
		
		public Int32 DrawArrayCallsCounter;
		
		public void ResetDrawArrayCallsCounter()
		{
			DrawArrayCallsCounter = 0;
		}
		
		public void IncrementDrawArrayCallsCounter()
		{
			DrawArrayCallsCounter++;
		}
		
		public Int32 GetDrawArrayCallsCount()
		{
			return DrawArrayCallsCounter;
		}
		
		#endregion
	}
}
