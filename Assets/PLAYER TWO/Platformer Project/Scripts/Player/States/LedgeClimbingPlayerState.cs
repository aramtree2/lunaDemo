using System.Collections;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Ledge Climbing Player State")]
	public class LedgeClimbingPlayerState : PlayerState
	{
		protected override void OnEnter(Player player) => StartCoroutine(SetPositionRoutine(player));

		protected override void OnExit(Player player)
		{
			player.ResetSkinParent();
			StopAllCoroutines();
		}

		protected override void OnStep(Player player) { }

		protected virtual IEnumerator SetPositionRoutine(Player player)
		{
			var elapsedTime = 0f;
			var totalDuration = player.stats.current.ledgeClimbingDuration;
			var halfDuration = totalDuration / 2f;

			var initialPosition = transform.localPosition;
			var targetVerticalPosition = transform.position + Vector3.up * (player.height + Physics.defaultContactOffset);
			var targetLateralPosition = targetVerticalPosition + transform.forward * player.radius * 2f;

			if (transform.parent != null)
			{
				targetVerticalPosition = transform.parent.InverseTransformPoint(targetVerticalPosition);
				targetLateralPosition = transform.parent.InverseTransformPoint(targetLateralPosition);
			}

			player.SetSkinParent(transform.parent);
			player.skin.position += transform.rotation * player.stats.current.ledgeClimbingSkinOffset;

			while (elapsedTime <= halfDuration)
			{
				elapsedTime += Time.deltaTime;
				transform.localPosition = Vector3.Lerp(initialPosition, targetVerticalPosition, elapsedTime / halfDuration);
				yield return null;
			}

			elapsedTime = 0;
			transform.localPosition = targetVerticalPosition;

			while (elapsedTime <= halfDuration)
			{
				elapsedTime += Time.deltaTime;
				transform.localPosition = Vector3.Lerp(targetVerticalPosition, targetLateralPosition, elapsedTime / halfDuration);
				yield return null;
			}

			transform.localPosition = targetLateralPosition;
			player.states.Change<IdlePlayerState>();
		}

		public override void OnContact(Player player, Collider other) { }
	}
}
