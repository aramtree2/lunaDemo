using UnityEngine;
using UnityEngine.Events;

namespace PLAYERTWO.PlatformerProject
{
	[RequireComponent(typeof(PlayerInputManager))]
	[RequireComponent(typeof(PlayerStatsManager))]
	[RequireComponent(typeof(PlayerStateManager))]
	[RequireComponent(typeof(Health))]
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/Player")]
	public class Player : Entity<Player>
	{
		/// <summary>
		/// Called when the Player jumps.
		/// </summary>
		public UnityEvent OnJump;

		/// <summary>
		/// Called when the Player gets damage.
		/// </summary>
		public UnityEvent OnHurt;

		/// <summary>
		/// Called when the Player died.
		/// </summary>
		public UnityEvent OnDie;

		/// <summary>
		/// Called when the Player uses the Spin Attack.
		/// </summary>
		public UnityEvent OnSpin;

		/// <summary>
		/// Called when the Player pick up an object.
		/// </summary>
		public UnityEvent OnPickUp;

		/// <summary>
		/// Called when the Player throws an object.
		/// </summary>
		public UnityEvent OnThrow;

		/// <summary>
		/// Called when the Player started the Stomp Attack.
		/// </summary>
		public UnityEvent OnStompStarted;

		/// <summary>
		/// Called when the Player landed from the Stomp Attack.
		/// </summary>
		public UnityEvent OnStompLanding;

		/// <summary>
		/// Called when the player grabs onto a ledge.
		/// </summary>
		public UnityEvent OnLedgeGrabbed;

		/// <summary>
		/// Called when the Player climbs a ledge.
		/// </summary>
		public UnityEvent OnLedgeClimbing;

		/// <summary>
		/// Called when the Player air dives.
		/// </summary>
		public UnityEvent OnAirDive;

		/// <summary>
		/// Called when the Player performs a backflip.
		/// </summary>
		public UnityEvent OnBackflip;

		public Transform pickableSlot;
		public Transform skin;

		private Vector3 m_respawnPosition;
		private Quaternion m_respawnRotation;

		private Collider[] m_attackSphereOverlaps = new Collider[10];

		protected Vector3 m_skinInitialPosition;
		protected Quaternion m_skinInitialRotation;

		/// <summary>
		/// Returns the Player Input Manager instance.
		/// </summary>
		public PlayerInputManager inputs { get; private set; }

		/// <summary>
		/// Returns the Player Stats Manager instance.
		/// </summary>
		public PlayerStatsManager stats { get; private set; }

		/// <summary>
		/// Returns the Health instance.
		/// </summary>
		public Health health { get; private set; }

		/// <summary>
		/// Returns true if the Player is on water.
		/// </summary>
		public bool onWater { get; protected set; }

		/// <summary>
		/// Returns true if the Player is holding an object.
		/// </summary>
		public bool holding { get; protected set; }

		/// <summary>
		/// Returns how many times the Player jumped.
		/// </summary>
		public int jumpCounter { get; protected set; }

		/// <summary>
		/// Returns how many times the Player performed an air spin.
		/// </summary>
		public int airSpinCounter { get; protected set; }

		/// <summary>
		/// Returns the normal of the last wall the Player touched.
		/// </summary>
		public Vector3 lastWallNormal { get; protected set; }

		/// <summary>
		/// Returns the Pole instance in which the Player is colliding with.
		/// </summary>
		public Pole pole { get; protected set; }

		/// <summary>
		/// Returns the Collider of the water the Player is swimming.
		/// </summary>
		public Collider water { get; protected set; }

		/// <summary>
		/// Return the Pickable instance which the Player is holding.
		/// </summary>
		public Pickable pickable { get; protected set; }

		/// <summary>
		/// Returns true if the Player health is not empty.
		/// </summary>
		public virtual bool isAlive => !health.isEmpty;

		/// <summary>
		/// Returns true if the Player can stand up.
		/// </summary>
		public virtual bool canStandUp => !SphereCast(Vector3.up, originalHeight * 0.5f);

