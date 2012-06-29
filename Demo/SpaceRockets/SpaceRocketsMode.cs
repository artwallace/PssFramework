using Demo.MainMenu;
using PsmFramework;
using PsmFramework.Engines.DrawEngine2d;
using PsmFramework.Engines.DrawEngine2d.Drawables;
using PsmFramework.Engines.DrawEngine2d.Support;
using PsmFramework.Modes;
using PsmFramework.Modes.TopDown2dAlt;

namespace Demo.SpaceRockets
{
	public class SpaceRocketsMode : TopDown2dAltModeBase
	{
		#region Constructor
		
		public SpaceRocketsMode(AppManager mgr)
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
			
			Layer l = DrawEngine2d.GetOrCreateLayer(1);
			//UltraSimpleSprite uss = new UltraSimpleSprite(l);
			
			TiledTexture tt = new TiledTexture(DrawEngine2d, "/Application/TwinStickShooter/Images/Ship64.png");
			
			Layer l2 = DrawEngine2d.GetOrCreateLayer(2);
			SuperSimpleSpriteGroup sssg = new SuperSimpleSpriteGroup(l2, tt);
			SuperSimpleSprite sss1 = new SuperSimpleSprite(sssg);
			sss1.SetPositionFromCenter(new Coordinate2(32f, 32f));
			SuperSimpleSprite sss2 = new SuperSimpleSprite(sssg);
			sss2.SetPositionFromCenter(new Coordinate2(96f, 32f));
			//sss2.Rotation = 45.0f;
			
			Layer debugOverlay = DrawEngine2d.GetOrCreateLayer(10);
			_DebugTextLabel = new DebugText(debugOverlay);
			_DebugTextLabel.Text = "Test";
			_DebugTextLabel.Position = new Coordinate2(100f, 100f);
		}
		
		protected override void Cleanup()
		{
			_DebugTextLabel = null;
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
			return new SpaceRocketsMode(mgr);
		}
		
		#endregion
		
		private DebugText _DebugTextLabel;
	}
}

