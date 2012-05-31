/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using Sce.Pss.Core;
using Sce.Pss.HighLevel.GameEngine2D.Base;

namespace Sce.Pss.HighLevel.GameEngine2D
{
	/// <summary>
	/// Base class for single sprite nodes.
	/// This is an abstract class.
	/// </summary>
	public abstract class SpriteBase : Node
	{
		/// <summary>
		/// Sprite geometry in the node's local space. 
		/// A TRS defines an oriented rectangle.
		/// </summary>
		public TRS Quad = TRS.Quad0_1;
		/// <summary>If true, the sprite UV are flipped horizontally.</summary>
		public bool FlipU = false;
		/// <summary>If true, the sprite UV are flipped vertically.</summary>
		public bool FlipV = false;
		/// <summary>The sprite color.</summary>
		public Vector4 Color = Colors.White;
		/// <summary>The blend mode.</summary>
		public BlendMode BlendMode = BlendMode.Normal;
		/// <summary>
		/// This is used only if the Sprite is drawn standalone (not in a SpriteList).
		/// If Sprite is used in a SpriteList, then the SpriteList's TextureInfo is used.
		/// </summary>
		public TextureInfo TextureInfo;
		/// <summary>The shader.</summary>
		public SpriteRenderer.ISpriteShader Shader = (SpriteRenderer.ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;
		/// <summary>Return the dimensions of this sprite in pixels.</summary>
		abstract public Vector2 CalcSizeInPixels();

		/// <summary>SpriteBase constructor.</summary>
		public SpriteBase()
		{
		}

		/// <summary>SpriteBase constructor.</summary>
		public SpriteBase( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		/// <summary>The draw function (expensive, standalone draw).</summary>
		public override void Draw()
		{
			Common.Assert( TextureInfo != null, "Sprite's TextureInfo is null" );
			Common.Assert( Shader != null, "Sprite's Shader is null" );

//			base.Draw(); // AdHocDraw

			////Common.Profiler.Push("SpriteBase.Draw");

			////Common.Profiler.Push("SpriteBase.Draw prelude");
			Director.Instance.GL.SetBlendMode( BlendMode );
			Shader.SetColor( ref Color );
			Shader.SetUVTransform( ref Math.UV_TransformFlipV );
			////Common.Profiler.Pop();
			Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, 1 );
			////Common.Profiler.Push("SpriteBase.internal_draw()");
			internal_draw();
			////Common.Profiler.Pop();
			////Common.Profiler.Push("SpriteBase.Draw end");
			Director.Instance.SpriteRenderer.EndSprites(); 
			////Common.Profiler.Pop();

			////Common.Profiler.Pop();
		}

		/// <summary>
		/// The content local bounds is the smallest Bounds2 containing this 
		/// sprite's Quad, and Quad itself (the sprite rectangle) if there is 
		/// no rotation.
		/// </summary>
		public override bool GetlContentLocalBounds( ref Bounds2 bounds )
		{
			bounds = GetlContentLocalBounds();
			return true;
		}

		/// <summary>
		/// The content local bounds is the smallest Bounds2 containing this 
		/// sprite's Quad, and Quad itself (the sprite rectangle) if there is 
		/// no rotation.
		/// </summary>
		public Bounds2 GetlContentLocalBounds()
		{
			return Quad.Bounds2();
		}

		/// <summary>
		/// Stretch sprite Quad so that it covers the entire screen. The scene
		/// needs to have been set/started, since it uses CurrentScene.Camera.
		/// </summary>
		public void MakeFullScreen()
		{
			Quad = new TRS( Director.Instance.CurrentScene.Camera.CalcBounds() );
		}

		/// <summary>
		/// Translate sprite geometry so that center of the sprite becomes aligned 
		/// with the position of the Node.
		/// </summary>
		public void CenterSprite()
		{
			Quad.Centering( new Vector2( 0.5f, 0.5f ) );
//			Quad.Centering( TRS.Local.Center ); // same as above
		}

		/// <summary>
		/// Modify the center of the sprite geometry.
		/// </summary>
		/// <param name="new_center">
		/// The new center, specified in Node local coordinates.
		/// You can pass constants defined under TRS.Local for conveniency.
		/// </param>
		public void CenterSprite( Vector2 new_center )
		{
			Quad.Centering( new_center );
		}

		abstract internal void internal_draw();
		abstract internal void internal_draw_cpu_transform();
	}

