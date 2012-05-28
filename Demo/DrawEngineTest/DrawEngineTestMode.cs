using Demo.MainMenu;
using PssFramework;
using PssFramework.Engines.DrawEngine2d;
using PssFramework.Engines.DrawEngine2d.DrawItems;
using PssFramework.Engines.DrawEngine2d.Support;
using PssFramework.Modes;
using PssFramework.Modes.TopDown2dAlt;

namespace Demo.DrawEngineTest
{
	public class DrawEngineTestMode : TopDown2dAltModeBase
	{
		#region Constructor
		
		public DrawEngineTestMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Logic
		
		protected override void Initialize()
		{
			//TODO: Remove this after testing!
			DrawEngine2d.ClearColor = Colors.Blue;
			//EnableDebugInfo();
			
			Layer l = DrawEngine2d.CreateLayer(1);
			
			UltraSimpleSprite uss = new UltraSimpleSprite(DrawEngine2d);
			
			l.Items.Add(uss);
		}
		
		protected override void Cleanup()
		{
		}
		
		public override void Update()
		{
			if (Mgr.GamePad0_Start_Pressed && Mgr.ModeChangeAllowed)
			{
				Mgr.GoToMode(MainMenuMode.MainMenuModeFactory);
				return;
			}
			
			if (Mgr.GamePad0_Select_Pressed)
			{
				if (Mgr.RunState == RunState.Running)
					Mgr.SetRunStateToPaused();
				else
					Mgr.SetRunStateToRunning();
			}
		}
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase DrawEngineTestModeFactory(AppManager mgr)
		{
			return new DrawEngineTestMode(mgr);
		}
		
		#endregion
	}
}

