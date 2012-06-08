using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using PsmFramework.Modes;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Imaging;
using Sce.Pss.Core.Input;

namespace PsmFramework
{
	public sealed class AppManager
	{
		#region Constructor, Dispose
		
		public AppManager(AppOptionsBase options, GraphicsContext gc, FpsPresets maxFps)
		{
			Initialize(options, gc, maxFps);
		}
		
		public void Dispose()
		{
			Cleanup();
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(AppOptionsBase options, GraphicsContext gc, FpsPresets maxFps)
		{
			SetRunStateToInitializing();
			UpdateRunState();
			
			InitializeOptions(options);
			InitializeGraphics(gc);
			InitializePerformance();
			InitializeTimers();
			InitializeFpsGovernor(maxFps);
			//InitializeTextures();
			InitializeModes();
			InitializeInput();
		}
		
		private void Cleanup()
		{
			CleanupInput();
			CleanupOptions();
			CleanupPerformance();
			CleanupModes();
			//CleanupTextures();
			CleanupGraphics();
		}
		
		#endregion
		
		#region AppLoop, Update, Render
		
		public void AppLoop()
		{
			SetRunStateToRunning();
			InitializeTimers();
			InitializeFpsGovernor(DefaultFpsLimit);
			
			InitializeCurrentMode();
			
			while (RunState != RunState.Ending)
			{
				FpsGovernor();
				RecalcTimers();
				
				SystemEvents.CheckEvents();
				
				CountFps();
				
				PerformanceTimer.Reset();
				PerformanceTimer.Start();
				Update();
				UpdateTicks = PerformanceTimer.ElapsedTicks;
				
				PerformanceTimer.Reset();
				PerformanceTimer.Start();
				Render();
				RenderTicks = PerformanceTimer.ElapsedTicks;
				
				UpdateRunState();
				
				if (RunStateRecentlyChanged && RunState == RunState.Paused)
					ResetPauseTimer();
			}
		}
		
		private void Update()
		{
			RefreshInputData();
			
			CurrentMode.UpdateInternal();
			CurrentMode.Update();
			
			if (ModeChanged)
			{
				CleanupPreviousMode();
				InitializeCurrentMode();
			}
		}
		
		private void Render()
		{
			if (!ModeChanged)
				CurrentMode.RenderInternal();
		}
		
		#endregion
		
		#region RunState
		
		public RunState RunState { get; private set; }
		private RunState ChangeToRunState;
		private Boolean RunStateChanging;
		private Boolean RunStateRecentlyChanged;
		
		private void UpdateRunState()
		{
			if (RunStateChanging)
			{
				RunStateChanging = false;
				RunStateRecentlyChanged = true;
				RunState = ChangeToRunState;
			}
			else if (RunStateRecentlyChanged)
				RunStateRecentlyChanged = false;
		}
		
		public void SetRunStateToInitializing()
		{
			RunStateChanging = true;
			RunStateRecentlyChanged = false;
			ChangeToRunState = RunState.Initializing;
		}
		
		public void SetRunStateToRunning()
		{
			RunStateChanging = true;
			RunStateRecentlyChanged = false;
			ChangeToRunState = RunState.Running;
		}
		
		public void SetRunStateToPaused()
		{
			RunStateChanging = true;
			RunStateRecentlyChanged = false;
			ChangeToRunState = RunState.Paused;
		}
		
		public void SetRunStateToEnding()
		{
			RunStateChanging = true;
			RunStateRecentlyChanged = false;
			ChangeToRunState = RunState.Ending;
		}
		
		#endregion
		
		#region Graphics
		
		public GraphicsContext GraphicsContext { get; private set; }
		public Single ScreenWidth { get; private set; }
		public Single ScreenHeight { get; private set; }
		public ImageRect ScreenRectangle { get; private set; }
		
		private void InitializeGraphics(GraphicsContext gc)
		{
			GraphicsContext = gc;
			ScreenWidth = GraphicsContext.Screen.Width;
			ScreenHeight = GraphicsContext.Screen.Height;
			ScreenRectangle = GraphicsContext.Screen.Rectangle;
		}
		
		private void CleanupGraphics()
		{
			GraphicsContext.Dispose();
			GraphicsContext = null;
		}
		
		#endregion
		
		#region Performance
		
		private Stopwatch PerformanceTimer;
		
		public Int64 UpdateTicks { get; private set; }
		public Int64 RenderTicks { get; private set; }
		
		private Int32 CurrentSec;
		private Int32 LastSecTracked;
		private Int32 CurrentAppLoopsPerSec;
		public Int32 AppLoopsPerSec { get; private set; }
		
		private void InitializePerformance()
		{
			PerformanceTimer = new Stopwatch();
			PerformanceTimer.Start();
			
			UpdateTicks = 0;
			RenderTicks = 0;
			
			AppLoopsPerSec = 0;
			CurrentAppLoopsPerSec = 0;
			LastSecTracked = DateTime.Now.Second;
		}
		
		private void CleanupPerformance()
		{
			PerformanceTimer.Stop();
			PerformanceTimer = null;
		}
		
		private void CountFps()
		{
			CurrentSec = DateTime.Now.Second;
			if (CurrentSec == LastSecTracked)
				CurrentAppLoopsPerSec++;
			else
			{
				AppLoopsPerSec = CurrentAppLoopsPerSec;
				LastSecTracked = CurrentSec;
				CurrentAppLoopsPerSec = 1;
			}
		}
		
		#endregion
		
		#region Fps Governor
		
		private FpsPresets DefaultFpsLimit;
		private FpsPresets PausedFpsLimit;
		private FpsPresets CurrentFpsLimit;
		private Int32 NextUpdate;
		
		public void InitializeFpsGovernor(FpsPresets defaultFpsLimit)
		{
			DefaultFpsLimit = defaultFpsLimit;
			PausedFpsLimit = FpsPresets.Max15Fps;
			NextUpdate = System.Environment.TickCount;
		}
		
		//TODO: Change so that if current update exceeds budget, delay is shortened or eliminated.
		public void FpsGovernor()
		{
			if (RunState != RunState.Paused)
				CurrentFpsLimit = CurrentMode.UseCustomFpsLimit ? CurrentMode.FpsLimit : DefaultFpsLimit;
			else
				CurrentFpsLimit = PausedFpsLimit;
			
			if (CurrentFpsLimit != FpsPresets.UnlimitedFps)
			{
				if (NextUpdate > System.Environment.TickCount)
					Thread.Sleep(NextUpdate - System.Environment.TickCount);
				NextUpdate = System.Environment.TickCount + (1000 / (Int32)CurrentFpsLimit);
			}
		}
		
		#endregion
		
		#region Timers
		
		//should be same as since LastUpdate except when paused.
		private Int32 LastLoopPass;
		public Int32 TicksSinceLastLoopPass { get; private set; }
		
		private Int32 LastUpdate;
		public Int32 TicksSinceLastUpdate { get; private set; }
		public Single TicksSinceLastUpdateF { get { return (Single)TicksSinceLastUpdate; } }
		
		private Int32 PauseLength;
		
		public void InitializeTimers()
		{
			LastLoopPass = System.Environment.TickCount - 1;
			TicksSinceLastLoopPass = 0;
			
			LastUpdate = LastLoopPass;
			TicksSinceLastUpdate = 0;
			
			PauseLength = 0;
		}
		
		public void RecalcTimers()
		{
			Int32 now = System.Environment.TickCount;
			
			TicksSinceLastLoopPass = now - LastLoopPass;
			LastLoopPass = now;
			
			if (RunState != RunState.Paused)
			{
				TicksSinceLastUpdate = now - LastUpdate;
				LastUpdate = now;
				
				//TODO: This will always return equivilant to TicksSinceLastLoopPass?
				if (PauseLength > 0)
				{
					TicksSinceLastUpdate -= PauseLength;
					PauseLength = 0;
				}
			}
			else
			{
				TicksSinceLastUpdate = 0;
				PauseLength += TicksSinceLastLoopPass;
			}
		}
		
		public void ResetPauseTimer()
		{
			PauseLength = 0;
		}
		
		#endregion
		
		#region Input
		
		//TODO: Add timers to record how long buttons have been held down for.
		
		private void InitializeInput()
		{
			CollectGamePadData = true;
			CollectTouchData = false;
		}
		
		private void CleanupInput()
		{
			CollectGamePadData = false;
			CollectTouchData = false;
		}
		
		public Boolean CollectGamePadData { get; private set; }
		public Boolean CollectTouchData { get; private set; }
		
		public GamePadData GamePadData { get; private set; }
		public List<TouchData> TouchData { get; private set; }
		
		public void RefreshInputData()
		{
			if (CollectGamePadData)
				GamePadData = GamePad.GetData(0);
			if (CollectTouchData)
				TouchData = Touch.GetData(0);
		}
		
		#region GamePad Button Shortcuts
		
		#region Up
		
		public Boolean GamePad0_Up_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Up) != 0); }
		}
		
		public Boolean GamePad0_Up
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Up) != 0); }
		}
		
		public Boolean GamePad0_Up_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Up) != 0); }
		}
		
		#endregion
		
		#region Down
		
		public Boolean GamePad0_Down_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Down) != 0); }
		}
		
		public Boolean GamePad0_Down
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Down) != 0); }
		}
		
		public Boolean GamePad0_Down_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Down) != 0); }
		}
		
		#endregion
		
		#region Left
		
		public Boolean GamePad0_Left_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Left) != 0); }
		}
		
		public Boolean GamePad0_Left
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Left) != 0); }
		}
		
		public Boolean GamePad0_Left_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Left) != 0); }
		}
		
		#endregion
		
		#region Right
		
		public Boolean GamePad0_Right_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Right) != 0); }
		}
		
		public Boolean GamePad0_Right
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Right) != 0); }
		}
		
		public Boolean GamePad0_Right_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Right) != 0); }
		}
		
		#endregion
		
		#region Cross
		
		public Boolean GamePad0_Cross_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Cross) != 0); }
		}
		
		public Boolean GamePad0_Cross
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Cross) != 0); }
		}
		
		public Boolean GamePad0_Cross_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Cross) != 0); }
		}
		
		#endregion
		
		#region Square
		
		public Boolean GamePad0_Square_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Square) != 0); }
		}
		
		public Boolean GamePad0_Square
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Square) != 0); }
		}
		
		public Boolean GamePad0_Square_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Square) != 0); }
		}
		
		#endregion
		
		#region Triangle
		
		public Boolean GamePad0_Triangle_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Triangle) != 0); }
		}
		
		public Boolean GamePad0_Triangle
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Triangle) != 0); }
		}
		
		public Boolean GamePad0_Triangle_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Triangle) != 0); }
		}
		
		#endregion
		
		#region Circle
		
		public Boolean GamePad0_Circle_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Circle) != 0); }
		}
		
		public Boolean GamePad0_Circle
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Circle) != 0); }
		}
		
		public Boolean GamePad0_Circle_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Circle) != 0); }
		}
		
		#endregion
		
		#region L1
		
		public Boolean GamePad0_L1_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.L) != 0); }
		}
		
		public Boolean GamePad0_L1
		{
			get { return ((GamePadData.Buttons & GamePadButtons.L) != 0); }
		}
		
		public Boolean GamePad0_L1_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.L) != 0); }
		}
		
		#endregion
		
		#region R1
		
		public Boolean GamePad0_R1_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.R) != 0); }
		}
		
		public Boolean GamePad0_R1
		{
			get { return ((GamePadData.Buttons & GamePadButtons.R) != 0); }
		}
		
		public Boolean GamePad0_R1_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.R) != 0); }
		}
		
		#endregion
		
		#region Start
		
		public Boolean GamePad0_Start_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Start) != 0); }
		}
		
		public Boolean GamePad0_Start
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Start) != 0); }
		}
		
		public Boolean GamePad0_Start_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Start) != 0); }
		}
		
		#endregion
		
		#region Select
		
		public Boolean GamePad0_Select_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Select) != 0); }
		}
		
		public Boolean GamePad0_Select
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Select) != 0); }
		}
		
		public Boolean GamePad0_Select_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Select) != 0); }
		}
		
		#endregion
		
		#region Back
		
		public Boolean GamePad0_Back_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Back) != 0); }
		}
		
		public Boolean GamePad0_Back
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Back) != 0); }
		}
		
		public Boolean GamePad0_Back_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Back) != 0); }
		}
		
		#endregion
		
		#region Enter
		
		public Boolean GamePad0_Enter_Pressed
		{
			get { return ((GamePadData.ButtonsDown & GamePadButtons.Enter) != 0); }
		}
		
		public Boolean GamePad0_Enter
		{
			get { return ((GamePadData.Buttons & GamePadButtons.Enter) != 0); }
		}
		
		public Boolean GamePad0_Enter_Released
		{
			get { return ((GamePadData.ButtonsUp & GamePadButtons.Enter) != 0); }
		}
		
		#endregion
		
		#endregion
		
		#endregion
		
		#region Modes
		
		public delegate ModeBase CreateModeDelegate(AppManager mgr);
		
		private CreateModeDelegate NextModeFactory;
		
		private const Int32 cMinTicksBetweenModeChanges = 100;
		private Int32 LastModeChange;
		
		public ModeBase PreviousMode { get; private set; }
		public ModeBase CurrentMode { get; private set; }
		public ModeBase ReturnMode { get; private set; }
		
		private void InitializeModes()
		{
			PreviousMode = null;
			CurrentMode = null;
			ReturnMode = null;
		}
		
		private void CleanupModes()
		{
			if (PreviousMode != null)
			{
				PreviousMode.Dispose();
				PreviousMode = null;
			}
			
			if (CurrentMode != null)
			{
				CurrentMode.Dispose();
				CurrentMode = null;
			}
			
			if (ReturnMode != null)
			{
				ReturnMode.Dispose();
				ReturnMode = null;
			}
		}
		
		public Boolean ModeChangeAllowed
		{
			get
			{
				return (System.Environment.TickCount - LastModeChange) > cMinTicksBetweenModeChanges;
			}
		}
		
		public Boolean ModeChanged
		{
			get { return PreviousMode != null; }
		}
		
		public void GoToMode(CreateModeDelegate factory)
		{
			LastModeChange = System.Environment.TickCount;
			PreviousMode = CurrentMode;
			NextModeFactory = factory;
			CurrentMode = null;
			ReturnMode = null;
		}
		
		//TODO: GoToThenReturn should not dispose of the original mode. perhaps as an option or another method.
		public void GoToModeThenReturn(CreateModeDelegate factory, ModeBase returnMode)
		{
			LastModeChange = System.Environment.TickCount;
			PreviousMode = CurrentMode;
			NextModeFactory = factory;
			CurrentMode = null;
			ReturnMode = returnMode;
		}
		
		public void ReturnToMode()
		{
			LastModeChange = System.Environment.TickCount;
			PreviousMode = CurrentMode;
			CurrentMode = ReturnMode;
			ReturnMode = null;
		}
		
		public void InitializeCurrentMode()
		{
			CurrentMode = NextModeFactory(this);
		}
		
		public void CleanupPreviousMode()
		{
			//PreviousMode.CleanupInternal();
			PreviousMode.Dispose();
			PreviousMode = null;
			
			//TODO: Re-enable this after Node finalizer is fixed!!!
			//if (!Debugger.IsAttached)
			GC.Collect();
		}
		
		#endregion
		
		#region Options
		
		public AppOptionsBase Options { get; private set; }
		
		private void InitializeOptions(AppOptionsBase options)
		{
			Options = options;
		}
		
		private void CleanupOptions()
		{
			Options.Dispose();
			Options = null;
		}
		
		#endregion
		
		#region Random Numbers
		
		private RandomGenerator _RandomGenerator = new RandomGenerator(System.Environment.TickCount);
		public RandomGenerator RandomGenerator { get { return _RandomGenerator; } }
		
		#endregion
	}
}

