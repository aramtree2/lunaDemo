using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "PLAYER TWO/Platformer Project/Player/New Player Stats")]
	public class PlayerStats : EntityStats<PlayerStats>
	{
		[Header("General Stats")]
		public float pushForce = 4f;
		public float snapForce = 15f;
		public float slideForce = 10f;
		public float rotationSpeed = 970f;
		public float gravity = 38f;
		public float fallGravity = 65f;
		public float gravityTopSpeed = 50f;

		[Header("Enemy Smash Stats")]
		public bool canSmashEnemies = true;
		public int smashDamage = 1;
		public float smashDistance = 0.25f;
		public float smashMinUpwardForce = 10f;
		public float smashMaxUpwardForce = 25f;

		[Header("Pick'n Throw Stats")]
		public bool canPickUp = true;
		public bool canPickUpOnAir = false;
		public bool canJumpWhileHolding = true;
		public float pickDistance = 0.5f;
		public float throwVelocityMultiplier = 1.5f;

		[Header("Motion Stats")]
		public float acceleration = 13f;
		public float deceleration = 28f;
		public float friction = 16f;
		public float topSpeed = 6f;
		public float turningDrag = 28f;
		public float airAcceleration = 32f;

		[Header("Running Stats")]
		public float runningAcceleration = 16f;
		public float runningTopSpeed = 7.5f;
		public float runningTurningDrag = 14f;

		[Header("Jump Stats")]
		public int multiJumps = 1;
		public float coyoteJumpThreshold = 0.15f;
		public float maxJumpHeight = 17f;
		public float minJumpHeight = 10f;

		[Header("Crouch Stats")]
		public float crouchHeight = 1f;
		public float crouchFriction = 10f;

		[Header("Crawling Stats")]
		public float crawlingAcceleration = 8f;
		public float crawlingFriction = 32f;
		public float crawlingTopSpeed = 2.5f;
		public float crawlingTurningSpeed = 3f;

		[Header("Wall Drag Stats")]
		public bool canWallDrag = true;
		public LayerMask wallDragLayers;
		public Vector3 wallDragSkinOffset;
		public float wallDragGravity = 12f;
		public float wallJumpDistance = 8f;
		public float wallJumpHeight = 15f;

		[Header("Pole Climb Stats")]
		public bool canPoleClimb = true;
		public Vector3 poleClimbSkinOffset;
		public float climbUpSpeed = 3f;
		public float climbDownSpeed = 8f;
		public float climbRotationSpeed = 2f;
		public float poleJumpDistance = 8f;
		public float poleJumpHeight = 15f;

		[Header("Swimming Stats")]
		public float waterConversion = 0.35f;
		public float waterRotationSpeed = 360f;
		public float waterUpwardsForce = 8f;
		public float waterJumpHeight = 15f;
		public float waterTurningDrag = 2.5f;
		public float swimAcceleration = 4f;
		public float swimDeceleration = 3f;
		public float swimTopSpeed = 4f;
		public float swimDiveForce = 15f;

		[Header("Spin Stats")]
		public bool canSpin = true;
		public bool canAirSpin = true;
		public float spinAttackRadius = 1f;
		public int spinDamage = 1;
		public float spinDuration = 0.5f;
		public float airSpinUpwardForce = 10f;
		public int allowedAirSpins = 1;

		[Header("Hurt Stats")]
		public float hurtUpwardForce = 10f;
		public float hurtBackwardsForce = 5f;

		[Header("Air Dive Stats")]
		public bool canAirDive = true;
		public float airDiveForwardForce = 16f;
		public float airDiveGroundDeceleration = 32f;
		public float airDiveGroundLeapHeight = 10f;

		[Header("Stomp Attack Stats")]
		public bool canStompAttack = true;
		public int stompAttackDamage = 3;
		public float stompDownwardForce = 20f;
		public float stompAirTime = 0.8f;
		public float stompGroundTime = 0.5f;
		public float stompGroundLeapHeight = 10f;

		[Header("Ledge Hanging Stats")]
		public bool canLedgeHang = true;
		public LayerMask ledgeHangingLayers;
		public Vector3 ledgeHangingSkinOffset;
		public float ledgeMaxForwardDistance = 0.25f;
		public float ledgeMaxDownwardDistance = 0.25f;
		public float ledgeSideMaxDistance = 0.5f;
		public float ledgeSideHeightOffset = 0.15f;
		public float ledgeSideCollisionRadius = 0.25f;
		public float ledgeMovementSpeed = 1.5f;

		[Header("Ledge Climbing Stats")]
		public bool canClimbLedges = true;
		public LayerMask ledgeClimbingLayers;
		public Vector3 ledgeClimbingSkinOffset;
		public float ledgeClimbingDuration = 1f;

		[Header("Backflip Stats")]
		public bool canBackflip = true;
		public float backflipJumpHeight = 23f;
		public float backflipGravity = 35f;
		public float backflipBackwardForce = 4f;
		public float backflipBackwardTurnForce = 8f;
	}
}