using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sce.Pss.Core;
using Sce.Pss.HighLevel.GameEngine2D;
using Sce.Pss.HighLevel.GameEngine2D.Base;

namespace PssFramework.Modes.TopDown2d
{
	//TODO: Add default Room friction
	//TODO: Add Tile visibility to support fog of war
	//TODO: Set camera to a specified point when entering room.
	
	public abstract class RoomBase : IDisposable
	{
		public LevelBase Level { get; private set; }
		public TopDown2dModeBase Mode { get { return Level.Mode; } }
		public AppManager Mgr { get { return Level.Mode.Mgr; } }
		
		#region Constructor, Dispose
		
		protected RoomBase(LevelBase level)
		{
			if (level == null)
				throw new ArgumentNullException();
			
			Level = level;
			
			InitializeInternal();
		}
		
		public void Dispose()
		{
			CleanupInternal();
			
			Level = null;
		}
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal()
		{
			//Sprites first
			InitializeSpritesInScene();
			
			InitializeBackground();
			InitializeCamera();
			InitializeActors();
			
			Initialize();
		}
		
		private void CleanupInternal()
		{
			Cleanup();
			
			CleanupBackground();
			CleanupCamera();
			CleanupActors();
			
			//Sprites Last
			CleanupSpritesInScene();
		}
		
		#endregion
		
		#region Update
		
		internal void UpdateInternal()
		{
			Update();
			Actors.ForEach(a => {a.Update(); } );
			UpdateCamera();
			UpdateBackground();
		}
		
		#endregion
		
		#region Room Logic
		
		/// <summary>
		/// Create actors and position them in the room here.
		/// </summary>
		public abstract void Initialize();
		
		public abstract void Cleanup();
		
		/// <summary>
		/// Only change room-specific stuff here. Actors are updated elsewhere.
		/// </summary>
		public abstract void Update();
		
		#endregion
		
		#region Camera
		
		private Vector2 CameraPstn;
		
		private Actor CameraSubject;
		
		private Int32 CameraMinX;
		private Int32 CameraMaxX;
		private Int32 CameraMinY;
		private Int32 CameraMaxY;
		
		private void InitializeCamera()
		{
			CameraMinX = Convert.ToInt32(Mgr.ScreenWidth / 2);
			CameraMinY = Convert.ToInt32(Mgr.ScreenHeight / 2);
			
			CameraMaxX = Convert.ToInt32(Background.Width - (Mgr.ScreenWidth / 2));
			CameraMaxY = Convert.ToInt32(Background.Height - (Mgr.ScreenHeight / 2));
			
			if (CameraMaxX < CameraMinX)
				CameraMaxX = CameraMinX;
			if (CameraMaxY < CameraMinY)
				CameraMaxY = CameraMinY;
		}
		
		private void CleanupCamera()
		{
			CameraSubject = null;
		}
		
		private void UpdateCamera()
		{
			if (CameraSubject != null)
				CameraPstn = CameraSubject.CameraPostion;
			
			if (CameraPstn.X < CameraMinX)
				CameraPstn.X = CameraMinX;
			else if (CameraPstn.X > CameraMaxX)
				CameraPstn.X = CameraMaxX;
			
			if (CameraPstn.Y < CameraMinY)
				CameraPstn.Y = CameraMinY;
			else if (CameraPstn.Y > CameraMaxY)
				CameraPstn.Y = CameraMaxY;
			
			Mode.SetCamera(CameraPstn);
		}
		
		public void SetCameraSubject(Actor actor)
		{
			CameraSubject = actor;
		}
		
		public void ClearCameraSubject()
		{
			CameraSubject = null;
		}
		
		#endregion
		
		#region Sprites
		
		private List<SpriteList> SpriteListsInScene;
		private List<RawSpriteTileArray> RawSpriteTileListsInScene;
		private List<SpriteUV> SpritesInScene;
		
		private void InitializeSpritesInScene()
		{
			SpriteListsInScene = new List<SpriteList>();
			RawSpriteTileListsInScene = new List<RawSpriteTileArray>();
			SpritesInScene = new List<SpriteUV>();
		}
		
		private void CleanupSpritesInScene()
		{
			RemoveAllFromScene();
			
			SpriteListsInScene.Clear();
			SpriteListsInScene = null;
			
			RawSpriteTileListsInScene.Clear();
			RawSpriteTileListsInScene = null;
			
			SpritesInScene.Clear();
			SpritesInScene = null;
			
			Mode.TextureManager.RemoveAllTexturesForUser(this);
		}
		
