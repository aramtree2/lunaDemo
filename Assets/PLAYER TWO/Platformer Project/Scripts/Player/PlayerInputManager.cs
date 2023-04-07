using UnityEngine;
using UnityEngine.InputSystem;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/Player Input Manager")]
	public class PlayerInputManager : MonoBehaviour
	{
		public InputActionAsset actions;

		protected InputAction m_movement;
		protected InputAction m_run;
		protected InputAction m_jump;
		protected InputAction m_dive;
		protected InputAction m_spin;
		protected InputAction m_pickAndDrop;
		protected InputAction m_crouch;
		protected InputAction m_airDive;
		protected InputAction m_stomp;
		protected InputAction m_releaseLedge;
		protected InputAction m_pause;
		protected InputAction m_look;

		protected Camera m_camera;

		protected const string k_mouseDeviceName = "Mouse";

		protected virtual void CacheActions()
		{
			m_movement = actions["Movement"];
			m_run = actions["Run"];
			m_jump = actions["Jump"];
			m_dive = actions["Dive"];
			m_spin = actions["Spin"];
			m_pickAndDrop = actions["PickAndDrop"];
			m_crouch = actions["Crouch"];
			m_airDive = actions["AirDive"];
			m_stomp = actions["Stomp"];
			m_releaseLedge = actions["ReleaseLedge"];
			m_pause = actions["Pause"];
			m_look = actions["Look"];
		}

		public virtual Vector3 GetMovementDirection()
		{
			var value = m_movement.ReadValue<Vector2>();
			return new Vector3(value.x, 0, value.y);
		}

		public virtual Vector3 GetLookDirection()
		{
			var value = m_look.ReadValue<Vector2>();
			return new Vector3(value.x, 0, value.y);
		}

		public virtual Vector3 GetMovementCameraDirection()
		{
			var direction = GetMovementDirection();

			if (direction.sqrMagnitude > 0)
			{
				var rotation = Quaternion.AngleAxis(m_camera.transform.eulerAngles.y, Vector3.up);
				direction = rotation * direction;
				direction = direction.normalized;
			}

			return direction;
		}

		public virtual bool IsLookingWithMouse() => m_look.activeControl.device.name.Equals(k_mouseDeviceName);

		public virtual bool GetRun() => m_run.IsPressed();
		public virtual bool GetRunUp() => m_run.WasReleasedThisFrame();
		public virtual bool GetJumpDown() => m_jump.WasPressedThisFrame();
		public virtual bool GetJumpUp() => m_jump.WasReleasedThisFrame();
		public virtual bool GetDive() => m_dive.IsPressed();
		public virtual bool GetSpinDown() => m_spin.WasPressedThisFrame();
		public virtual bool GetPickAndDropDown() => m_pickAndDrop.WasPressedThisFrame();
		public virtual bool GetCrouchAndCraw() => m_crouch.IsPressed();
		public virtual bool GetAirDiveDown() => m_airDive.WasPressedThisFrame();
		public virtual bool GetStompDown() => m_stomp.WasPressedThisFrame();
		public virtual bool GetReleaseLedgeDown() => m_releaseLedge.WasPressedThisFrame();
		public virtual bool GetPauseDown() => m_pause.WasPressedThisFrame();

		protected virtual void Start()
		{
			m_camera = Camera.main;
			actions.Enable();
			CacheActions();
		}

		protected virtual void OnEnable() => actions?.Enable();
		protected virtual void OnDisable() => actions?.Disable();
	}
}
