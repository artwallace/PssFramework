/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.Pss.Core;
using Sce.Pss.Core.Graphics; // BlendMode
using Sce.Pss.Core.Imaging;	// Font

namespace Sce.Pss.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// BlendMode wraps the blend state (BlendFunc+'enabled' bool) and provides some human friendly blend mode names.
	/// </summary>
	public struct BlendMode 
	{
		/// <summary>Blend enabled flag.</summary>
		public bool Enabled;
		/// <summary>Blend function.</summary>
		public BlendFunc BlendFunc;
		/// <summary></summary>
		public BlendMode( bool enabled, BlendFunc blend_func )
		{
			Enabled = enabled;
			BlendFunc = blend_func;
		}
		/// <summary>No alpha blend: dst = src</summary>
		public static BlendMode None = new BlendMode( false, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.One ) );
		/// <summary>Normal alpha blend: dst = lerp( dst, src, src.a )</summary>
		public static BlendMode Normal = new BlendMode( true, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) );
		/// <summary>Additive alpha blend: dst = dst + src</summary>
		public static BlendMode Additive = new BlendMode( true, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.One ) );
		/// <summary>Multiplicative alpha blend: dst = dst * src</summary>
		public static BlendMode Multiplicative = new BlendMode( true, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.DstColor, BlendFuncFactor.Zero ) );
		/// <summary>Premultiplied alpha blend: dst = dst * (1-src.a ) + src</summary>
		public static BlendMode PremultipliedAlpha = new BlendMode( true, new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.One, BlendFuncFactor.OneMinusSrcAlpha ) );
	}

	/// <summary>
	/// Augment Sce.Pss.Core.Graphics.GraphicsContext with a matrix stack and a couple of other functions.
	/// </summary>
	public class GraphicsContextAlpha : System.IDisposable
	{
		Sce.Pss.Core.Graphics.GraphicsContext m_context;
		bool m_context_must_be_disposed; // if true, m_context is disposed of in this class
		Texture2D m_white_texture;
		TextureInfo m_white_texture_info;

		/// <summary>
		/// The core graphics context.
		/// </summary>
		public Sce.Pss.Core.Graphics.GraphicsContext Context { get { return m_context; } }

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		/// <summary>GraphicsContextAlpha constructor.</summary>
		public GraphicsContextAlpha( Sce.Pss.Core.Graphics.GraphicsContext context = null )
		{
			m_context = context;
			m_context_must_be_disposed = false;

			if ( m_context == null )
			{
				m_context = new Sce.Pss.Core.Graphics.GraphicsContext();
				m_context_must_be_disposed = true; // this class takes ownership of m_context
			}

			ModelMatrix = new MatrixStack(16);
			ViewMatrix = new MatrixStack(16);
			ProjectionMatrix = new MatrixStack(8);
			m_white_texture = CreateTextureUnicolor( 0xffffffff );
			m_white_texture_info = new TextureInfo( m_white_texture );
			DebugStats = new DebugStats_();
		}

		/// <summary>
		/// Dispose implementation.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				if ( m_context_must_be_disposed )
					Common.DisposeAndNullify< Sce.Pss.Core.Graphics.GraphicsContext >( ref m_context );
				Common.DisposeAndNullify< Texture2D >( ref m_white_texture );
				m_disposed = true;
			}
		}

		/// <summary>
		/// The model matrix stack, similar to OpenGL.
		/// </summary>
		public MatrixStack ModelMatrix;

		/// <summary>
		/// The view matrix stack, similar to OpenGL.
		/// </summary>
		public MatrixStack ViewMatrix;

		/// <summary>
		/// The projection matrix stack, similar to OpenGL.
		/// </summary>
		public MatrixStack ProjectionMatrix;

		/// <summary>
		/// DebugStats at the moment only stores a DrawArrays calls counter.
		/// </summary>
		public class DebugStats_
		{
			uint m_current_frame = 0;

			/// <summary>
			/// Number of times DrawArrays got called by the GameEngine2D library (not including your own calls to that function). 
			/// Since DrawArrays is a costly function, this counter is a useful profiling information.
			/// The counter gets reset every frame.
			/// </summary>
			public uint DrawArraysCount = 0;

			public void OnDrawArray()
			{
				if ( m_current_frame != Common.FrameCount )
				{
					m_current_frame = Common.FrameCount;
					DrawArraysCount = 0;
				}
				++DrawArraysCount;
			}
		}

		/// <summary>
		/// DebugStats for simple profiling.
		/// </summary>
		public DebugStats_ DebugStats;
		
		// Return a small white texture with all pixels set to 0xffffffff.
		// Used as default in some shaders.