	/// <summary>
	/// SpriteUV is a sprite for which you set uvs manually. Uvs are stored as a TRS object. 
	/// Note that the cost of using SpriteUV alone is heavy, try as much as you can to 
	/// use then as children of SpriteList.
	/// </summary>
	public class SpriteUV 
	: SpriteBase
	{
		/// <summary>The UV is specified as a TRS, which lets you define any oriented rectangle in the UV domain.</summary>
		public TRS UV = TRS.Quad0_1;

		/// <summary>SpriteUV constructor.</summary>
		public SpriteUV()
		{
		}

		/// <summary>SpriteUV constructor.</summary>
		public SpriteUV( TextureInfo texture_info )
		: base( texture_info )
		{
		}

		/// <summary>
		/// Based on the uv and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// If the uv is Quad0_1 (the 0,1 unit quad), then this will return thr texture size in pixels.
		/// </summary>
		override public Vector2 CalcSizeInPixels()
		{
			Common.Assert( TextureInfo != null );
			Common.Assert( TextureInfo.Texture != null );

			return new Vector2( UV.S.X * (float)TextureInfo.Texture.Width ,
								UV.S.Y * (float)TextureInfo.Texture.Height );
		}

		override internal void internal_draw()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, ref UV );
		}

		override internal void internal_draw_cpu_transform()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Matrix3 trans = GetTransform(); // warning: ignored local Camera and VertexZ
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, ref UV, ref trans );
		}
	}

	/// <summary>
	/// SpriteTile is a sprite for which you specify a tile index (1D or 2D) in a TextureInfo. 
	/// Note that the cost of using SpriteUV alone is heavy, try as much as you can to use 
	/// then as children of SpriteList.
	/// </summary>
	public class SpriteTile : SpriteBase
	{
		/// <summary>
		/// TileIndex2D defines the UV that will be used for this sprite. 
		/// Tiles are indexed in 2 dimensions.
		/// </summary>
		public Vector2i TileIndex2D = new Vector2i(0,0);

		/// <summary>
		/// Instead of TileIndex2D you can also work with a flattened 1d index, for animation, etc.
		/// In that case the set/get calculation depend on TextureInfo, so TextureInfo must have 
		/// been set properly.
		/// </summary>
		public int TileIndex1D
		{
			set 
			{ 
				Common.Assert( TextureInfo != null );
				TileIndex2D = new Vector2i( value % TextureInfo.NumTiles.X, value / TextureInfo.NumTiles.X );
			}

			get 
			{ 
				Common.Assert( TextureInfo != null );
				return TileIndex2D.X + TileIndex2D.Y * TextureInfo.NumTiles.X; 
			}
		}

		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		public SpriteTile()
		{
		}

		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		public SpriteTile( TextureInfo texture_info )
		: base( texture_info )
		{
		}

		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		/// <param name="texture_info">The tiled texture object.</param>
		/// <param name="index">2D tile index. (0,0) is the bottom left tile.</param>
		public SpriteTile( TextureInfo texture_info, Vector2i index )
		: base( texture_info )
		{
			TileIndex2D = index;
		}

		/// <summary>
		/// SpriteTile constructor.
		/// </summary>
		/// <param name="texture_info">The tiled texture object.</param>
		/// <param name="index">1D tile index. Flat indexing starts from bottom left tile, which is (0,0) in 2D.</param>
		public SpriteTile( TextureInfo texture_info, int index )
		: base( texture_info )
		{
			TileIndex1D = index;
		}

		/// <summary>
		/// Based on the uv and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// </summary>
		override public Vector2 CalcSizeInPixels()
		{
			// in the tile case, all sprites have the same pixel size
			return TextureInfo.TileSizeInPixelsf;
		}

		override internal void internal_draw()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, TileIndex2D );
		}

		override internal void internal_draw_cpu_transform()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Matrix3 trans = GetTransform(); // warning: ignored local Camera and VertexZ
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, TileIndex2D, ref trans );
		}
	}

	/// <summary>
	/// Draw sprites in batch to reduce the number of draw calls, state setup etc.
	/// 
	/// Just adding SpriteUV or SpriteTile objects as children of a SpriteList with AddChild()
	/// will enable batch rendering, with the limitation that the TextureInfo, BlendMode, 
	/// and Color property of the sprites will be ignored in favor of the parent SpriteList's 
	/// TextureInfo, BlendMode, and Color properties.
	/// 
	/// Important: some functions in SpriteUV and SpriteTile use their local TextureInfo
	/// instead of the parent's SpriteTile one, so you probably want to set both to be safe.
	/// </summary>
	public class SpriteList : Node
	{
		/// <summary>
		/// If EnableLocalTransform flag is true, the children sprite's local transform matrices get used,
		/// but vertices get partly transformed on the cpu. You can turn this behavior off to ignore the local 
		/// transform matrix to save a little bit of cpu processing (and rely on Sprite's Quad only 
		/// to position the sprite). In that case (EnableLocalTransform=false) the Position, Scale, Skew, Pivot 
		/// will be ignored.
		/// </summary>
		public bool EnableLocalTransform = true;
		/// <summary>The color that will be used for all sprites in the Children list.</summary>
		public Vector4 Color = Colors.White;
		/// <summary>The blend mode that will be used for all sprites in the Children list.</summary>
		public BlendMode BlendMode = BlendMode.Normal;
		/// <summary>The TextureInfo object that will be used for all sprites in the Children list.</summary>
		public TextureInfo TextureInfo; 
		/// <summary>The shader that will be used for all sprites in the Children list.</summary>
		public SpriteRenderer.ISpriteShader Shader = (SpriteRenderer.ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;

		/// <summary>
		/// SpriteList constructor.
		/// TextureInfo must be specified in constructor since there is no default for it.
		/// </summary>
		public SpriteList( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		public override void DrawHierarchy()
		{
			if ( !Visible )
				return;

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawTransform ) != 0 )
				DebugDrawTransform();
//			#endif

			////Common.Profiler.Push("DrawHierarchy's PushTransform");
			PushTransform();
			////Common.Profiler.Pop();

			{
				Director.Instance.GL.SetBlendMode( BlendMode );
				Shader.SetColor( ref Color );
				Shader.SetUVTransform( ref Math.UV_TransformFlipV );
				Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, Children.Count );
			}

			int index=0;
			for ( ; index < Children.Count; ++index )
			{
				if ( Children[index].Order >= 0 ) break;

				if ( !EnableLocalTransform ) ((SpriteBase)Children[index]).internal_draw();
				else ((SpriteBase)Children[index]).internal_draw_cpu_transform();
			}

			////Common.Profiler.Push("DrawHierarchy's PostDraw");
			Draw();
			////Common.Profiler.Pop();

			for ( ; index < Children.Count; ++index )
			{
				if ( !EnableLocalTransform ) ((SpriteBase)Children[index]).internal_draw();
				else ((SpriteBase)Children[index]).internal_draw_cpu_transform();
			}

			{
				Director.Instance.SpriteRenderer.EndSprites(); 
			}

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawPivot ) != 0 )
				DebugDrawPivot();

			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawContentLocalBounds ) != 0 )
				DebugDrawContentLocalBounds();
