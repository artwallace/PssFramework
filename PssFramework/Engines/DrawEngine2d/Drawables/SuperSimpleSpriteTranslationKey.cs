using System;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	//TODO: Need to get rid of this struct and keep data in the sprite class.
	public struct SuperSimpleSpriteTranslationKey
	{
		public Single X;
		public Single Y;
		public Single Scale;
		public Single Angle;
		
		public SuperSimpleSpriteTranslationKey(Single x, Single y, Single scale, Single angle)
		{
			X = x;
			Y = y;
			Scale = scale;
			Angle = angle;
		}
	}
}