		public void AddToScene(SpriteList spriteList, Int32 order = 0)
		{
			SpriteListsInScene.Add(spriteList);
			Mode.AddToScene(spriteList, order);
		}
		
		public void AddToScene(RawSpriteTileArray spriteList, Int32 order = 0)
		{
			RawSpriteTileListsInScene.Add(spriteList);
			Mode.AddToScene(spriteList, order);
		}
		
		public void AddToScene(SpriteUV sprite, Int32 order = 0)
		{
			SpritesInScene.Add(sprite);
			Mode.AddToScene(sprite, order);
		}
		
		public void RemoveFromScene(SpriteList spriteList)
		{
			SpriteListsInScene.Remove(spriteList);
			Mode.RemoveFromScene(spriteList);
		}
		
		public void RemoveFromScene(RawSpriteTileArray spriteList)
		{
			RawSpriteTileListsInScene.Remove(spriteList);
			Mode.RemoveFromScene(spriteList);
		}
		
		public void RemoveFromScene(SpriteUV sprite)
		{
			SpritesInScene.Remove(sprite);
			Mode.RemoveFromScene(sprite);
		}
		
		private void RemoveAllFromScene()
		{
			SpriteListsInScene.ForEach(s => { Mode.RemoveFromScene(s); });
			SpriteListsInScene.Clear();
			SpritesInScene.ForEach(s => { Mode.RemoveFromScene(s); });
			SpritesInScene.Clear();
		}
		
		#endregion
		
		#region Background
		
		protected Background Background;
		
		private void InitializeBackground()
		{
			Background = new Background(this, BackgroundTileColumns, BackgroundTileRows, BackgroundAsset, BackgroundAssetColumns, BackgroundAssetRows, BackgroundAssetTileWidth, BackgroundAssetTileHeight);
		}
		
		private void CleanupBackground()
		{
			Background.Dispose();
			Background = null;
		}
		
		private void UpdateBackground()
		{
			Background.Update();
		}
		
		protected abstract String BackgroundAsset { get; }
		protected abstract Int32 BackgroundAssetColumns { get; }
		protected abstract Int32 BackgroundAssetRows { get; }
		protected abstract Int32 BackgroundAssetTileWidth { get; }
		protected abstract Int32 BackgroundAssetTileHeight { get; }
		
		protected abstract Int32 BackgroundTileColumns { get; }
		protected abstract Int32 BackgroundTileRows { get; }
		
		/// <summary>
		/// This method is used by the room to populate the data about individual tiles.
		/// For instance, it could say tile 1,1 is a water tile with 2f friction.
		/// Data might be loaded from an Xml file or hardcoded.
		/// </summary>
		public abstract BackgroundTileData GetBackgroundTileData(Int32 column, Int32 row);
		
		#endregion
		
		#region Actors
		
		public List<Actor> Actors { get; private set; }
		
		private void InitializeActors()
		{
			Actors = new List<Actor>();
		}
		
		private void CleanupActors()
		{
			foreach (Actor actor in Actors)
			{
				RemoveFromScene(actor.Sprite);
				actor.Dispose();
			}
			
			Actors.Clear();
			Actors = null;
		}
		
		protected void AddActor(Actor actor)
		{
			if (actor == null)
				throw new ArgumentNullException();
			
			if (Actors.Contains(actor))
				throw new ArgumentException();
			
			Actors.Add(actor);
			Mode.AddToScene(actor.Sprite);
		}
		
		protected void RemoveActor(Actor actor)
		{
			if (actor == null)
				throw new ArgumentNullException();
			
			if (!Actors.Contains(actor))
				throw new ArgumentException();
			
			Actors.Remove(actor);
			Mode.RemoveFromScene(actor.Sprite);
		}
		
		#endregion
		
		#region Debug
		
		internal void GetDebugInfo(StringBuilder sb)
		{
			Vector2i cameraTile = Background.GetTileFromRoomPostion(CameraPstn);
			sb.Append("Camera Tile: ");
			sb.AppendLine(cameraTile.ToString());
			
			Vector2i actorTile = Background.GetTileFromRoomPostion(CameraSubject.Position);
			sb.Append("Actor Tile: ");
			sb.AppendLine(actorTile.ToString());
			
			Background.GetDebugInfo(sb);
		}
		
		#endregion
	}
}

