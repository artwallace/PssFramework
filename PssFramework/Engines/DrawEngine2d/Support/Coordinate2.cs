using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public struct Coordinate2
	{
		#region Constructor
		
		public Coordinate2(Single x, Single y)
		{
			_X = x;
			_Y = y;
		}
		
		#endregion
		
		#region XY
		
		private Single _X;
		public Single X { get { return _X; } }
		
		private Single _Y;
		public Single Y { get { return _Y; } }
		
		#endregion
		
		#region Static Presets
		
		public static readonly Coordinate2 X0Y0 = new Coordinate2(0f, 0f);
		
		#endregion
	}
}
