using System;
using Sce.Pss.Core;

namespace PssFramework.Modes.Isometric2d
{
	public class DiamondBackground : BackgroundBase
	{
		#region Constructor
		
		public DiamondBackground(RoomBase room, Int32 columns, Int32 rows, String asset, Int32 assetColumns, Int32 assetRows, Int32 tileWidth, Int32 tileHeight, Int32 horizontalScreenPadding, Int32 verticalScreenPadding)
			: base(room, columns, rows, asset, assetColumns, assetRows, tileWidth, tileHeight, horizontalScreenPadding, verticalScreenPadding)
		{
		}
		
		#endregion
		
		#region Update
		
		private Boolean FirstUpdate = true;
		private Vector2 LastUpdateCameraPstn;
		
		internal override void Update()
		{
			//Only update if something's changed.
			if (LastUpdateCameraPstn == Mode.CameraLowerLeftPosition && !FirstUpdate)
				return;
			FirstUpdate = false;
			LastUpdateCameraPstn = Mode.CameraLowerLeftPosition;
		}
		
		#endregion
		
		#region Factory Delegate
		
		public static BackgroundBase DiamondBackgroundFactory(RoomBase room, Int32 columns, Int32 rows, String asset, Int32 assetColumns, Int32 assetRows, Int32 tileWidth, Int32 tileHeight, Int32 horizontalScreenPadding, Int32 verticalScreenPadding)
		{
			return new DiamondBackground(room, columns, rows, asset, assetColumns, assetRows, tileWidth, tileHeight, horizontalScreenPadding, verticalScreenPadding);
		}
		
		#endregion
	}
}