		private const float k_waterExitOffset = 0.25f;

		protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
		protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();
		protected virtual void InitializeHealth() => health = GetComponent<Health>();
		protected virtual void InitializeTag() => tag = GameTags.Player;

		protected virtual void InitializeRespawn()
		{
			m_respawnPosition = transform.position;
			m_respawnRotation = transform.rotation;
		}

		protected virtual void InitializeSkin()
		{
			if (skin)
			{
				m_skinInitialPosition = skin.localPosition;
				m_skinInitialRotation = skin.localRotation;
			}
		}

		/// <summary>
		/// Resets Player state, health, position, and rotation.
		/// </summary>
		public virtual void Respawn()
		{
			health.Reset();
			transform.SetPositionAndRotation(m_respawnPosition, m_respawnRotation);
			states.Change<IdlePlayerState>();
		}

		/// <summary>
		/// Sets the position and rotation of the Player for the next respawn.
		/// </summary>
		public virtual void SetRespawn(Vector3 position, Quaternion rotation)
		{
			m_respawnPosition = position;
			m_respawnRotation = rotation;
		}

		/// <summary>
		/// Applies damage to this Player decreasing its health with proper reaction.
		/// </summary>
		/// <param name="amount">The amount of health you want to decrease.</param>
		public virtual void ApplyDamage(int amount)
		{
			if (!health.isEmpty && !health.recovering)
			{
				health.Damage(amount);
				lateralVelocity = -transform.forward * stats.current.hurtBackwardsForce;

				if (!onWater)
				{
					verticalVelocity = Vector3.up * stats.current.hurtUpwardForce;
					states.Change<HurtPlayerState>();
				}

				OnHurt?.Invoke();

				if (health.isEmpty)
				{
					Throw();
					OnDie?.Invoke();
				}
			}
		}

		/// <summary>
		/// Kills the Player.
		/// </summary>
		public virtual void Die()
		{
			health.Set(0);
			OnDie?.Invoke();
		}

		/// <summary>
		/// Makes the Player transition to the Swim State.
		/// </summary>
		/// <param name="water">The instance of the water collider.</param>
		public virtual void EnterWater(Collider water)
		{
			if (!onWater && !health.isEmpty)
			{
				Throw();
				onWater = true;
				this.water = water;
				states.Change<SwimPlayerState>();
			}
		}

		/// <summary>
		/// Makes the Player exit the current water instance.
		/// </summary>
		public virtual void ExitWater()
		{
			if (onWater)
			{
				onWater = false;
			}
		}

		/// <summary>
		/// Attaches the Player to a given Pole.
		/// </summary>
		/// <param name="pole">The Pole you want to attach the Player to.</param>
		public virtual void GrabPole(Collider other)
		{
			if (stats.current.canPoleClimb && velocity.y <= 0
				&& !holding && other.TryGetComponent(out Pole pole))
			{
				this.pole = pole;
				states.Change<PoleClimbingPlayerState>();
			}
		}

		protected override bool EvaluateLanding(RaycastHit hit)
		{
			return base.EvaluateLanding(hit) && !hit.collider.CompareTag(GameTags.Spring);
		}

		protected override void HandleSlopeLimit(RaycastHit hit)
		{
			var slopeDirection = Vector3.Cross(hit.normal, Vector3.Cross(hit.normal, Vector3.up));
			slopeDirection = slopeDirection.normalized;
			controller.Move(slopeDirection * stats.current.slideForce * Time.deltaTime);
		}

		protected override void HandleHighLedge(RaycastHit hit)
		{
			var edgeNormal = hit.point - position;
			var edgePushDirection = Vector3.Cross(edgeNormal, Vector3.Cross(edgeNormal, Vector3.up));
			controller.Move(edgePushDirection * stats.current.gravity * Time.deltaTime);
		}

