using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Backflip Player State")]
	public class BackflipPlayerState : PlayerState
	{
		protected override void OnEnter(Player player) { }

		protected override void OnExit(Player player) { }

		protected override void OnStep(Player player)
		{
			player.Gravity(player.stats.current.backflipGravity);

			if (player.isGrounded)
			{
				player.lateralVelocity = Vector3.zero;
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
