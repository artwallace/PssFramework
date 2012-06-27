using System;

namespace PsmFramework.Engines.DrawEngine2d.Shaders
{
	public class FontShader : ShaderBase
	{
		#region Constructor
		
		public FontShader(DrawEngine2d drawEngine2d)
			: base(drawEngine2d)
		{
		}
		
		#endregion
		
		#region Path
		
		public override String Path
		{
			get
			{
				return "PsmFramework.Engines.DrawEngine2d.Shaders.Font.cgx";
			}
		}
		
		#endregion
		
		#region ShaderProgram
		
		protected override void InitializeShaderProgram()
		{
			ShaderProgram.SetUniformBinding(0, "MVP");
			ShaderProgram.SetUniformBinding(1, "Color");
			ShaderProgram.SetUniformBinding(2, "UVTransform");
			
			ShaderProgram.SetAttributeBinding(0, "vin_data");
		}
		
		protected override void CleanupShaderProgram()
		{
		}
		
		#endregion
	}
}

