using System;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class DebugText : IDrawable
	{
		#region Constructor, Dispose
		
		public DebugText(Layer layer)
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
			InitializeShader();
			InitializeRenderingCache();
		}
		
		private void Cleanup()
		{
			CleanupRenderingCache();
			CleanupShader();
			CleanupLayer();
		}
		
		#endregion
		
		#region Render
		
		public void Render()
		{
			if(RenderingRecacheRequired)
				GenerateCachedRendering();
			
			FontShader.SetShaderProgram();
			FontShader.SetVertexBuffer();
			
			if (Layer.DrawEngine2d.GraphicsContext.GetTexture(0) != Layer.DrawEngine2d.DebugFont.Texture)
				Layer.DrawEngine2d.GraphicsContext.SetTexture(0, Layer.DrawEngine2d.DebugFont.Texture);
			
			for(Int32 c = 0; c < CachedRendering.Length; c++)
			{
				Matrix4 scaleMatrix = Matrix4.Scale(new Vector3(DebugFont.FontWidth, DebugFont.FontHeight, 1.0f));
				Matrix4 transMatrix = Matrix4.Translation(new Vector3(CachedRendering[c].Position.X, CachedRendering[c].Position.Y, 1.0f));
				Matrix4 modelMatrix = transMatrix * scaleMatrix;
				Matrix4 worldViewProj = Layer.DrawEngine2d.ProjectionMatrix * Layer.DrawEngine2d.ModelViewMatrix * modelMatrix;
				
				FontShader.ShaderProgram.SetUniformValue(0, ref worldViewProj);
				
				//TODO: this needs to be changed to be an array of VBOs, like ge2d.
				FontShader.DrawArrays();
			}
		}
		
		#endregion
		
		#region Rendering Cache
		
		private void InitializeRenderingCache()
		{
			RenderingRecacheRequired = true;
		}
		
		private void CleanupRenderingCache()
		{
			CachedRendering = new RenderingCacheData[0];
		}
		
		private void GenerateCachedRendering()
		{
			RenderingRecacheRequired = false;
			
			if(String.IsNullOrWhiteSpace(Text))
			{
				CachedRendering = new RenderingCacheData[0];
				return;
			}
			
			Int32 charCount = 0;
			
			foreach(Char c in Text)
			{
				if(c == '\n' || c == '\r')
					continue;
				else
					charCount++;
			}
			
			CachedRendering = new RenderingCacheData[charCount];
			
			Int32 cacheIndex = 0;
			Int32 lineNumber = 0;
			Int32 charOnThisLineNumber = 0;
			
			foreach(Char c in Text)
			{
				if(c == '\n')
				{
					lineNumber++;
					charOnThisLineNumber = 0;
					continue;
				}
				else if(c == '\r')
					continue;
				
				//TODO: Needs spacing added.
				//TODO: Add support for opposite Coordinate Mode here.
				CachedRendering[cacheIndex].Position.X = Position.X + (DebugFont.FontWidth * charOnThisLineNumber);
				CachedRendering[cacheIndex].Position.Y = Position.Y + (DebugFont.FontHeight * lineNumber);
				CachedRendering[cacheIndex].CharCode = c;
				
				//Final things to do.
				cacheIndex++;
				charOnThisLineNumber++;
			}
		}
		
		private Boolean RenderingRecacheRequired;
		
		private RenderingCacheData[] CachedRendering;
		
		private struct RenderingCacheData
		{
			public Coordinate2 Position;
			public Int32 CharCode;
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
		
		#region Text
		
		private String _Text;
		public String Text
		{
			get { return _Text; }
			set
			{
				if (_Text == value)
					return;
				
				Layer.DrawEngine2d.SetRenderRequired();
				
				//TODO: Should the text get cleaned here?
				//Strip whitespace, \r, etc?
				_Text = value.Trim();
			}
		}
		
		#endregion
		
		#region Position
		
		private Coordinate2 _Position;
		public Coordinate2 Position
		{
			get { return _Position; }
			set
			{
				if (_Position == value)
					return;
				
				Layer.DrawEngine2d.SetRenderRequired();
				
				_Position = value;
			}
		}
		
		#endregion
		
		#region Shader Program
		
		private void InitializeShader()
		{
			//TODO: Should share shader, cache in DE2d.
			FontShader = new FontShader(Layer.DrawEngine2d);
		}
		
		private void CleanupShader()
		{
			FontShader.Dispose();
			FontShader = null;
		}
		
		private FontShader FontShader;
		
		#endregion
		
		#region AdHocDraw
		
		//TODO: Impliment AdHocDraw.
		
		#endregion
	}
}

