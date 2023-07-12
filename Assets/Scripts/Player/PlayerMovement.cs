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
		public float jumpHeight = 2;
		public float speed = 3;
		public float gravityMultiplier = 2;
		public float crouchSpeedMultipler = .5f;
		public float sprintSpeedMultiplier = 2;

		[Title("Checks", titleAlignment: TitleAlignments.Centered)]
		public Transform headCheck;
		public float headCheckRadius = .4f;
		public LayerMask headMask;

		public Transform groundCheck;
		public float groundCheckRadius = .4f;
		public LayerMask groundMask;

		//private
		//---------------------------------------
		private CharacterController charControl;

		private Vector3 playerGravityVelocity;
		private float speedMultiplier = 1;
		[ShowInInspector, ReadOnly]
		private bool crouching = false;
		private bool proning = false;

		private bool freeze = false;

		private float charControllerHeightDestination;
		private Vector3 cameraRootDestination;

		// Initial variables to cache
		//---------------------------------------------------------------------------
		private float initialCharControllerHeight;
		private Vector3 initialLocalCameraRootPos;
		private float initialSpeedMultipler;
		//---------------------------------------------------------------------------

		private float staminaCooldownTimer = 0;

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
			//---------------------------------------------------------------------------
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
				//JumpLogic();

				CrouchLogic();
				HandleCameraRootPos();
				HandleCharControlHeight();

				staminaCooldownTimer += Time.deltaTime;
				if(staminaCooldownTimer > staminaRechargeCooldown)
                {
					RegenStamina();
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
				//DOTween.To(() => charControl.height, x => charControl.height = x, charControllerCrouchHeight, charControllerCrouchSpeed);
				//cameraRoot.MoveRootTowardsPos(cameraRoot.initialCameraLocalRootPos - new Vector3(0,cameraRootCrouchLowerLength,0), cameraRootCrouchLowerSpeed);
			}
			else
			{
				if (!Physics.CheckSphere(headCheck.position, headCheckRadius, headMask, QueryTriggerInteraction.Ignore))
                {
					this.crouching = false;
					//DOTween.To(() => charControl.height, x => charControl.height = x, initialCharControllerHeight, charControllerCrouchSpeed);
					//cameraRoot.MoveRootTowardsPos(cameraRoot.initialCameraLocalRootPos, cameraRootBackUpSpeed);
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
			if (isGrounded())
			{
				if (Input.GetButtonDown("Jump"))
				{
					Jump();
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
			charControl.Move(NormalizedMoveVector() * speed * speedMultiplier * Time.deltaTime);
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
				speedMultiplier = sprintSpeedMultiplier;
				DegenStamina();

            }
			else if (crouching)
            {
				speedMultiplier = crouchSpeedMultipler;
            }
            else
            {
				speedMultiplier = initialSpeedMultipler;
            }
        }


		// stamina
		//-----------------------------------------------------------------------------------
		private void RegenStamina()
        {
			
			currentStamina += (staminaRegenSpeed * Time.deltaTime);
        }
		private void DegenStamina()
        {
			currentStamina -= (staminaDegenSpeed * Time.deltaTime);
			staminaCooldownTimer = 0;
		}

		private void ClampStamina()
        {
			currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
		//-----------------------------------------------------------------------------------


		// Freeze / unfreeze movement
		//-----------------------------------------------------------------------------------
		private void FreezeMovement()
        {
			freeze = true;
        }
		private void UnfreezeMovement()
        {
			freeze = false;
        }
		//-----------------------------------------------------------------------------------

		private void HandleCharControlHeight()
        {
            if (crouching)
            {
				charControl.height = Mathf.Lerp(charControl.height, initialCharControllerHeight - charControllerCrouchLength, charControllerCrouchSpeed * Time.deltaTime);
            }
            else
            {
				charControl.height = Mathf.Lerp(charControl.height, initialCharControllerHeight, charControllBackUpSpeed * Time.deltaTime);
            }
        }
		private void HandleCameraRootPos()
        {
            if (crouching)
            {
				cameraRoot.transform.localPosition = Vector3.Lerp(cameraRoot.transform.localPosition, initialLocalCameraRootPos - new Vector3(0,cameraRootCrouchLowerLength,0), cameraRootCrouchLowerSpeed*Time.deltaTime);
			}
            else
            {
				cameraRoot.transform.localPosition = Vector3.Lerp(cameraRoot.transform.localPosition, initialLocalCameraRootPos, cameraRootBackUpSpeed*Time.deltaTime);
			}

        }
	}
}