//		public Texture2D WhiteTexture 
//		{ 
//			get { return m_white_texture; }
//		}

		/// <summary>
		/// Return a small white texture with all pixels set to 0xffffffff, as a TextureInfo
		/// Used as default in some shaders.
		/// </summary>
		public TextureInfo WhiteTextureInfo
		{ 
			get { return m_white_texture_info; }
		}

		/// <summary>
		/// GetMVP() is a shortcut for ProjectionMatrix.Get() * ViewMatrix.Get() * ModelMatrix.Get().
		/// </summary>
		public Matrix4 GetMVP()
		{
			return ProjectionMatrix.Get() * ViewMatrix.Get() * ModelMatrix.Get();
		}

		/// <summary>
		/// GetAspect() is a shortcut that returns the viewport's aspect ratio (width/height).
		/// </summary>
		public float GetAspect()
		{
			ImageRect r = Context.GetViewport();
			Common.Assert( ( r.Width !=0 ) && ( r.Height != 0 ) );
			return(float)r.Width/(float)r.Height;
		}

		/// <summary>
		/// This function returns the viewport as a Bounds2 object.
		/// </summary>
		public Bounds2 GetViewportf()
		{
			ImageRect r = Context.GetViewport();
			return new Bounds2( new Vector2( r.X, r.Y ) ,
								new Vector2( r.X + r.Width ,
											 r.Y + r.Height ) );
		}

		/// <summary>
		/// Set the depth write mask.
		/// </summary>
		public void SetDepthMask( bool write )
		{
			DepthFunc func = Context.GetDepthFunc();
			func.WriteMask = write;
			Context.SetDepthFunc( func );
		}

		/// <summary>
		/// Set the blend mode.
		/// </summary>
		public void SetBlendMode( BlendMode mode )
		{
			if ( !mode.Enabled ) Context.Disable( EnableMode.Blend );
			else
			{
				Context.Enable( EnableMode.Blend );
				Context.SetBlendFunc( mode.BlendFunc );
			}
		}

		/// <summary>
		/// Create a small texture where all pixels have the same color.
		/// </summary>
		public static Texture2D CreateTextureUnicolor( uint color )
		{
			int w = 16;
			int h = 16;
			uint[] data = new uint[ w * h ];
			for ( int i=0; i < data.Length; ++i )
				data[i] = color;
			var texture = new Texture2D( w, h, false, PixelFormat.Rgba );
			texture.SetPixels( 0, data );
			return texture;
		}

		/// <summary>
		/// Given a Font object and a text, create a texture representing text.
		/// </summary>
		public static Texture2D CreateTextureFromFont( string text, Font font, uint color = 0xffffffff )
		{
			int width = font.GetTextWidth( text, 0, text.Length );
			int height = font.Metrics.Height;

			var image = new Image(
				ImageMode.Rgba,
				new ImageSize( width, height ),
				new ImageColor(0,0,0,0)
				);

			image.DrawText(
				text,
				new ImageColor(
					(int)( ( color >> 16 ) & 0xff ),
					(int)( ( color >> 8 ) & 0xff ),
					(int)( ( color >> 0 ) & 0xff ),
					(int)( ( color >> 24 ) & 0xff )
					),
				font,
				new ImagePosition(0,0)
				);

			var texture = new Texture2D( width, height, false, PixelFormat.Rgba );
			texture.SetPixels( 0, image.ToBuffer() );
			image.Dispose();

			return texture;
		}
	}

	/// <summary>
	/// Some named colored constants.
	/// </summary>
	public static class Colors
	{
		/// <summary>0,0,0,1</summary>
		public static Vector4 Black = new Vector4(0,0,0,1);
		/// <summary>1,0,0,1</summary>
		public static Vector4 Red = new Vector4(1,0,0,1);
		/// <summary>0,1,0,1</summary>
		public static Vector4 Green = new Vector4(0,1,0,1);
		/// <summary>1,1,0,1</summary>
		public static Vector4 Yellow = new Vector4(1,1,0,1);
		/// <summary>0,0,1,1</summary>
		public static Vector4 Blue = new Vector4(0,0,1,1);
		/// <summary>1,0,1,1</summary>
		public static Vector4 Magenta = new Vector4(1,0,1,1);
		/// <summary>0,1,1,1</summary>
		public static Vector4 Cyan = new Vector4(0,1,1,1);
		/// <summary>1,1,1,1</summary>
		public static Vector4 White = new Vector4(1,1,1,1);

		/// <summary>0.5,1,0,1</summary>
		public static Vector4 Lime = new Vector4(0.5f,1.0f,0.0f,1.0f);
		/// <summary>0,0.5,1,1</summary>
		public static Vector4 LightBlue = new Vector4(0.0f,0.5f,1.0f,1.0f);
		/// <summary>1,0,0.5,1</summary>
		public static Vector4 Pink = new Vector4(1.0f,0.0f,0.5f,1.0f);
		/// <summary>1,0.5,0,1</summary>
		public static Vector4 Orange = new Vector4(1.0f,0.5f,0.0f,1.0f);
		/// <summary>0,1,0.5,1</summary>
		public static Vector4 LightCyan = new Vector4(0.0f,1.0f,0.5f,1.0f);
		/// <summary>0.5,0,1,1</summary>
		public static Vector4 Purple = new Vector4(0.5f,0.0f,1.0f,1.0f);

		/// <summary>0.05,0.05,0.05,1</summary>
		public static Vector4 Grey05 = new Vector4(0.05f,0.05f,0.05f,1.0f);
		/// <summary>0.1,0.1,0.1,1</summary>
		public static Vector4 Grey10 = new Vector4(0.1f,0.1f,0.1f,1.0f);
		/// <summary>0.2,0.2,0.2,1</summary>
		public static Vector4 Grey20 = new Vector4(0.2f,0.2f,0.2f,1.0f);
		/// <summary>0.3,0.3,0.3,1</summary>
		public static Vector4 Grey30 = new Vector4(0.3f,0.3f,0.3f,1.0f);
		/// <summary>0.4,0.4,0.4,1</summary>
		public static Vector4 Grey40 = new Vector4(0.4f,0.4f,0.4f,1.0f);
		/// <summary>0.5,0.5,0.5,1</summary>
		public static Vector4 Grey50 = new Vector4(0.5f,0.5f,0.5f,1.0f);
		/// <summary>0.6,0.6,0.6,1</summary>
		public static Vector4 Grey60 = new Vector4(0.6f,0.6f,0.6f,1.0f);
		/// <summary>0.7,0.7,0.7,1</summary>
		public static Vector4 Grey70 = new Vector4(0.7f,0.7f,0.7f,1.0f);
		/// <summary>0.8,0.8,0.8,1</summary>
		public static Vector4 Grey80 = new Vector4(0.8f,0.8f,0.8f,1.0f);
		/// <summary>0.9,0.9,0.9,1</summary>
		public static Vector4 Grey90 = new Vector4(0.9f,0.9f,0.9f,1.0f);
	}
}

