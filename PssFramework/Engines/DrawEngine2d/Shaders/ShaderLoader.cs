using System;
using System.IO;
using System.Reflection;
using Sce.Pss.Core.Graphics;

namespace PssFramework.Engines.DrawEngine2d.Shaders
{
	internal static class ShaderLoader
	{
		private static Assembly ResourceAssembly = Assembly.GetExecutingAssembly();
		
		internal static ShaderProgram Load(String resourcePath)
		{
			Byte[] shaderFile = GetEmbeddedResource(resourcePath);
			ShaderProgram shaderProgram = new ShaderProgram(shaderFile);
			return shaderProgram;
		}
		
		private static Byte[] GetEmbeddedResource(String resourcePath)
		{
			if (ResourceAssembly.GetManifestResourceInfo(resourcePath) == null)
			{
				//String[] allResources = ResourceAssembly.GetManifestResourceNames();
				throw new ArgumentException("Unable to load shader from resource: " + resourcePath);
			}
			
			Stream stream = ResourceAssembly.GetManifestResourceStream(resourcePath);
			Byte[] data = new Byte[stream.Length];
			stream.Read(data, 0, data.Length);
			return data;
		}
	}
}

