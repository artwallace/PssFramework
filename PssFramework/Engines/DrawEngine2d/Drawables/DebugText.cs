using System;
using PsmFramework.Engines.DrawEngine2d.Shaders;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public class DebugText : DrawableBase
	{
		#region Constructor, Dispose
		
		public DebugText(Layer layer)
			: base(layer)
		{
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected override void Initialize()
		{
			InitializeRenderingCache();
		}
		
		protected override void Cleanup()
		{
			CleanupRenderingCache();
		}
		
		#endregion
		
		#region Render
		
		public override void Render()
		{
			if(RenderingRecacheRequired)
				GenerateCachedRendering();
			
			DrawEngine2d.FontShader.SetVertexBuffer();
			DrawEngine2d.FontShader.SetShaderProgram();
			
			Texture2D t = new Texture2D("/Application/TwinStickShooter/Images/Ship64.png", false);
			DrawEngine2d.GraphicsContext.SetTexture(0, t);
			//if (DrawEngine2d.GraphicsContext.GetTexture(0) != DrawEngine2d.DebugFont.Texture)
			//DrawEngine2d.GraphicsContext.SetTexture(0, DrawEngine2d.DebugFont.Texture);
			
			foreach(RenderingCacheData cacheData in CachedRendering)
			{
				Vector3 scaleVector = new Vector3(DebugFont.FontWidth, DebugFont.FontHeight, 1.0f);
				Matrix4 scaleMatrix = Matrix4.Scale(scaleVector);
				Vector3 translationVector = new Vector3(cacheData.Position.X, cacheData.Position.Y, 1.0f);
				Matrix4 transMatrix = Matrix4.Translation(translationVector);
				Matrix4 modelMatrix = transMatrix * scaleMatrix;
				Matrix4 worldViewProj = Layer.DrawEngine2d.ProjectionMatrix * Layer.DrawEngine2d.ModelViewMatrix;// * modelMatrix;
				
				DrawEngine2d.FontShader.SetWorldViewProjection(ref worldViewProj);
				
				//TODO: this needs to be changed to be an array of VBOs, like ge2d.
				DrawEngine2d.FontShader.DrawArrays();
			}
			
			for(Int32 c = 0; c < CachedRendering.Length; c++)
			{
				//Matrix4 scaleMatrix = Matrix4.Scale(new Vector3(DebugFont.FontWidth, DebugFont.FontHeight, 1.0f));
				//Matrix4 transMatrix = Matrix4.Translation(new Vector3(CachedRendering[c].Position.X, CachedRendering[c].Position.Y, 0.0f));
				//Matrix4 modelMatrix = transMatrix * scaleMatrix;
				Matrix4 worldViewProj = DrawEngine2d.ProjectionMatrix * DrawEngine2d.ModelViewMatrix;// * modelMatrix;
				
				DrawEngine2d.FontShader.SetWorldViewProjection(ref worldViewProj);
				
				//TODO: this needs to be changed to be an array of VBOs, like ge2d.
				DrawEngine2d.FontShader.DrawArrays();
			}
		}
		
		#endregion
		
		#region MarkAsChanged
		
		private void MarkAsChanged()
		{
			RenderingRecacheRequired = true;
			DrawEngine2d.SetRenderRequired();
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
				CachedRendering[cacheIndex].CharCode = c;
				CachedRendering[cacheIndex].Position.X = Position.X + (DebugFont.FontWidth * charOnThisLineNumber);
				CachedRendering[cacheIndex].Position.Y = Position.Y + (DebugFont.FontHeight * lineNumber);
				
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
		
		#region Text
		
		private String _Text;
		public String Text
		{
			get { return _Text; }
			set
			{
				if (_Text == value)
					return;
				
				MarkAsChanged();
				
				_Text = Clean(value);
			}
		}
		
		private static String Clean(String text)
		{
			return text.Trim();
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
				
				MarkAsChanged();
				
				_Position = value;
			}
		}
		
		#endregion
		
		#region AdHocDraw
		
		//TODO: Impliment AdHocDraw.
		
		#endregion
	}
}

