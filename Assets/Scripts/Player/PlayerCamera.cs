using UnityEngine;

namespace Player
{
	public class PlayerCamera : MonoBehaviour
	{
		public float sensitivity = 2;
		public Camera cam;
		public GameObject holder;
		public Transform playerTransform;
		public Transform leftCheck;
		public Transform rightCheck;
		public float leanLength = .6f;
		public float leanSpeed = 6f;
		public float maxLeanAngle = 45f;
		public float leanRotSpeed = 3f;
		public float leanCheckRadius = .1f;
		public LayerMask leanCheckMask;


		private float verticalRotation = 0f;
		private float horizontalRotation = 0f;

		private float leanInput = 0f;


		private Vector3 destination;
		private Vector3 rotationDestination;
		private Vector3 initialPosition;

		private bool freeze = false;
		public Camera getCameraRef()
		{
			return cam;
		}

		void Awake()
		{
			Cursor.lockState = CursorLockMode.Locked;
			initialPosition = transform.position;
		}
        private void Start()
        {
			PlayerSingleton.instance.pausing.Pause += FreezeCameraMovement;
			PlayerSingleton.instance.pausing.Unpause += UnfreezeCameraMovement;
		}

        private void OnDisable()
        {
			PlayerSingleton.instance.pausing.Pause -= FreezeCameraMovement;
			PlayerSingleton.instance.pausing.Unpause -= UnfreezeCameraMovement;
		}
        public void Update()
		{
            if (!freeze)
            {
				cameraMovement();
				cameraRotation();
				leanBehaviour();
				LerpToDestination();
				
			}
		}

        private void OnDrawGizmos()
        {
			Gizmos.DrawWireSphere(leftCheck.position, leanCheckRadius);
			Gizmos.DrawWireSphere(rightCheck.position, leanCheckRadius);
		}

        /// <summary>
        /// Move the camera, can be used to implement recoil.
        /// </summary>
        /// <param name="move"></param>
        public void ApplyCameraMovement(Vector2 move)
        {
			horizontalRotation += move.x;
			verticalRotation -= move.y;
        }
		private void cameraMovement()
		{
			// Horizontal player rotation
			horizontalRotation += (Input.GetAxis("Mouse X") * sensitivity);
			playerTransform.rotation = Quaternion.Euler(0, horizontalRotation, 0);

			// Vertical camera rotation
			verticalRotation -= (Input.GetAxis("Mouse Y") * sensitivity);
			verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

		}

		private void leanBehaviour()
        {
            if (leanCheckColliding())
            {
				leanInput = 0;
            }
            else
            {
				leanInput = Input.GetAxis("Leaning");
			}

			rotationDestination = Vector3.forward * -leanInput * maxLeanAngle;
			destination = transform.parent.position + (transform.right * leanInput * leanLength);
		}
		private bool leanCheckColliding()
		{

			if (Physics.CheckSphere(leftCheck.position, leanCheckRadius,leanCheckMask,QueryTriggerInteraction.Ignore))
			{
				return true;
			}
			if (Physics.CheckSphere(rightCheck.position, leanCheckRadius, leanCheckMask, QueryTriggerInteraction.Ignore))
			{
				return true;
			}
			return false;
		}

		private void cameraRotation()
        {
			holder.transform.localRotation = Quaternion.Euler(new Vector3(verticalRotation, 0, 0));
			
			//quaternion * quaternion is "adding" them 
			transform.localRotation = Quaternion.Lerp(transform.localRotation,  Quaternion.Euler(rotationDestination), Time.deltaTime * leanRotSpeed);
		}
		private void LerpToDestination()
        {
			transform.position = Vector3.Lerp(transform.position, destination, leanSpeed * Time.deltaTime);
        }

		private void FreezeCameraMovement()
        {
			freeze = true;
        }

		private void UnfreezeCameraMovement()
        {
			freeze = false;
        }
	}
}

