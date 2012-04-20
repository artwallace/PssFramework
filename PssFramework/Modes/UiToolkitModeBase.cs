using System;

namespace PssFramework.Modes
{
	public abstract class UiToolkitModeBase : ModeBase
	{
		#region Constructor, Dispose
		
		protected UiToolkitModeBase(AppManager mgr)
			: base(mgr)
		{
		}
		
		public override void Dispose()
		{
			base.Dispose();
		}
		
		#endregion
	}
}