		/// <summary>
		/// Moves the Player smoothly in a given direction.
		/// </summary>
		/// <param name="direction">The direction you want to move.</param>
		public virtual void Accelerate(Vector3 direction)
		{
			var turningDrag = isGrounded && inputs.GetRun() ? stats.current.runningTurningDrag : stats.current.turningDrag;
			var acceleration = isGrounded && inputs.GetRun() ? stats.current.runningAcceleration : stats.current.acceleration;
			var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;
			var topSpeed = inputs.GetRun() ? stats.current.runningTopSpeed : stats.current.topSpeed;

			Accelerate(direction, turningDrag, finalAcceleration, topSpeed);

			if (inputs.GetRunUp())
			{
				lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
			}
		}

		/// <summary>
		/// Moves the Player smoothly in a given direction with water stats.
		/// </summary>
		/// <param name="direction">The direction you want to move.</param>
		public virtual void WaterAcceleration(Vector3 direction) =>
			Accelerate(direction, stats.current.waterTurningDrag, stats.current.swimAcceleration, stats.current.swimTopSpeed);

		/// <summary>
		/// Moves the Player smoothly in a given direction with crawling stats.
		/// </summary>
		/// <param name="direction">The direction you want to move.</param>
		public virtual void CrawlingAccelerate(Vector3 direction) =>
			Accelerate(direction, stats.current.crawlingTurningSpeed, stats.current.crawlingAcceleration, stats.current.crawlingTopSpeed);

		/// <summary>
		/// Smoothly sets Lateral Velocity to zero by its deceleration stats.
		/// </summary>
		public virtual void Decelerate() => Decelerate(stats.current.deceleration);

		/// <summary>
		/// Smoothly sets Lateral Velocity to zero by its friction stats.
		/// </summary>
		public virtual void Friction() => Decelerate(stats.current.friction);

		/// <summary>
		/// Applies a downward force by its gravity stats.
		/// </summary>
		public virtual void Gravity()
		{
			if (!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
			{
				var speed = verticalVelocity.y;
				var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;
				speed -= force * Time.deltaTime;
				speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);
				verticalVelocity = new Vector3(0, speed, 0);
			}
		}

		/// <summary>
		/// Applies a downward force when ground by its snap stats.
		/// </summary>
		public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

		/// <summary>
		/// Rotate the Player forward to a given direction.
		/// </summary>
		/// <param name="direction">The direction you want it to face.</param>
		public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirection(direction, stats.current.rotationSpeed);

		/// <summary>
		/// Rotates the Player forward to a given direction with water stats.
		/// </summary>
		/// <param name="direction">The direction you want it to face.</param>
		public virtual void WaterFaceDirection(Vector3 direction) => FaceDirection(direction, stats.current.waterRotationSpeed);

		/// <summary>
		/// Makes a transition to the Fall State if the Player is not grounded.
		/// </summary>
		public virtual void Fall()
		{
			if (!isGrounded)
			{
				states.Change<FallPlayerState>();
			}
		}

		/// <summary>
		/// Handles ground jump with proper evaluations and height control.
		/// </summary>
		public virtual void Jump()
		{
			var canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
			var canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);
			var holdJump = !holding || stats.current.canJumpWhileHolding;

			if ((isGrounded || canMultiJump || canCoyoteJump) && holdJump)
			{
				if (inputs.GetJumpDown())
				{
					Jump(stats.current.maxJumpHeight);
				}
			}

			if (inputs.GetJumpUp() && (jumpCounter > 0) && (verticalVelocity.y > stats.current.minJumpHeight))
			{
				verticalVelocity = Vector3.up * stats.current.minJumpHeight;
			}
		}

		/// <summary>
		/// Applies an upward force to the Player.
		/// </summary>
		/// <param name="height">The force you want to apply.</param>
		public virtual void Jump(float height)
		{
			jumpCounter++;
			verticalVelocity = Vector3.up * height;
			states.Change<FallPlayerState>();
			OnJump?.Invoke();
		}

		/// <summary>
		/// Applies jump force to the Player in a given direction.
		/// </summary>
		/// <param name="direction">The direction that you want to jump.</param>
		/// <param name="height">The upward force that you want to apply.</param>
		/// <param name="distance">The force towards the direction that you want to apply.</param>
		public virtual void DirectionalJump(Vector3 direction, float height, float distance)
		{
			jumpCounter++;
			verticalVelocity = Vector3.up * height;
			lateralVelocity = direction * distance;
			OnJump?.Invoke();
		}

