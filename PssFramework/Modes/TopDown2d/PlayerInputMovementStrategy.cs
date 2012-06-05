using System;
using Sce.Pss.Core;

namespace PsmFramework.Modes.TopDown2d
{
	public class PlayerInputMovementStrategy : MovementStrategyBase
	{
		private Boolean UseLeftAnalog;
		private Boolean UseDPad;
		
		#region Constructor
		
		public PlayerInputMovementStrategy(Actor actor, Boolean useLeftAnalog, Boolean useDPad)
			: base(actor)
		{
			UseLeftAnalog = useLeftAnalog;
			UseDPad = useDPad;
		}
		
		#endregion
		
		#region Move
		
		public override void Move()
		{
			if (UseLeftAnalog)
				Move_LeftAnalog();
			if (UseDPad)
				Move_DPad();
		}
		
		private void Move_LeftAnalog()
		{
		}
		
		private void Move_DPad()
		{
			//TODO: Need 
			if (Mgr.GamePad0_Up)
				Actor.AddForce(Actor.Heading.Perpendicular().Multiply(3f));
			else if (Mgr.GamePad0_Down)
				Actor.AddForce(Actor.Heading.Perpendicular().Negate());
			
			if (Mgr.GamePad0_Left)
				Actor.AddRotationToHeading(0.0085f * Mgr.TicksSinceLastUpdate);
			else if (Mgr.GamePad0_Right)
				Actor.AddRotationToHeading(-0.0085f * Mgr.TicksSinceLastUpdate);
		}
		
		#endregion
	}
}

