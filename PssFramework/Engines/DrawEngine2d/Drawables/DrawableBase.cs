using System;

namespace PsmFramework.Engines.DrawEngine2d.Drawables
{
	public abstract class DrawableBase : IDisposablePlus
	{
		#region Constructor, Dispose
		
		public DrawableBase(LayerBase layer)
		{
			InitializeInternal(layer);
			Initialize();
		}
		
		public void Dispose()
		{
			Cleanup();
			CleanupInternal();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void InitializeInternal(LayerBase layer)
		{
			InitializeLayer(layer);
			InitializeDrawEngine2d();
			InitializeChanged();
			InitializeVisibility();
		}
		
		private void CleanupInternal()
		{
			CleanupVisibility();
			CleanupChanged();
			CleanupDrawEngine2d();
			CleanupLayer();
		}
		
		protected virtual void Initialize()
		{
		}
		
		protected virtual void Cleanup()
		{
		}
		
		#endregion
		
		#region Layer
		
		private void InitializeLayer(LayerBase layer)
		{
			Layer = layer;
			Layer.AddDrawable(this);
		}
		
		private void CleanupLayer()
		{
			Layer.RemoveDrawable(this);
			Layer = null;
		}
		
		public LayerBase Layer;
		
		#endregion
		
		#region DrawEngine2d
		
		private void InitializeDrawEngine2d()
		{
			DrawEngine2d = Layer.DrawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d = null;
		}
		
		public DrawEngine2d DrawEngine2d { get; private set; }
		
		#endregion
		
		#region Render
		
		public abstract void Render();
		
		#endregion
		
		#region Changed
		
		private void InitializeChanged()
		{
			//TODO: force changed here?
		}
		
		private void CleanupChanged()
		{
		}
		
		private Boolean _Changed;
		protected Boolean Changed
		{
			get { return _Changed; }
			private set
			{
				if (_Changed == value)
					return;
				
				_Changed = value;
				
				if(_Changed)
					DrawEngine2d.SetRenderRequired();
				
				ChangedHelper();
			}
		}
		
		protected void MarkAsChanged()
		{
			Changed = true;
		}
		
		protected void ClearChanged()
		{
			Changed = false;
		}
		
		//Poor-man's OnChanged event.
		protected virtual void ChangedHelper()
		{
		}
		
		#endregion
		
		#region Visibility
		
		private void InitializeVisibility()
		{
			Visible = true;
		}
		
		private void CleanupVisibility()
		{
		}
		
		private Boolean _Visible;
		public Boolean Visible
		{
			get { return _Visible; }
			set
			{
				if (_Visible == value)
					return;
				
				_Visible = value;
				MarkAsChanged();
			}
		}
		
		#endregion
	}
}

