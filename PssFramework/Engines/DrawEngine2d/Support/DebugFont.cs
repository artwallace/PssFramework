using System;
using System.Collections.Generic;
using Sce.Pss.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	internal class DebugFont : IDisposable
	{
		#region Constructor, Dispose
		
		public DebugFont()
		{
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize()
		{
			InitializeGlyphTable();
			InitializeTexture();
		}
		
		private void Cleanup()
		{
			CleanupTexture();
			CleanupGlyphTable();
		}
		
		#endregion
		
		#region Glyph Table
		
		private void InitializeGlyphTable()
		{
			//GlyphTableIndex = new Dictionary<Int32, Char>();
			GlyphTable = new Dictionary<Char, DebugFontGlyph>();
			
			CreateGlyphTableEntry(0x00000000, 0x00000000, ' ');
			CreateGlyphTableEntry(0x10101010, 0x00100000, '!');
			CreateGlyphTableEntry(0x00282828, 0x00000000, '"');
			CreateGlyphTableEntry(0x287c2828, 0x0028287c, '#');
			CreateGlyphTableEntry(0x38147810, 0x00103c50, '$');
			CreateGlyphTableEntry(0x10204c0c, 0x00606408, '%');
			CreateGlyphTableEntry(0x08141408, 0x00582454, '&');
			CreateGlyphTableEntry(0x00102040, 0x00000000, '\'');
			CreateGlyphTableEntry(0x10102040, 0x00402010, '(');
			CreateGlyphTableEntry(0x10100804, 0x00040810, ')');
			CreateGlyphTableEntry(0x10385410, 0x00105438, '*');
			CreateGlyphTableEntry(0x7c101000, 0x00001010, '+');
			CreateGlyphTableEntry(0x00000000, 0x08101000, ',');
			CreateGlyphTableEntry(0x7c000000, 0x00000000, '-');
			CreateGlyphTableEntry(0x00000000, 0x00181800, '.');
			CreateGlyphTableEntry(0x10204000, 0x00000408, '/');
			CreateGlyphTableEntry(0x54644438, 0x0038444c, '0');
			CreateGlyphTableEntry(0x10141810, 0x007c1010, '1');
			CreateGlyphTableEntry(0x20404438, 0x007c0418, '2');
			CreateGlyphTableEntry(0x30404438, 0x00384440, '3');
			CreateGlyphTableEntry(0x24283020, 0x0020207c, '4');
			CreateGlyphTableEntry(0x403c047c, 0x00384440, '5');
			CreateGlyphTableEntry(0x3c040830, 0x00384444, '6');
			CreateGlyphTableEntry(0x1020447c, 0x00101010, '7');
			CreateGlyphTableEntry(0x38444438, 0x00384444, '8');
			CreateGlyphTableEntry(0x78444438, 0x00182040, '9');
			CreateGlyphTableEntry(0x00100000, 0x00001000, ':');
			CreateGlyphTableEntry(0x00100000, 0x08101000, ';');
			CreateGlyphTableEntry(0x0c183060, 0x00603018, '<');
			CreateGlyphTableEntry(0x007c0000, 0x0000007c, '=');
			CreateGlyphTableEntry(0x6030180c, 0x000c1830, '>');
			CreateGlyphTableEntry(0x20404438, 0x00100010, '?');
			CreateGlyphTableEntry(0x54744438, 0x00380474, '@');
			CreateGlyphTableEntry(0x44442810, 0x0044447c, 'A');
			CreateGlyphTableEntry(0x3c48483c, 0x003c4848, 'B');
			CreateGlyphTableEntry(0x04044830, 0x00304804, 'C');
			CreateGlyphTableEntry(0x4848281c, 0x001c2848, 'D');
			CreateGlyphTableEntry(0x3c04047c, 0x007c0404, 'E');
			CreateGlyphTableEntry(0x3c04047c, 0x00040404, 'F');
			CreateGlyphTableEntry(0x74044438, 0x00384444, 'G');
			CreateGlyphTableEntry(0x7c444444, 0x00444444, 'H');
			CreateGlyphTableEntry(0x10101038, 0x00381010, 'I');
			CreateGlyphTableEntry(0x20202070, 0x00182420, 'J');
			CreateGlyphTableEntry(0x0c142444, 0x00442414, 'K');
			CreateGlyphTableEntry(0x04040404, 0x007c0404, 'L');
			CreateGlyphTableEntry(0x54546c44, 0x00444444, 'M');
			CreateGlyphTableEntry(0x544c4c44, 0x00446464, 'N');
			CreateGlyphTableEntry(0x44444438, 0x00384444, 'O');
			CreateGlyphTableEntry(0x3c44443c, 0x00040404, 'P');
			CreateGlyphTableEntry(0x44444438, 0x00582454, 'Q');
			CreateGlyphTableEntry(0x3c44443c, 0x00442414, 'R');
			CreateGlyphTableEntry(0x38044438, 0x00384440, 'S');
			CreateGlyphTableEntry(0x1010107c, 0x00101010, 'T');
			CreateGlyphTableEntry(0x44444444, 0x00384444, 'U');
			CreateGlyphTableEntry(0x44444444, 0x00102828, 'V');
			CreateGlyphTableEntry(0x54444444, 0x00446c54, 'W');
			CreateGlyphTableEntry(0x10284444, 0x00444428, 'X');
			CreateGlyphTableEntry(0x38444444, 0x00101010, 'Y');
			CreateGlyphTableEntry(0x1020407c, 0x007c0408, 'Z');
			CreateGlyphTableEntry(0x10101070, 0x00701010, '[');
			CreateGlyphTableEntry(0x10080400, 0x00004020, '\\');
			CreateGlyphTableEntry(0x1010101c, 0x001c1010, ']');
			CreateGlyphTableEntry(0x00442810, 0x00000000, '^');
			CreateGlyphTableEntry(0x00000000, 0x007c0000, '_');
			CreateGlyphTableEntry(0x00100804, 0x00000000, '`');
			CreateGlyphTableEntry(0x40380000, 0x00784478, 'a');
			CreateGlyphTableEntry(0x4c340404, 0x00344c44, 'b');
			CreateGlyphTableEntry(0x44380000, 0x00384404, 'c');
			CreateGlyphTableEntry(0x64584040, 0x00586444, 'd');
			CreateGlyphTableEntry(0x44380000, 0x0038047c, 'e');
			CreateGlyphTableEntry(0x7c105020, 0x00101010, 'f');
			CreateGlyphTableEntry(0x64580000, 0x38405864, 'g');
			CreateGlyphTableEntry(0x4c340404, 0x00444444, 'h');
			CreateGlyphTableEntry(0x10180010, 0x00381010, 'i');
			CreateGlyphTableEntry(0x10180010, 0x0c121010, 'j');
			CreateGlyphTableEntry(0x14240404, 0x0024140c, 'k');
			CreateGlyphTableEntry(0x10101018, 0x00381010, 'l');
			CreateGlyphTableEntry(0x542c0000, 0x00545454, 'm');
			CreateGlyphTableEntry(0x4c340000, 0x00444444, 'n');
			CreateGlyphTableEntry(0x44380000, 0x00384444, 'o');
			CreateGlyphTableEntry(0x4c340000, 0x0404344c, 'p');
			CreateGlyphTableEntry(0x64580000, 0x40405864, 'q');
			CreateGlyphTableEntry(0x4c340000, 0x00040404, 'r');
			CreateGlyphTableEntry(0x04780000, 0x003c403c, 's');
			CreateGlyphTableEntry(0x083c0808, 0x00304808, 't');
			CreateGlyphTableEntry(0x24240000, 0x00582424, 'u');
			CreateGlyphTableEntry(0x44440000, 0x00102844, 'v');
			CreateGlyphTableEntry(0x54440000, 0x00285454, 'w');
			CreateGlyphTableEntry(0x28440000, 0x00442810, 'x');
			CreateGlyphTableEntry(0x44440000, 0x38405864, 'y');
			CreateGlyphTableEntry(0x207c0000, 0x007c0810, 'z');
			CreateGlyphTableEntry(0x04080830, 0x00300808, '{');
			CreateGlyphTableEntry(0x10101010, 0x00101010, '|');
			CreateGlyphTableEntry(0x2010100c, 0x000c1010, '}');
			CreateGlyphTableEntry(0x0000007c, 0x00000000, '~');
		}
		
		private void CleanupGlyphTable()
		{
			//GlyphTableIndex.Clear();
			//GlyphTableIndex = null;
			
			GlyphTable.Clear();
			GlyphTable = null;
		}
		
		//private Dictionary<Int32, Char> GlyphTableIndex;
		private Dictionary<Char, DebugFontGlyph> GlyphTable;
		
		private void CreateGlyphTableEntry(UInt32 data1, UInt32 data2, Char c)
		{
			//GlyphTableIndex[(Int32)c] = c;
			GlyphTable[c] = new DebugFontGlyph(c, data1, data2);
		}
		
		//These values are tied directly to the hardcoded glyph data and should not be altered.
		internal const Int32 FontWidth = 8;
		internal const Int32 FontHeight = 8;
		
		private const Int32 NotPrintableChars = 32;
		
		private Int32 GetGlyphIndex(Char c)
		{
			return (Int32)c - NotPrintableChars;
		}
		
		#endregion
		
		#region Texture
		
		private void InitializeTexture()
		{
			GlyphTexturePositions = new Dictionary<Char, RectangularArea2i>();
			
			GenerateTexture();
		}
		
		private void CleanupTexture()
		{
			GlyphTexturePositions.Clear();
			GlyphTexturePositions = null;
			
			Texture.Dispose();
			Texture = null;
		}
		
		private const Int32 MaxTextureCharCapacity = 128;
		
		private const Byte PixelDark = (Byte)0x00;
		private const Byte PixelLit = (Byte)0xff;
		
		public Texture2D Texture { get; private set; }
		
		private Dictionary<Char, RectangularArea2i> GlyphTexturePositions;
		
		private RectangularArea2i CalcPositionOfCharInTexture(Char c)
		{
			Int32 ci = (Int32)c - NotPrintableChars;
			
			return new RectangularArea2i(
				ci * FontWidth,
				0,
				ci * FontWidth + FontWidth,
				FontHeight
				);
		}
		
		private void GenerateTexture()
		{
			Int32 textureWidth = MaxTextureCharCapacity * FontWidth;
			Int32 textureHeight = FontHeight;
			
			Byte[] texturePixels = new Byte[textureWidth * textureHeight];
			
			//TODO: Is this necessary?
			for (Int32 i = 0; i < texturePixels.Length; i++)
			{
				if (texturePixels[i] != PixelDark)
					throw new InvalidProgramException("Just wanted to see if this was needed.");
				texturePixels[i] = PixelDark;
			}
			
			foreach(DebugFontGlyph glyph in GlyphTable.Values)
				DecodeCharPixelData(ref texturePixels, glyph);
			
			Texture = new Texture2D(textureWidth, textureHeight, false, PixelFormat.Luminance);
			Texture.SetPixels(0, texturePixels, PixelFormat.Luminance);
			Texture.SetFilter(TextureFilterMode.Nearest, TextureFilterMode.Nearest, TextureFilterMode.Nearest);
		}
		
		private void DecodeCharPixelData(ref Byte[] texturePixels, DebugFontGlyph glyph)
		{
			Int32 halfway = FontWidth * FontHeight / 2;
			Boolean pixelIsLit;
			
			for (Int32 y = 0; y < FontHeight; y++)
			{
				for (Int32 x = 0; x < FontWidth; x++)
				{
					Int32 charPixelIndex = x + FontHeight * y;
					
					if(charPixelIndex < halfway)
						pixelIsLit = ((glyph.Data1 & (1 << charPixelIndex)) != 0);
					else
						pixelIsLit = ((glyph.Data2 & (1 << (charPixelIndex - halfway))) != 0);
					
					//(c * CharSizei.X + x) + y * font_size.X
					Int32 charPstn = GetGlyphIndex(glyph.Character);
					Int32 texturePixelIndex = (charPstn * FontWidth + x) + (FontWidth * y);
					
					texturePixels[texturePixelIndex] = pixelIsLit ? PixelLit : PixelDark;
				}
			}
		}
		
		#endregion
	}
}

