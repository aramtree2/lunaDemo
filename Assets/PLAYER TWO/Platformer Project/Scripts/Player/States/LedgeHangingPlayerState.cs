using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Ledge Hanging Player State")]
	public class LedgeHangingPlayerState : PlayerState
	{
		protected bool m_keepParent;

		protected override void OnEnter(Player player)
		{
			m_keepParent = false;
			player.skin.position += transform.rotation * player.stats.current.ledgeHangingSkinOffset;
		}

		protected override void OnExit(Player player)
		{
			if (!m_keepParent)
			{
				transform.parent = null;
			}

			player.skin.position -= transform.rotation * player.stats.current.ledgeHangingSkinOffset;
		}

		protected override void OnStep(Player player)
		{
			var ledgeTopMaxDistance = player.radius + player.stats.current.ledgeMaxForwardDistance;
			var ledgeTopHeightOffset = player.height * 0.5f + player.stats.current.ledgeMaxDownwardDistance;
			var topOrigin = player.position + Vector3.up * ledgeTopHeightOffset + transform.forward * ledgeTopMaxDistance;
			var sideOrigin = player.position + Vector3.up * player.height * 0.5f + Vector3.down * player.stats.current.ledgeSideHeightOffset;
			var rayDistance = player.radius + player.stats.current.ledgeSideMaxDistance;
			var rayRadius = player.stats.current.ledgeSideCollisionRadius;

			if (Physics.SphereCast(sideOrigin, rayRadius, transform.forward, out var sideHit,
				rayDistance, player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore) &&
				Physics.Raycast(topOrigin, Vector3.down, out var topHit, player.height,
				player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore))
			{
				var inputDirection = player.inputs.GetMovementDirection();
				var ledgeSideOrigin = sideOrigin + transform.right * Mathf.Sign(inputDirection.x) * player.radius;
				var ledgeHeight = topHit.point.y - player.height * 0.5f;
				var sideForward = -new Vector3(sideHit.normal.x, 0, sideHit.normal.z).normalized;

				player.FaceDirection(sideForward);

				if (Physics.Raycast(ledgeSideOrigin, sideForward, rayDistance,
					player.stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore))
				{
					player.lateralVelocity = transform.right * inputDirection.x * player.stats.current.ledgeMovementSpeed;
				}
				else
				{
					player.lateralVelocity = Vector3.zero;
				}

				transform.position = new Vector3(sideHit.point.x, ledgeHeight, sideHit.point.z) - sideForward * player.radius - player.center;

				if (player.inputs.GetReleaseLedgeDown())
				{
					player.FaceDirection(-sideForward);
					player.states.Change<FallPlayerState>();
				}
				else if (player.inputs.GetJumpDown())
				{
					player.Jump(player.stats.current.maxJumpHeight);
					player.states.Change<FallPlayerState>();
				}
				else if (inputDirection.z > 0 && player.stats.current.canClimbLedges &&
						((1 << topHit.collider.gameObject.layer) & player.stats.current.ledgeClimbingLayers) != 0)
				{
					m_keepParent = true;
					player.states.Change<LedgeClimbingPlayerState>();
					player.OnLedgeClimbing?.Invoke();
				}
			}
			else
			{
				player.states.Change<FallPlayerState>();
			}
		}

		public override void OnContact(Player player, Collider other) { }
	}
}
