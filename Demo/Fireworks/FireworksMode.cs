using System;
using Demo.MainMenu;
using PsmFramework;
using PsmFramework.Modes;
using PsmFramework.Modes.FixedFront2d;
using Sce.Pss.Core;
using Sce.Pss.HighLevel.GameEngine2D.Base;
using PsmFramework.Engines.PadServer;
using Sce.Pss.Core.Input;

namespace Demo.Fireworks
{
	public class FireworksMode : FixedFront2dModeBase
	{
		protected override UInt32 SpritesCapacity { get { return 400; } }
		protected override UInt32 DrawHelpersCapacity { get { return 500; } }
		protected override Vector4 ClearColor { get { return Colors.Black; } }
		protected override Boolean DrawDebugGrid { get { return true; } }
		
		#region Constructor
		
		public FireworksMode(AppManager mgr)
			: base(mgr)
		{
		}
		
		#endregion
		
		#region Mode Logic
		
		protected override void Initialize()
		{
			EnableDebugInfo();
			
			//TODO: For testing PadServer. Remove later.
			gpadReader = new GamepadReader(ControllerTypes.SonySixaxis);
			gpadReader.OpenChannel();
		}
		
		protected override void Cleanup()
		{
			//TODO: For testing PadServer. Remove later.
			gpadReader.CloseChannel();
			gpadReader.Dispose();
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
			
			//TODO: For testing PadServer. Remove later.
			GamePadData data = gpadReader.GetData();
		}
		
		//TODO: For testing PadServer. Remove later.
		private GamepadReader gpadReader;
		
		#endregion
		
		#region Mode Factory Delegate
		
		public static ModeBase FireworksModeFactory(AppManager mgr)
		{
			return new FireworksMode(mgr);
		}
		
		#endregion
	}
}

