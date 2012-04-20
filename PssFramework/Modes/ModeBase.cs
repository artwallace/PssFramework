using System;

namespace PssFramework.Modes
{
	public abstract class ModeBase : IDisposable
	{
		public AppManager Mgr { get; private set; }
		
		#region Constructor, Dispose
		
		protected ModeBase(AppManager mgr)
		{
			if (mgr == null)
				throw new ArgumentNullException();
			Mgr = mgr;
			
			InitializeInternal();
			Initialize();
		}
		
		public virtual void Dispose()
		{
			Cleanup();
			CleanupInternal();
			
			Mgr = null;
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		protected abstract void InitializeInternal();
		
		protected abstract void CleanupInternal();
		
		#endregion
		
		#region Update, Render
		
		internal abstract void UpdateInternal();
		
		internal abstract void RenderInternal();
		
		#endregion
		
		#region Mode Logic
		
		protected abstract void Initialize();
		
		protected abstract void Cleanup();
		
		public abstract void Update();
		
		#endregion
		
		#region Fps Governor
		
		public virtual Boolean UseCustomFpsLimit { get { return false; } }
		public virtual FpsPresets FpsLimit { get { return FpsPresets.Max60Fps; } }
		
		#endregion
	}
}

