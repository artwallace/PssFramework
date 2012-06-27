using System;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	internal struct DebugFontGlyph
	{
		public DebugFontGlyph(Char character, UInt32 data1, UInt32 data2)
		{
			Character = character;
			Data1 = data1;
			Data2 = data2;
		}
		
		public readonly Char Character;
		public readonly UInt32 Data1;//Rename this to top half
		public readonly UInt32 Data2;//Rename this to bottom half
		//TODO: See about combining these into one value. Then this class could possibly be deleted.
	}
}

