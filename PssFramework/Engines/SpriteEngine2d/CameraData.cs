using System;

namespace PssFramework.SpriteEngine
{
	/// Pack some internal bits in a struct so we can copy easily
	internal struct CameraData
	{
		/// The support vector is the 2D world vector that maps to "from center of screen to middle of right screen edge"
		/// (or "to the middle of the top screen edge" if m_support_is_y is set to true). It is decomposed into
		/// a unit vector component 'm_support_unit_vec' and its len 'm_support_scale'.
		
		internal Vector2 m_support_unit_vec; 
		internal float m_support_scale;
		internal bool m_support_is_y;
		internal Vector2 m_center; // world coordinates of the screen center (view center)
		internal float m_aspect;
		internal float m_znear;
		internal float m_zfar;
	}
}

