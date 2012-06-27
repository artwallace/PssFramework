using System;

namespace PsmFramework.Engines.DrawEngine2d.Shaders
{
	public class UltraSimpleSpriteShader : ShaderBase
	{
		#region Constructor
		
		public UltraSimpleSpriteShader(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Path
		
		public override String Path
		{
			get
			{
				return "PsmFramework.Engines.DrawEngine2d.Shaders.UltraSimpleSprite.cgx";
			}
		}
		
		#endregion
		
		#region ShaderProgram
		
		protected override void InitializeShaderProgram()
		{
			ShaderProgram.SetUniformBinding(0, "u_WorldMatrix");
		}
		
		protected override void CleanupShaderProgram()
		{
		}
		
		#endregion
	}
}

