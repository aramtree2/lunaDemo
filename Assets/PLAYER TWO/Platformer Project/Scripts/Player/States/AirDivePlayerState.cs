using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Air Dive Player State")]
	public class AirDivePlayerState : PlayerState
	{
		protected override void OnEnter(Player player)
		{
			player.verticalVelocity = Vector3.zero;
			player.lateralVelocity = player.transform.forward * player.stats.current.airDiveForwardForce;
		}

		protected override void OnExit(Player player) { }

		protected override void OnStep(Player player)
		{
			player.Gravity();
			player.Jump();

			if (player.isGrounded)
			{
				player.Decelerate(player.stats.current.airDiveGroundDeceleration);

				if (player.lateralVelocity.sqrMagnitude == 0)
				{
					player.verticalVelocity = Vector3.up * player.stats.current.airDiveGroundLeapHeight;
					player.states.Change<FallPlayerState>();
				}
			}
		}

		public override void OnContact(Player player, Collider other)
		{
			if (!player.isGrounded)
			{
				player.WallDrag(other);
				player.GrabPole(other);
			}
		}
	}
}
