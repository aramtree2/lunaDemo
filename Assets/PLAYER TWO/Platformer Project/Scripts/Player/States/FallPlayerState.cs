using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Fall Player State")]
	public class FallPlayerState : PlayerState
	{
		protected override void OnEnter(Player player) { }

		protected override void OnExit(Player player) { }

		protected override void OnStep(Player player)
		{
			player.Gravity();
			player.SnapToGround();
			player.Jump();
			player.Spin();
			player.PickAndThrow();
			player.AirDive();
			player.StompAttack();
			player.LedgeGrab();

			if (!player.isGrounded)
			{
				var inputDirection = player.inputs.GetMovementCameraDirection();

				if (inputDirection.sqrMagnitude > 0)
				{
					var dot = Vector3.Dot(inputDirection, player.lateralVelocity);

					if (dot >= 0)
					{
						player.Accelerate(inputDirection);
						player.FaceDirectionSmooth(player.lateralVelocity);
					}
					else
					{
						player.Decelerate();
					}
				}
			}
			else
			{
				player.states.Change<IdlePlayerState>();
			}
		}

		public override void OnContact(Player player, Collider other)
		{
			player.PushRigidbody(other);
			player.WallDrag(other);
			player.GrabPole(other);
		}
	}
}
