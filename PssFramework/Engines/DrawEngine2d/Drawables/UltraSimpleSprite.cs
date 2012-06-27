using System;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class UltraSimpleSprite : IDrawable, IDisposable
	{
		#region Constructor, Dispose
		
		public UltraSimpleSprite(Layer layer)
		{
			Initialize(layer);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(Layer layer)
		{
			InitializeLayer(layer);
			
			InitializeVertices();
			InitializeIndices();
			InitializeTextureCoordinates();
			InitializeColor();
			
			InitializeTexture();
			InitializeShaderProgram();
			InitializeVertexBuffer();
			InitializeScreenMatrix();
		}
		
		private void Cleanup()
		{
			CleanupScreenMatrix();
			CleanupVertexBuffer();
			CleanupShaderProgram();
			CleanupTexture();
			
			CleanupColor();
			CleanupTextureCoordinates();
			CleanupIndices();
			CleanupVertices();
			
			CleanupLayer();
		}
		
		#endregion
		
		#region Render
		
		public void Render()
		{
			Layer.DrawEngine2d.GraphicsContext.SetVertexBuffer(0, VertexBuffer);
			
			Layer.DrawEngine2d.GraphicsContext.SetShaderProgram(ShaderProgram);
			Layer.DrawEngine2d.GraphicsContext.SetTexture(0, Texture);
			ShaderProgram.SetUniformValue(0, ref UnitScreenMatrix);
			
			Layer.DrawEngine2d.GraphicsContext.DrawArrays(DrawMode.TriangleStrip, 0, IndexCount);
		}
		
		#endregion
		
		#region Layer
		
		private void InitializeLayer(Layer layer)
		{
			Layer = layer;
			Layer.AddDrawable(this);
		}
		
		private void CleanupLayer()
		{
			Layer.RemoveDrawable(this);
			Layer = null;
		}
		
		protected Layer Layer;
		
		#endregion
		
		#region Vertex Rendering Order Indices
		
		private void InitializeIndices()
		{
			Indices = new UInt16[4];
			Indices[0] = 0;
			Indices[1] = 1;
			Indices[2] = 2;
			Indices[3] = 3;
		}
		
		private void CleanupIndices()
		{
			Indices = new UInt16[0];
		}
		
		private const Int32 IndexCount = 4;
		private UInt16[] Indices;
		
		#endregion
		
		#region Vertex Coordinates
		
		private void InitializeVertices()
		{
			Vertices = new Single[VertexCount * 3];
			
			//TODO: these are temporary, for testing.
			VertexCoordinates_0_TopLeft = new Vector2(0.0f, 0.0f);
			VertexCoordinates_1_BottomLeft = new Vector2(0.0f, 1.0f);
			VertexCoordinates_2_TopRight = new Vector2(1.0f, 0.0f);
			VertexCoordinates_3_BottomRight = new Vector2(1.0f, 1.0f);
		}
		
		private void CleanupVertices()
		{
			Vertices = new Single[0];
		}
		
		private Single[] Vertices;
		
		private const Int32 VertexCount = 4;
		
		private const Single VertexZ = 0.0f;
		
		private Vector2 VertexCoordinates_0_TopLeft
		{
			get
			{
				return new Vector2(Vertices[0], Vertices[1]);
			}
			set
			{
				Vertices[0] = value.X;
				Vertices[1] = value.Y;
				Vertices[2] = VertexZ;
			}
		}
		
		private Vector2 VertexCoordinates_1_BottomLeft
		{
			get
			{
				return new Vector2(Vertices[3], Vertices[4]);
			}
			set
			{
				Vertices[3] = value.X;
				Vertices[4] = value.Y;
				Vertices[5] = VertexZ;
			}
		}
		
		private Vector2 VertexCoordinates_2_TopRight
		{
			get
			{
				return new Vector2(Vertices[6], Vertices[7]);
			}
			set
			{
				Vertices[6] = value.X;
				Vertices[7] = value.Y;
				Vertices[8] = VertexZ;
			}
		}
		
		private Vector2 VertexCoordinates_3_BottomRight
		{
			get
			{
				return new Vector2(Vertices[9], Vertices[10]);
			}
			set
			{
				Vertices[9] = value.X;
				Vertices[10] = value.Y;
				Vertices[11] = VertexZ;
			}
		}
		
		#endregion
		
		#region Texture Coordinates
		
		private void InitializeTextureCoordinates()
		{
			TextureCoordinates = new Single[4 * 2];
			
			//TODO: these are temporary, for testing.
			TextureCoordinates_0_TopLeft = new Vector2(0.0f, 0.0f);
			TextureCoordinates_1_BottomLeft = new Vector2(0.0f, 1.0f);
			TextureCoordinates_2_TopRight = new Vector2(1.0f, 0.0f);
			TextureCoordinates_3_BottomRight = new Vector2(1.0f, 1.0f);
		}
		
		private void CleanupTextureCoordinates()
		{
			TextureCoordinates = new Single[0];
		}
		
		private Single[] TextureCoordinates;
		
		private Vector2 TextureCoordinates_0_TopLeft
		{
			get
			{
				return new Vector2(TextureCoordinates[0], TextureCoordinates[1]);
			}
			set
			{
				TextureCoordinates[0] = value.X;
				TextureCoordinates[1] = value.Y;
			}
		}
		private Vector2 TextureCoordinates_1_BottomLeft
		{
			get
			{
				return new Vector2(TextureCoordinates[2], TextureCoordinates[3]);
			}
			set
			{
				TextureCoordinates[2] = value.X;
				TextureCoordinates[3] = value.Y;
			}
		}
		private Vector2 TextureCoordinates_2_TopRight
		{
			get
			{
				return new Vector2(TextureCoordinates[4], TextureCoordinates[5]);
			}
			set
			{
				TextureCoordinates[4] = value.X;
				TextureCoordinates[5] = value.Y;
			}
		}
		private Vector2 TextureCoordinates_3_BottomRight
		{
			get
			{
				return new Vector2(TextureCoordinates[6], TextureCoordinates[7]);
			}
			set
			{
				TextureCoordinates[6] = value.X;
				TextureCoordinates[7] = value.Y;
			}
		}
		
		#endregion
		
		#region Color
		
		private void InitializeColor()
		{
			VertexColors = new Single[VertexCount * 4];
			Color = Colors.White;
		}
		
		private void CleanupColor()
		{
			VertexColors = new Single[0];
		}
		
		private Single[] VertexColors;
		
		private Color _Color;
		private Color Color
		{
			get { return _Color; }
			set
			{
				_Color = value;
				
				VertexColors[0] = _Color.R;
				VertexColors[1] = _Color.G;
				VertexColors[2] = _Color.B;
				VertexColors[3] = _Color.A;
				
				VertexColors[4] = _Color.R;
				VertexColors[5] = _Color.G;
				VertexColors[6] = _Color.B;
				VertexColors[7] = _Color.A;
				
				VertexColors[8] = _Color.R;
				VertexColors[9] = _Color.G;
				VertexColors[10] = _Color.B;
				VertexColors[11] = _Color.A;
				
				VertexColors[12] = _Color.R;
				VertexColors[13] = _Color.G;
				VertexColors[14] = _Color.B;
				VertexColors[15] = _Color.A;
			}
		}
		
		#endregion
		
		#region Vertex Buffer
		
		private void InitializeVertexBuffer()
		{
			VertexBuffer = new VertexBuffer(VertexCount, IndexCount, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);
			
			VertexBuffer.SetVertices(0, Vertices);
			VertexBuffer.SetVertices(1, TextureCoordinates);
			VertexBuffer.SetVertices(2, VertexColors);
			VertexBuffer.SetIndices(Indices);
		}
		
		private void CleanupVertexBuffer()
		{
			VertexBuffer.Dispose();
			VertexBuffer = null;
		}
		
		private VertexBuffer VertexBuffer;
		
		#endregion
		
		#region Shader Program
		
		private void InitializeShaderProgram()
		{
			ShaderProgram = ShaderLoader.Load(ShaderPaths.UltraSimpleSprite);
			ShaderProgram.SetUniformBinding(ShaderBindingIndex, ShaderBindingName);
		}
		
		private void CleanupShaderProgram()
		{
			ShaderProgram.Dispose();
			ShaderProgram = null;
		}
		
		private ShaderProgram ShaderProgram;
		
		private const Int32 ShaderBindingIndex = 0;
		private const String ShaderBindingName = "u_WorldMatrix";
		
		#endregion
		
		#region Texture
		
		//TODO: Most of this will be moved somewhere else after testing.
		
		private void InitializeTexture()//String path
		{
			TexturePath = "/Application/TwinStickShooter/Images/Ship64.png";//path;
			
			Texture = new Texture2D(TexturePath, false);
			
			TextureWidth = Texture.Width;
			TextureHeight = Texture.Height;
		}
		
		private void CleanupTexture()
		{
			TexturePath = String.Empty;
			TextureWidth = 0;
			TextureHeight = 0;
			Texture.Dispose();//TODO: When textures are shared, this will be BAD!
			Texture = null;
		}
		
		private Texture2D Texture;
		
		private String TexturePath;
		
		private Single TextureWidth;
		private Single TextureHeight;
		
		#endregion
		
		#region Screen Matrix
		
		private void InitializeScreenMatrix()
		{
			//TODO: I have no idea what these values represent.
			//TODO: What makes "* 2.0f" necessary?
			
			UnitScreenMatrixX = new Vector4(
				TextureWidth * 2.0f / Layer.DrawEngine2d.ScreenWidth,
				0.0f,
				0.0f,
				0.0f
				);
			
			UnitScreenMatrixY = new Vector4(
				0.0f,
				TextureHeight * (-2.0f) / Layer.DrawEngine2d.ScreenHeight,
				0.0f,
				0.0f
				);
			
			UnitScreenMatrixZ = new Vector4(
				0.0f,
				0.0f,
				1.0f,
				0.0f
				);
			
			UnitScreenMatrixW = new Vector4(
				-1.0f,
				1.0f,
				0.0f,
				1.0f
				);
			
			UnitScreenMatrix = new Matrix4(
				UnitScreenMatrixX,
				UnitScreenMatrixY,
				UnitScreenMatrixZ,
				UnitScreenMatrixW
				);
		}
		
		private void CleanupScreenMatrix()
		{
		}
		
		private Matrix4 UnitScreenMatrix;
		
		private Vector4 UnitScreenMatrixX;
		private Vector4 UnitScreenMatrixY;
		private Vector4 UnitScreenMatrixZ;
		private Vector4 UnitScreenMatrixW;
		
		#endregion
	}
}

