using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Support;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d
{
	public class TiledTexture : IDisposable, IEquatable<TiledTexture>
	{
		#region Constructor, Dispose
		
		public TiledTexture(DrawEngine2d drawEngine2d, String path, Int32 columns = 1, Int32 rows = 1)
		{
			Initialize(drawEngine2d, path, columns, rows, RectangularArea2i.Zero);
		}
		
		public TiledTexture(DrawEngine2d drawEngine2d, String path, Int32 columns, Int32 rows, RectangularArea2i sourceArea)
		{
			Initialize(drawEngine2d, path, columns, rows, sourceArea);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(DrawEngine2d drawEngine2d, String path, Int32 columns, Int32 rows, RectangularArea2i sourceArea)
		{
			InitializeDrawEngine2d(drawEngine2d);
			InitializeTexture2D(path);
			InitializeTiles(columns, rows, sourceArea);
			InitializeCachedTileCoordinates();
		}
		
		private void Cleanup()
		{
			CleanupCachedTileCoordinates();
			CleanupTiles();
			CleanupTexture2D();
			CleanupDrawEngine2d();
		}
		
		#endregion
		
		#region DrawEngine2d
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d)
		{
			if (drawEngine2d == null)
				throw new ArgumentNullException();
			
			DrawEngine2d = drawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d = null;
		}
		
		private DrawEngine2d DrawEngine2d;
		
		#endregion
		
		#region Texture2D
		
		private void InitializeTexture2D(String path)
		{
			if(String.IsNullOrWhiteSpace(path))
				throw new ArgumentException();
			
			Path = path;
			Texture = DrawEngine2d.GetOrCreateTexture2D(this, Path);
		}
		
		private void CleanupTexture2D()
		{
			DrawEngine2d.RemoveTexture2D(this, Path);
			Texture = null;
			Path = null;
		}
		
		public String Path { get; private set; }
		
		public Texture2D Texture { get; private set; }
		
		#endregion
		
		#region Tiles
		
		private void InitializeTiles(Int32 columns, Int32 rows, RectangularArea2i sourceArea)
		{
			if (columns < 1 || rows < 1)
				throw new ArgumentException();
			
			Columns = columns;
			Rows = rows;
			
			SourceArea = sourceArea;
			
			if (SourceArea.Left == 0 && SourceArea.Top == 0 && SourceArea.Right == 0 && SourceArea.Bottom == 0)
			{
				SourceAreaWidth = Texture.Width;
				SourceAreaHeight = Texture.Height;
			}
			else
			{
				SourceAreaWidth = SourceArea.Right - SourceArea.Left;
				SourceAreaHeight = SourceArea.Bottom - SourceArea.Top;
			}
			
			if (SourceAreaWidth < 1 || SourceAreaHeight < 1)
				throw new ArgumentOutOfRangeException();
			
			if (SourceAreaWidth % Columns != 0 || SourceAreaHeight % Rows != 0)
				throw new ArgumentOutOfRangeException("Source texture is not evenly divisible by the number of requested tiles.");
			
			TileWidth = SourceAreaWidth / Columns;
			TileHeight = SourceAreaHeight / Rows;
		}
		
		private void CleanupTiles()
		{
		}
		
		public Int32 Columns { get; private set; }
		
		public Int32 Rows { get; private set; }
		
		public Int32 TileWidth { get; private set; }
		
		public Int32 TileHeight { get; private set; }
		
		public RectangularArea2i SourceArea { get; private set; }
		
		public Int32 SourceAreaWidth { get; private set; }
		
		public Int32 SourceAreaHeight { get; private set; }
		
		#endregion
		
		#region Cached Tile Coordinates
		
		private void InitializeCachedTileCoordinates()
		{
			CachedTileCoordinates = new Dictionary<TiledTextureIndex, RectangularArea2i>();
			
			GenerateCachedTileCoordinates();
		}
		
		private void CleanupCachedTileCoordinates()
		{
			CachedTileCoordinates.Clear();
			CachedTileCoordinates = null;
		}
		
		private Dictionary<TiledTextureIndex, RectangularArea2i> CachedTileCoordinates;
		
		private void GenerateCachedTileCoordinates()
		{
			for(Int32 row = 0; row < Rows; row++)
			{
				Int32 topPstn = SourceArea.Top + (row * TileHeight);
				Int32 bottomPstn = SourceArea.Top + (row * TileHeight) + TileHeight;
				
				for(Int32 column = 0; column < Columns; column++)
				{
					Int32 leftPstn = SourceArea.Left + (column * TileWidth);
					Int32 rightPstn = SourceArea.Left + (column * TileWidth) + TileWidth;
					
					TiledTextureIndex tti = new TiledTextureIndex(column, row);
					
					RectangularArea2i ra = new RectangularArea2i(leftPstn, topPstn, rightPstn, bottomPstn);
					
					CachedTileCoordinates.Add(tti, ra);
				}
			}
		}
		
		public RectangularArea2i GetTileCoordinates(TiledTextureIndex index)
		{
			return CachedTileCoordinates[index];
		}
		
		public RectangularArea2i GetTileCoordinates(Int32 column, Int32 row)
		{
			return CachedTileCoordinates[new TiledTextureIndex(column, row)];
		}
		
		#endregion
		
		#region IEquatable, etc.
		
		public override Boolean Equals(Object o)
		{
			if (o is TiledTexture)
				return this.Equals((TiledTexture)o);
			
			return false;
		}
		
		public Boolean Equals(TiledTexture o)
		{
			if(o == null)
				return false;
			
			return
				(Path == o.Path) &&
				(Columns == o.Columns) &&
				(Rows == o.Rows) &&
				(SourceArea == o.SourceArea)
				;
		}
		
		public override Int32 GetHashCode()
		{
			return Path.GetHashCode() ^ SourceArea.GetHashCode() ^ Columns ^ Rows;
		}
		
		public static Boolean operator ==(TiledTexture o1, TiledTexture o2)
		{
			if (Object.ReferenceEquals(o1, o2))
				return true;
			
			if (((Object)o1 == null) || ((Object)o2 == null))
				return false;
			
			return o1.Equals(o2);
		}
		
		public static Boolean operator !=(TiledTexture o1, TiledTexture o2)
		{
			return !(o1 == o2);
		}
		
		#endregion
	}
}

