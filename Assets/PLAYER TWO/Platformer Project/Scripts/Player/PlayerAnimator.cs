using System.Collections.Generic;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[RequireComponent(typeof(Player))]
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/Player Animator")]
	public class PlayerAnimator : MonoBehaviour
	{
		[System.Serializable]
		public class ForcedTransition
		{
			public int fromStateId;
			public int animationLayer;
			public string toAnimationClip;
		}

		public Animator animator;

		[Header("Parameters Names")]
		public string stateName = "State";
		public string lastStateName = "Last State";
		public string lateralSpeedName = "Lateral Speed";
		public string verticalSpeedName = "Vertical Speed";
		public string lateralAnimationSpeedName = "Lateral Animation Speed";
		public string isGroundedName = "Is Grounded";
		public string isHoldingName = "Is Holding";

		[Header("Settings")]
		public float minLateralAnimationSpeed = 0.5f;
		public List<ForcedTransition> forcedTransitions;

		protected int m_stateHash;
		protected int m_lastStateHash;
		protected int m_lateralSpeedHash;
		protected int m_verticalSpeedHash;
		protected int m_lateralAnimationSpeedHash;
		protected int m_isGroundedHash;
		protected int m_isHoldingHash;

		protected Dictionary<int, ForcedTransition> m_forcedTransitions;

		protected Player m_player;

		protected virtual void InitializePlayer()
		{
			m_player = GetComponent<Player>();
			m_player.states.onChange.AddListener(HandleForcedTransitions);
		}

		protected virtual void InitializeForcedTransitions()
		{
			m_forcedTransitions = new Dictionary<int, ForcedTransition>();

			foreach (var transition in forcedTransitions)
			{
				if (!m_forcedTransitions.ContainsKey(transition.fromStateId))
				{
					m_forcedTransitions.Add(transition.fromStateId, transition);
				}
			}
		}

		protected virtual void ReadParametersHash()
		{
			m_stateHash = Animator.StringToHash(stateName);
			m_lastStateHash = Animator.StringToHash(lastStateName);
			m_lateralSpeedHash = Animator.StringToHash(lateralSpeedName);
			m_verticalSpeedHash = Animator.StringToHash(verticalSpeedName);
			m_lateralAnimationSpeedHash = Animator.StringToHash(lateralAnimationSpeedName);
			m_isGroundedHash = Animator.StringToHash(isGroundedName);
			m_isHoldingHash = Animator.StringToHash(isHoldingName);
		}

		protected virtual void HandleForcedTransitions()
		{
			var lastStateIndex = m_player.states.lastIndex;

			if (m_forcedTransitions.ContainsKey(lastStateIndex))
			{
				var layer = m_forcedTransitions[lastStateIndex].animationLayer;
				animator.Play(m_forcedTransitions[lastStateIndex].toAnimationClip, layer);
			}
		}

		protected virtual void HandleAnimatorParameters()
		{
			var lateralSpeed = m_player.lateralVelocity.magnitude;
			var verticalSpeed = m_player.verticalVelocity.y;
			var lateralAnimationSpeed = Mathf.Max(minLateralAnimationSpeed, lateralSpeed / m_player.stats.current.topSpeed);

			animator.SetInteger(m_stateHash, m_player.states.index);
			animator.SetInteger(m_lastStateHash, m_player.states.lastIndex);
			animator.SetFloat(m_lateralSpeedHash, lateralSpeed);
			animator.SetFloat(m_verticalSpeedHash, verticalSpeed);
			animator.SetFloat(m_lateralAnimationSpeedHash, lateralAnimationSpeed);
			animator.SetBool(m_isGroundedHash, m_player.isGrounded);
			animator.SetBool(m_isHoldingHash, m_player.holding);
		}

		protected virtual void Start()
		{
			InitializePlayer();
			InitializeForcedTransitions();
			ReadParametersHash();
		}

		protected virtual void LateUpdate() => HandleAnimatorParameters();
	}
}
