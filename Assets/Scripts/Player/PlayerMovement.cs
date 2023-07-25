using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Player
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMovement : MonoBehaviour
	{
		public CameraRoot cameraRoot;
		[Title("Keybinds",titleAlignment: TitleAlignments.Centered)]
		public KeyCode sprintKey = KeyCode.LeftShift;
		public KeyCode crouchKey = KeyCode.LeftControl;
		public KeyCode proneKey = KeyCode.Z;

		[Title("Stamina", titleAlignment: TitleAlignments.Centered)]
		public float maxStamina = 100f;
		public float currentStamina;
		public float staminaRechargeCooldown = 2f;
		public float staminaRegenSpeed = 10f;
		public float staminaDegenSpeed = 20f;


		[Title("Changing level stuff", titleAlignment: TitleAlignments.Centered)]
		[Tooltip("Time it takes char controller to get back up from crouching")]
		public float charControllBackUpSpeed = .5f;
		public float charControllerCrouchSpeed = 1.5f;
		[Tooltip("How far to crouch down")]
		public float charControllerCrouchLength = .35f;

		//Camera
		//---------------------------------------------------------------------------------------------------
		public float cameraRootCrouchLowerSpeed = 1;
		public float cameraRootBackUpSpeed = .5f;
		public float cameraRootCrouchLowerLength = 1;

		//---------------------------------------------------------------------------------------------------

		[Title("Stats", titleAlignment: TitleAlignments.Centered)]
		public bool canJump;
		public float jumpHeight = 2;
		public float speed = 3;
		public float gravityMultiplier = 2;
		public float crouchSpeedMultipler = .5f;
		public float sprintSpeedMultiplier = 2;

		[Title("Audio")]
		public AudioSource outOfBreathSound;
		public GamingIsLove.Footsteps.Footstepper footStepper;
		public float crouchingVolume = .1f;

		[Title("Checks", titleAlignment: TitleAlignments.Centered)]
		public Transform headCheck;
		public float headCheckRadius = .4f;
		public LayerMask headMask;
		public Transform groundCheck;
		public float groundCheckRadius = .4f;
		public LayerMask groundMask;


		private CharacterController charControl;
		private Vector3 playerGravityVelocity;
		private float speedMultiplier = 1;
		[ShowInInspector, ReadOnly]
		public bool crouching = false;
		[ReadOnly]
		public bool moving;
		[ReadOnly]
		public bool sprinting;
		private bool proning = false;
		private bool freeze = false;
		[ShowInInspector,ReadOnly]
		private bool stopMovement = false;

		// Initial variables to cache
		//---------------------------------------------------------------------------
		private float initialCharControllerHeight;
		private Vector3 initialLocalCameraRootPos;
		private float initialSpeedMultipler;
		private float initialWalkLoudness;
		//---------------------------------------------------------------------------

		private float staminaCooldownTimer = 0;
		private float staminaOutOfBreathCooldown;
		private float staminaOutOfBreathTimer;

		public bool HasStamina()
        {
			if (currentStamina > 0) return true;
			return false;
        }
		void Awake()
		{
			charControl = GetComponent<CharacterController>();
			currentStamina = maxStamina;

			//cache values
			//---------------------------------------------------------------------------
			initialCharControllerHeight = charControl.height;
			initialLocalCameraRootPos = cameraRoot.transform.localPosition;
			initialSpeedMultipler = speedMultiplier;
			initialWalkLoudness = footStepper.walkVolume;
			//---------------------------------------------------------------------------
		}
        private void Start()
        {
			staminaOutOfBreathCooldown = staminaRechargeCooldown + .5f;

		}
        private void OnEnable()
        {
			PlayerSingleton.instance.pausing.Pause += FreezeMovement;
			PlayerSingleton.instance.pausing.Unpause += UnfreezeMovement;
		}
        private void OnDisable()
        {
			PlayerSingleton.instance.pausing.Pause -= FreezeMovement;
			PlayerSingleton.instance.pausing.Unpause -= UnfreezeMovement;
		}

        void Update()
		{
            if (!freeze)
            {
				HandleSpeedMultiplier();
				HandleGravity();
				HandleMovement();
				footStepper.Speed = new Vector2(charControl.velocity.x, charControl.velocity.z);
				JumpLogic();
				CrouchLogic();
				HandleCameraRootPos();
				HandleCharControlHeight();


				staminaCooldownTimer += Time.deltaTime;
				staminaOutOfBreathTimer += Time.deltaTime;
				if (staminaCooldownTimer > staminaRechargeCooldown)
                {
					InstantlyRegenStamina();
					//RegenStamina();
                }

			}
		}
        private void LateUpdate()
        {
			ClampStamina();
        }

        private void Crouch(bool crouching)
		{
			if (crouching)
			{
				this.crouching = true;
				speedMultiplier = .5f;
				footStepper.walkVolume = crouchingVolume;
			}
			else
			{
				if (!Physics.CheckSphere(headCheck.position, headCheckRadius, headMask, QueryTriggerInteraction.Ignore))
                {
					this.crouching = false;
					footStepper.walkVolume = initialWalkLoudness;
				}
				Debug.Log("Stuff is above head, cannot uncrouch.");
			}
			
		}

		private void CrouchLogic()
        {
			if (Input.GetKeyDown(crouchKey) && !crouching)
			{
				Crouch(true);
			}
			else if (Input.GetKeyDown(crouchKey) && crouching)
			{
				Crouch(false);
			}
		}

	
		private void Prone(bool proning)
        {
            if (proning)
            {
				//unprone
            }
            else
            {
				//prone
            }
        }
        private void OnDrawGizmos()
        {
			Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
			Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
        }

        private bool isGrounded()
        {
			if (Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore))
			{
				// hit ground
				return true;
			}
			return false;
        }
		public void Jump()
		{
			playerGravityVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
		}
		private void JumpLogic()
        {
            if (canJump)
            {
				if (isGrounded() && !crouching && !proning)
				{
					if (Input.GetButtonDown("Jump"))
					{
						Jump();
					}
				}
			}
		}

		private Vector3 NormalizedMoveVector()
		{
			Vector3 vertical = transform.forward * Input.GetAxisRaw("Vertical");
			Vector3 horizontal = transform.right * Input.GetAxisRaw("Horizontal");
			return Vector3.Normalize(vertical + horizontal);
		}
		private void HandleMovement()
        {
            if (!stopMovement)
            {
				charControl.Move(NormalizedMoveVector() * speed * speedMultiplier * Time.deltaTime);
				if (charControl.velocity.magnitude > 0.01f)
                {
					moving = true;
                }
                else
                {
					moving = false;
                }
			}
				
		}
		private void HandleGravity()
		{
			playerGravityVelocity += (Physics.gravity* gravityMultiplier) * Time.deltaTime;

			//set gravity to 0 when player is on the ground so i doesn't keep decreasing
			if (charControl.isGrounded && playerGravityVelocity.y < 0)
			{
				playerGravityVelocity.y = 0;
			}
			//Apply gravity to charactercontroller
			charControl.Move(playerGravityVelocity * Time.deltaTime);
		}
		private void HandleSpeedMultiplier()
        {

            if (Input.GetKey(sprintKey) && HasStamina() && !crouching)
            {
				Sprint();
            }
			else if (crouching)
            {
				speedMultiplier = crouchSpeedMultipler;
				sprinting = false;
			}
            else
            {
				speedMultiplier = initialSpeedMultipler;
				sprinting = false;
			}
        }
		private void Sprint()
        {
			speedMultiplier = sprintSpeedMultiplier;
			DegenStamina();
			sprinting = true;
		}
		private void HandleCharControlHeight()
		{
			if (crouching)
			{
				charControl.height = Mathf.Lerp(charControl.height, initialCharControllerHeight - charControllerCrouchLength, charControllerCrouchSpeed * Time.deltaTime);
			}
			else
			{
				//uncrouch
				charControl.height = Mathf.Lerp(charControl.height, initialCharControllerHeight, charControllBackUpSpeed * Time.deltaTime);
			}
		}
		private void HandleCameraRootPos()
		{
			if (crouching)
			{
				cameraRoot.transform.localPosition = Vector3.Lerp(cameraRoot.transform.localPosition, initialLocalCameraRootPos - new Vector3(0, cameraRootCrouchLowerLength, 0), cameraRootCrouchLowerSpeed * Time.deltaTime);
			}
			else
			{
				cameraRoot.transform.localPosition = Vector3.Lerp(cameraRoot.transform.localPosition, initialLocalCameraRootPos, cameraRootBackUpSpeed * Time.deltaTime);
			}

		}

		// stamina
		//-----------------------------------------------------------------------------------
		private void RegenStamina()
        {
			
			currentStamina += (staminaRegenSpeed * Time.deltaTime);
        }

		private void InstantlyRegenStamina()
        {
			currentStamina = maxStamina;
        }
		private void DegenStamina()
        {
			currentStamina -= (staminaDegenSpeed * Time.deltaTime);
			if (currentStamina <= 0.01f && (staminaOutOfBreathTimer > staminaOutOfBreathCooldown))
            {
				outOfBreathSound.Play();
				staminaOutOfBreathTimer = 0;
			}

			if (currentStamina >= 0.01f)
				staminaCooldownTimer = 0;
		}

		private void ClampStamina()
        {
			currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
		//-----------------------------------------------------------------------------------


		// Freeze / unfreeze movement
		//-----------------------------------------------------------------------------------
		public void Stop()
        {
			stopMovement = true;
        }
		public void Resume()
        {
			stopMovement = false;

		}
		private void FreezeMovement()
        {
			freeze = true;
        }
		private void UnfreezeMovement()
        {
			freeze = false;
        }
		//-----------------------------------------------------------------------------------


	}
}

