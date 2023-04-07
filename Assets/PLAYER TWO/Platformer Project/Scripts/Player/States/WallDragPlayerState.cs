using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Wall Drag Player State")]
	public class WallDragPlayerState : PlayerState
	{
		protected override void OnEnter(Player player)
		{
			player.velocity = Vector3.zero;
			var direction = player.lastWallNormal;
			direction = new Vector3(direction.x, 0, direction.z).normalized;
			player.FaceDirection(direction);
			player.skin.position += transform.rotation * player.stats.current.wallDragSkinOffset;
		}

		protected override void OnExit(Player player)
		{
			player.skin.position -= transform.rotation * player.stats.current.wallDragSkinOffset;
		}

		protected override void OnStep(Player player)
		{
			player.verticalVelocity += Vector3.down * player.stats.current.wallDragGravity * Time.deltaTime;

			if (player.isGrounded || !player.CapsuleCast(-transform.forward, player.radius))
			{
				player.states.Change<IdlePlayerState>();
			}
			else if (player.inputs.GetJumpDown())
			{
				player.DirectionalJump(player.transform.forward, player.stats.current.wallJumpHeight, player.stats.current.wallJumpDistance);
				player.states.Change<FallPlayerState>();
			}
		}

		public override void OnContact(Player player, Collider other) { }
	}
}
