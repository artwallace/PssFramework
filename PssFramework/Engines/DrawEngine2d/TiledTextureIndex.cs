using System;

namespace PsmFramework.Engines.DrawEngine2d
{
	public struct TiledTextureIndex
	{
		#region Constructor
		
		public TiledTextureIndex(Int32 column, Int32 row = 0)
		{
			if(column < 0)
				throw new ArgumentOutOfRangeException();
			
			if(row < 0)
				throw new ArgumentOutOfRangeException();
			
			Column = column;
			Row = row;
		}
		
		#endregion
		
		#region Column, Row
		
		public readonly Int32 Column;
		
		public readonly Int32 Row;
		
		#endregion
	}
}

