using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[RequireComponent(typeof(Player))]
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/Player Particles")]
	public class PlayerParticles : MonoBehaviour
	{
		public float walkDustMinSpeed = 3.5f;
		public float landingParticleMinSpeed = 5f;

		public ParticleSystem walkDust;
		public ParticleSystem landDust;
		public ParticleSystem hurtDust;

		protected Player m_player;

		/// <summary>
		/// Start playing a given particle.
		/// </summary>
		/// <param name="particle">The particle you want to play.</param>
		public virtual void Play(ParticleSystem particle)
		{
			if (!particle.isPlaying)
			{
				particle.Play();
			}
		}

		/// <summary>
		/// Stop a given particle.
		/// </summary>
		/// <param name="particle">The particle you want to stop.</param>
		public virtual void Stop(ParticleSystem particle)
		{
			if (particle.isPlaying)
			{
				particle.Stop();
			}
		}

		protected virtual void HandleWalkParticle()
		{
			if (m_player.isGrounded && !m_player.onWater)
			{
				if (m_player.lateralVelocity.magnitude > walkDustMinSpeed)
				{
					Play(walkDust);
				}
				else
				{
					Stop(walkDust);
				}
			}
			else
			{
				Stop(walkDust);
			}
		}

		protected virtual void HandleLandParticle()
		{
			if (!m_player.onWater &&
				Mathf.Abs(m_player.velocity.y) >= landingParticleMinSpeed)
			{
				Play(landDust);
			}
		}

		protected virtual void HandleHurtParticle() => Play(hurtDust);

		protected virtual void Start()
		{
			m_player = GetComponent<Player>();
			m_player.OnGroundEnter.AddListener(HandleLandParticle);
			m_player.OnHurt.AddListener(HandleHurtParticle);
		}

		protected virtual void Update() => HandleWalkParticle();
	}
}
