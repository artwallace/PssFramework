using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Coordinate2i
	{
		#region Constructor
		
		public Coordinate2i(Int32 x, Int32 y)
		{
			_X = x;
			_Y = y;
		}
		
		#endregion
		
		#region XY
		
		private Int32 _X;
		public Int32 X { get { return _X; } }
		
		private Int32 _Y;
		public Int32 Y { get { return _Y; } }
		
		#endregion
		
		#region Static Presets
		
		public static readonly Coordinate2i X0Y0 = new Coordinate2i(0, 0);
		
		#endregion
	}
}
