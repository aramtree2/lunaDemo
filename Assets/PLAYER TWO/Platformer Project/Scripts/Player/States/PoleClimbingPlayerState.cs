using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Pole Climbing Player State")]
	public class PoleClimbingPlayerState : PlayerState
	{
		protected float m_collisionRadius;

		protected const float k_poleOffset = 0.01f;

		protected override void OnEnter(Player player)
		{
			player.velocity = Vector3.zero;
			player.pole.GetDirectionToPole(transform, out m_collisionRadius);
			player.skin.position += transform.rotation * player.stats.current.poleClimbSkinOffset;
		}

		protected override void OnExit(Player player)
		{
			player.skin.position -= transform.rotation * player.stats.current.poleClimbSkinOffset;
		}

		protected override void OnStep(Player player)
		{
			var poleDirection = player.pole.GetDirectionToPole(transform);
			var inputDirection = player.inputs.GetMovementDirection();

			player.FaceDirection(poleDirection);
			player.lateralVelocity = transform.right * inputDirection.x * player.stats.current.climbRotationSpeed;

			if (inputDirection.z != 0)
			{
				var speed = inputDirection.z > 0 ? player.stats.current.climbUpSpeed : -player.stats.current.climbDownSpeed;
				player.verticalVelocity = Vector3.up * speed;
			}
			else
			{
				player.verticalVelocity = Vector3.zero;
			}

			if (player.inputs.GetJumpDown())
			{
				player.FaceDirection(-poleDirection);
				player.DirectionalJump(-poleDirection, player.stats.current.poleJumpHeight, player.stats.current.poleJumpDistance);
				player.states.Change<FallPlayerState>();
			}

			if (player.isGrounded)
			{
				player.states.Change<IdlePlayerState>();
			}

			var offset = player.height * 0.5f + player.center.y;
			var center = new Vector3(player.pole.center.x, transform.position.y, player.pole.center.z);
			var position = center - poleDirection * m_collisionRadius;

			transform.position = player.pole.ClampPointToPoleHeight(position, offset);
		}

		public override void OnContact(Player player, Collider other) { }
	}
}