		/// <summary>
		/// Sets the jump counter to zero affecting further jump evaluations.
		/// </summary>
		public virtual void ResetJumps() => jumpCounter = 0;

		/// <summary>
		/// Sets the air spin counter back to zero.
		/// </summary>
		public virtual void ResetAirSpins() => airSpinCounter = 0;

		public virtual void SphereAttack(float radius, int damage)
		{
			var overlaps = Physics.OverlapSphereNonAlloc(position, radius, m_attackSphereOverlaps);

			for (int i = 0; i < overlaps; i++)
			{
				if (m_attackSphereOverlaps[i].transform != transform)
				{
					if (m_attackSphereOverlaps[i].TryGetComponent(out Enemy enemy))
					{
						enemy.ApplyDamage(damage);
					}
					else if (m_attackSphereOverlaps[i].TryGetComponent(out Breakable breakable))
					{
						breakable.Break();
					}
				}
			}
		}

		public virtual void Spin()
		{
			var canAirSpin = (isGrounded || stats.current.canAirSpin) && airSpinCounter < stats.current.allowedAirSpins;

			if (stats.current.canSpin && canAirSpin && !holding && inputs.GetSpinDown())
			{
				if (!isGrounded)
				{
					airSpinCounter++;
				}

				states.Change<SpinPlayerState>();
				OnSpin?.Invoke();
			}
		}

		public virtual void PickAndThrow()
		{
			if (stats.current.canPickUp && inputs.GetPickAndDropDown())
			{
				if (!holding)
				{
					if (CapsuleCast(transform.forward,
						stats.current.pickDistance, out var hit))
					{
						if (hit.transform.TryGetComponent(out Pickable pickable))
						{
							PickUp(pickable);
						}
					}
				}
				else
				{
					Throw();
				}
			}
		}

		public virtual void PickUp(Pickable pickable)
		{
			if (!holding && (isGrounded || stats.current.canPickUpOnAir))
			{
				holding = true;
				this.pickable = pickable;
				pickable.PickUp(pickableSlot);
				pickable.onRespawn.AddListener(RemovePickable);
				OnPickUp?.Invoke();
			}
		}

		public virtual void Throw()
		{
			if (holding)
			{
				var force = lateralVelocity.magnitude * stats.current.throwVelocityMultiplier;
				pickable.Release(transform.forward, force);
				pickable = null;
				holding = false;
				OnThrow?.Invoke();
			}
		}

		public virtual void RemovePickable()
		{
			if (holding)
			{
				pickable = null;
				holding = false;
			}
		}

		public virtual void EnemySmash()
		{
			if (stats.current.canSmashEnemies && !isGrounded && velocity.y <= 0)
			{
				var distance = height * 0.5f + stats.current.smashDistance;

				if (SphereCast(Vector3.down, distance, out var hit))
				{
					if (hit.collider.CompareTag(GameTags.Enemy) &&
						hit.collider.TryGetComponent<Enemy>(out var enemy))
					{
						if (enemy.unsizedPosition.y < unsizedPosition.y)
						{
							if (states.IsCurrentOfType(typeof(StompPlayerState)))
							{
								enemy.ApplyDamage(stats.current.stompAttackDamage);
							}
							else
							{
								var rebound = Mathf.Clamp(-verticalVelocity.y,
									stats.current.smashMinUpwardForce, stats.current.smashMaxUpwardForce);

								verticalVelocity = Vector3.up * rebound;
								enemy.ApplyDamage(stats.current.smashDamage);
							}
						}
					}
				}
			}
		}

		public virtual void AirDive()
		{
			if (stats.current.canAirDive && !isGrounded && inputs.GetAirDiveDown())
			{
				states.Change<AirDivePlayerState>();
				OnAirDive?.Invoke();
			}
		}

