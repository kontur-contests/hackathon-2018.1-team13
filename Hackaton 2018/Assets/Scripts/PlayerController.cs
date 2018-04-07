using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LookState { free, aim, locked }
public enum CharacterState { crouch, walk, run, jump, inair}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IHitable
{
	public static bool controlsEnabled = true;
	public static PlayerController instance;
	public static PlayerInput input;

	[HideInInspector]	public CharacterController characterController;
	[HideInInspector]	public Camera cameraController;

	private CharacterState stateCharacter = CharacterState.walk;
	public LookState stateLook = LookState.free;
	
	public Quaternion m_characterRot;
	public Quaternion m_cameraRot;
	private Vector3 moveDir = Vector3.zero;

	private CollisionFlags m_collisionFlags;

	private float crouchDeltaHeight = 0.3f;
	private Vector3 m_standCamPos;
	private Vector3 m_crouchCamPos;
	private float m_standHeight;
	private float m_crouchHeight;
	private float m_standCenter;
	private float m_crouchCenter;

	public Vector3 pointCenter	{	get	{
			return transform.position + Vector3.up * characterController.center.y;	}	}
	public Vector3 pointFeet	{	get	{
			return transform.position - Vector3.up * (characterController.height * 0.5f - characterController.center.y - 0.20f);	}	}
	public Vector3 pointHead	{	get	{
			return transform.position + Vector3.up * (characterController.height * 0.5f + characterController.center.y - 0.20f);	}	}

	[Header("Weapons")]
	private int current_weapon = 0;
	// public AWeapon[] weapons;

	private static float health = 10f;
	private static float max_health = 10f;

	private void Awake()
	{
		instance = this;

		characterController = GetComponent<CharacterController>();
		cameraController = GetComponentInChildren<Camera>();
		input = new PlayerInput();

		m_characterRot = transform.rotation;
		m_cameraRot = cameraController.transform.localRotation;
		
		m_standCamPos = m_crouchCamPos = cameraController.transform.localPosition;
		m_crouchCamPos.y -= crouchDeltaHeight;
		m_standHeight = characterController.height;
		m_crouchHeight = m_standHeight - crouchDeltaHeight;
		m_standCenter = characterController.center.y;
		m_crouchCenter = m_standCenter - crouchDeltaHeight * 0.5f;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Start()
	{
		
	}

	private void FixedUpdate()
	{
		input.Update();

		UpdateStates();
		Movement();

		if (input.b_LMB || input.b_RMB)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void Update()
	{
		MouseLook();
	}
	
	// public float mouse_smooth = 5f;
	private void MouseLook()
	{
		if (stateLook != LookState.locked)
		{
			if (stateLook == LookState.aim)
				input.mouseAxis *= 0.5f;

			m_characterRot *= Quaternion.Euler(0, input.mouseAxis.x, 0);
			m_cameraRot *= Quaternion.Euler(-input.mouseAxis.y, 0, 0);
			m_cameraRot = ClampRotationAroundXAxis(m_cameraRot);

			//transform.localRotation = Quaternion.Slerp(transform.localRotation, characterRot,
			//	mouse_smooth * Time.deltaTime);
			//cameraController.transform.localRotation = Quaternion.Slerp(cameraController.transform.localRotation, cameraRot,
			//	mouse_smooth * Time.deltaTime);

			transform.localRotation = m_characterRot;
			cameraController.transform.localRotation = m_cameraRot;
		}
	}

	private void UpdateStates()
	{
		if (stateCharacter == CharacterState.inair)
		{
			if (characterController.isGrounded)
			{
				Debug.Log("land");
				stateCharacter = CharacterState.walk;
				moveDir.y = 0;
			}
		}
		else if (stateCharacter == CharacterState.crouch)
		{
			if (!input.b_Crouch)
			{
				RaycastHit hitInfo;
				LayerMask layerMask = LayerMask.NameToLayer("Default") + LayerMask.NameToLayer("Character");
				Physics.SphereCast(transform.position, characterController.radius, Vector3.up, out hitInfo, characterController.height / 2f, layerMask, QueryTriggerInteraction.Ignore);
				if (!hitInfo.transform)
					stateCharacter = CharacterState.walk;
			}
		}
		else if (input.b_Crouch)
		{
			stateCharacter = CharacterState.crouch;
		}
		else if (input.b_Jump)
		{
			stateCharacter = CharacterState.jump;
		}
		else if (input.b_Sprint && stateLook == LookState.free)
		{
			stateCharacter = CharacterState.run;
		}
		else
		{
			stateCharacter = CharacterState.walk;
		}
	}

	[Header("Move Speed")]
	public float speedNormal = 2f;
	public float speedSprint = 4f;
	public float speedCrouch = 1f;
	[Header("Jump")]
	public float jumpPower = 2.5f;
	public float gravityPull = 3f;
	private void Movement()
	{
		Crouch();

		float speed = speedNormal;
		if (stateCharacter == CharacterState.crouch || stateLook == LookState.aim)
			speed = speedCrouch;
		else if (stateCharacter == CharacterState.run)
			speed = speedSprint;

		Vector2 dir = new Vector2(input.keyAxis.x, input.keyAxis.y);
		if (dir.magnitude > 0)
			dir.Normalize();
		Vector3 plannedMove = transform.forward * dir.y + transform.right * dir.x;
		
		RaycastHit hitInfo;
		LayerMask layerMask = LayerMask.NameToLayer("Default") + LayerMask.NameToLayer("Character");
		Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo, characterController.height / 2f, layerMask, QueryTriggerInteraction.Ignore);
		plannedMove = Vector3.ProjectOnPlane(plannedMove, hitInfo.normal).normalized;
		moveDir.x = plannedMove.x * speed;
		moveDir.z = plannedMove.z * speed;

		if (stateCharacter == CharacterState.jump)
		{
			Debug.Log("jump");
			stateCharacter = CharacterState.inair;
			moveDir.y = jumpPower;
		}
		else if (stateCharacter == CharacterState.inair)
		{
			moveDir += Physics.gravity * Time.fixedDeltaTime;
		}
		else
		{
			moveDir.y = -gravityPull;
		}
		
		m_collisionFlags = characterController.Move(moveDir * Time.fixedDeltaTime);
	}

	private void Crouch()
	{
		cameraController.transform.localPosition = Vector3.MoveTowards(cameraController.transform.localPosition, stateCharacter == CharacterState.crouch ? m_crouchCamPos : m_standCamPos, 2 * Time.deltaTime);
		characterController.height = stateCharacter == CharacterState.crouch ? m_crouchHeight : m_standHeight;
		characterController.center = new Vector3(characterController.center.x,
			stateCharacter == CharacterState.crouch ? m_crouchCenter : m_standCenter,
			characterController.center.z);
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (m_collisionFlags == CollisionFlags.Above)
		{
			print("x");
			moveDir.y = -gravityPull;
		}
	}

	private Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

		angleX = Mathf.Clamp(angleX, -70, 70);

		q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}

	#region IHitable
	public void OnHit(AttackInfo aInfo)
	{
		health -= aInfo.damage;
		if (health > 0)
		{
			m_cameraRot *= Quaternion.Euler(Random.Range(-15f, 1f), 0, 0);
			m_characterRot *= Quaternion.Euler(0, Random.Range(-10f, 10f), 0);
			//Debug.Log("Got Hit");
		}
		else
		{
			//Debug.Log("Diez");
		}
	}
	#endregion
}
