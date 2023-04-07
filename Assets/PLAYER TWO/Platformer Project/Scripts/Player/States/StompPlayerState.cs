using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Stomp Player State")]
	public class StompPlayerState : PlayerState
	{
		protected float m_airTimer;
		protected float m_groundTimer;
		protected bool landed;

		protected override void OnEnter(Player player)
		{
			landed = false;
			m_airTimer = m_groundTimer = 0;
			player.velocity = Vector3.zero;
			player.OnStompStarted?.Invoke();
		}

		protected override void OnExit(Player player) { }

		protected override void OnStep(Player player)
		{
			if (m_airTimer >= player.stats.current.stompAirTime)
			{
				player.verticalVelocity += Vector3.down * player.stats.current.stompDownwardForce;
			}
			else
			{
				m_airTimer += Time.deltaTime;
			}

			if (player.isGrounded)
			{
				if (!landed)
				{
					landed = true;
					player.OnStompLanding?.Invoke();
				}

				if (m_groundTimer >= player.stats.current.stompGroundTime)
				{
					player.verticalVelocity = Vector3.up * player.stats.current.stompGroundLeapHeight;
					player.states.Change<FallPlayerState>();
				}
				else
				{
					m_groundTimer += Time.deltaTime;
				}
			}
		}

		public override void OnContact(Player player, Collider other) { }
	}
}
