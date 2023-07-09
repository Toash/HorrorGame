using UnityEngine;
using Sirenix.OdinInspector;

namespace Player
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMovement : MonoBehaviour
	{
		public KeyCode sprintKey = KeyCode.LeftShift;
		public KeyCode crouchKey = KeyCode.LeftControl;
		public KeyCode proneKey = KeyCode.Z;

		public float maxStamina = 100f;
		public float currentStamina;
		public float staminaRechargeCooldown = 2f;
		public float staminaRegenSpeed = 10f;
		public float staminaDegenSpeed = 20f;


		public float crouchSpeed = 4;
		public float crouchHeight = .35f;

		public float jumpHeight = 2;
		public float speed = 3;
		public float gravityMultiplier = 2;

		public float sprintMultiplier = 2;

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
		private bool crouching = false;
		private bool proning = false;

		private bool freeze = false;
		private Vector3 scaleDestination = new Vector3(1,1,1);

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
				HandleScaling();
				JumpLogic();
				CrouchLogic();

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
				//crouch
				this.crouching = true;
				scaleDestination = new Vector3(1, crouchHeight, 1);	
			}
			else
			{
				//uncrouch, cant uncrouch if stuff above head.
				if (!Physics.CheckSphere(headCheck.position, headCheckRadius, headMask, QueryTriggerInteraction.Ignore))
                {
					this.crouching = false;
					scaleDestination = new Vector3(1, 1, 1);
				}
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

		private void HandleScaling()
        {
			transform.localScale = Vector3.Lerp(transform.localScale, scaleDestination, crouchSpeed * Time.deltaTime); ;
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
            if (Input.GetKey(sprintKey) && HasStamina())
            {
				speedMultiplier = sprintMultiplier;
				DegenStamina();

            }
            else
            {
				speedMultiplier = 1;
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
	}
}