//			#endif

			////Common.Profiler.Push("DrawHierarchy's PopTransform");
			PopTransform();
			////Common.Profiler.Pop();
		}
	}

	/// <summary>Data struct used by RawSpriteTileList.</summary>
	public struct RawSpriteTile
	{
		/// <summary>Sprite geometry (position, rotation, scale define a rectangle).</summary>
		public TRS Quad;
		/// <summary>The tile index.</summary>
		public Vector2i TileIndex2D;
		/// <summary>If true, the sprite UV are flipped horizontally.</summary>
		public bool FlipU;
		/// <summary>If true, the sprite UV are flipped vertically.</summary>
		public bool FlipV;
//		float VertexZ;

		/// <summary>RawSpriteTile constructor.</summary>
		public RawSpriteTile( TRS positioning, Vector2i tile_index, bool flipu=false, bool flipv=false )
		{
			Quad = positioning;
			TileIndex2D = tile_index;
			FlipU = flipu;
			FlipV = flipv;
//			VertexZ = vertexz;
		}
	}

	/// <summary>
	/// Draw sprites in batch to reduce number of draw calls, state setup etc.
	/// Unlike SpriteList, instead of holding a list of Node objects, this holds 
	/// a list of RawSpriteTile, which is more lightweight. In effect this is a 
	/// thin wrap of SpriteRenderer, usable as a Node.
	/// </summary>
	public class RawSpriteTileList : Node
	{
		/// <summary>The list of RawSpriteTile objects to render.</summary>
		public List< RawSpriteTile > Sprites = new List< RawSpriteTile >();
		/// <summary>The color that will be used for all sprites in the Children list.</summary>
		public Vector4 Color = Colors.White;
		/// <summary>The blend mode that will be used for all sprites in the Children list.</summary>
		public BlendMode BlendMode = BlendMode.Normal;
		/// <summary>The TextureInfo object that will be used for all sprites in the Children list.</summary>
		public TextureInfo TextureInfo; 
		/// <summary>The shader that will be used for all sprites in the Children list.</summary>
		public SpriteRenderer.ISpriteShader Shader = (SpriteRenderer.ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;

		/// <summary>
		/// RawSpriteTileList constructor.
		/// TextureInfo must be specified in constructor since there is no default for it.
		/// </summary>
		public RawSpriteTileList( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		/// <summary>The draw function, draws all sprites in Sprites list.</summary>
		public override void Draw()
		{
			Director.Instance.GL.SetBlendMode( BlendMode );
			Shader.SetColor( ref Color );
			Shader.SetUVTransform( ref Math.UV_TransformFlipV );
			Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, Sprites.Count );

//			System.Console.WriteLine( Sprites.Count );

			foreach ( RawSpriteTile sprite in Sprites )
			{
				Director.Instance.SpriteRenderer.FlipU = sprite.FlipU;
				Director.Instance.SpriteRenderer.FlipV = sprite.FlipV;
				TRS copy = sprite.Quad;
				Director.Instance.SpriteRenderer.AddSprite( ref copy, sprite.TileIndex2D );
			}

			Director.Instance.SpriteRenderer.EndSprites(); 
		}

		/// <summary>
		/// Based on the tile size and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// </summary>
		public Vector2 CalcSizeInPixels()
		{
			// in the tile case, all sprites have the same pixel size
			return TextureInfo.TileSizeInPixelsf;
		}
	}

} // namespace Sce.Pss.HighLevel.GameEngine2D