		public virtual void StompAttack()
		{
			if (!isGrounded && stats.current.canStompAttack && inputs.GetStompDown())
			{
				states.Change<StompPlayerState>();
			}
		}

		public virtual void LedgeGrab()
		{
			if (stats.current.canLedgeHang && velocity.y < 0 && !holding &&
				DetectingLedge(stats.current.ledgeMaxForwardDistance, stats.current.ledgeMaxDownwardDistance, out var hit))
			{
				if (!(hit.collider is CapsuleCollider) && !(hit.collider is SphereCollider))
				{
					var ledgeDistance = radius + stats.current.ledgeMaxForwardDistance;
					var lateralOffset = transform.forward * ledgeDistance;
					var verticalOffset = Vector3.down * height * 0.5f - center;
					velocity = Vector3.zero;
					transform.parent = hit.collider.CompareTag(GameTags.Platform) ? hit.transform : null;
					transform.position = hit.point - lateralOffset + verticalOffset;
					states.Change<LedgeHangingPlayerState>();
					OnLedgeGrabbed?.Invoke();
				}
			}
		}

		public virtual void Backflip(float force)
		{
			if (stats.current.canBackflip)
			{
				verticalVelocity = Vector3.up * stats.current.backflipJumpHeight;
				lateralVelocity = -transform.forward * force;
				states.Change<BackflipPlayerState>();
				OnBackflip.Invoke();
			}
		}

		/// <summary>
		/// Sets the Skin parent to a given transform.
		/// </summary>
		/// <param name="parent">The transform you want to parent the skin to.</param>
		public virtual void SetSkinParent(Transform parent)
		{
			if (skin)
			{
				skin.parent = parent;
			}
		}

		/// <summary>
		/// Resets the Skin parenting to its initial one, with original position and rotation.
		/// </summary>
		public virtual void ResetSkinParent()
		{
			if (skin)
			{
				skin.parent = transform;
				skin.localPosition = m_skinInitialPosition;
				skin.localRotation = m_skinInitialRotation;
			}
		}

		public virtual void WallDrag(Collider other)
		{
			if (stats.current.canWallDrag && velocity.y <= 0 &&
				!holding && !other.TryGetComponent<Rigidbody>(out _))
			{
				if (CapsuleCast(transform.forward, 0.25f, out var hit,
					stats.current.wallDragLayers) && !DetectingLedge(0.25f, height, out _))
				{
					lastWallNormal = hit.normal;
					states.Change<WallDragPlayerState>();
				}
			}
		}

		public virtual void PushRigidbody(Collider other)
		{
			if (!IsPointUnderStep(other.bounds.max) &&
				other.TryGetComponent(out Rigidbody rigidbody))
			{
				var force = lateralVelocity * stats.current.pushForce;
				rigidbody.velocity += force / rigidbody.mass * Time.deltaTime;
			}
		}

		protected virtual bool DetectingLedge(float forwardDistance, float downwardDistance, out RaycastHit ledgeHit)
		{
			var ledgeMaxDistance = radius + forwardDistance;
			var origin = position + transform.up * height * 0.5f + transform.forward * ledgeMaxDistance;
			var distance = downwardDistance;

			return Physics.Raycast(origin, Vector3.down, out ledgeHit, distance,
				stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore);
		}

		protected override void Awake()
		{
			base.Awake();
			InitializeInputs();
			InitializeStats();
			InitializeHealth();
			InitializeTag();
			InitializeRespawn();
			OnGroundEnter.AddListener(ResetJumps);
			OnGroundEnter.AddListener(ResetAirSpins);
		}

		protected override void OnUpdate() => EnemySmash();

		protected virtual void OnTriggerStay(Collider other)
		{
			if (other.CompareTag(GameTags.VolumeWater))
			{
				if (!onWater && other.bounds.Contains(unsizedPosition))
				{
					EnterWater(other);
				}
				else if (onWater)
				{
					var exitPoint = position + Vector3.down * k_waterExitOffset;

					if (!other.bounds.Contains(exitPoint))
					{
						ExitWater();
					}
				}
			}
		}
	}
}
