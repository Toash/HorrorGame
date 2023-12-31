using System;
using UnityEngine;

namespace Player
{
	public class PlayerCamera : MonoBehaviour
	{
		public KeyCode zoomKey = KeyCode.Mouse1;
		public float sensitivity = 2;
		public float zoomAmount;
		public float zoomSpeed;
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
		private float initialZoom;
		private float zoomDestination;

		private bool freeze = false;	

		void Awake()
		{
			Cursor.lockState = CursorLockMode.Locked;
			initialPosition = transform.position;
			initialZoom = cam.fieldOfView;
		}
        private void Start()
        {
			PlayerSingleton.instance.pausing.Pause += FreezeCameraMovement;
			PlayerSingleton.instance.pausing.Unpause += UnfreezeCameraMovement;

			//Set innitial rotatino to match parent obnject
			horizontalRotation = playerTransform.parent.rotation.y;
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
				CameraZooming();
			}
		}

        private void CameraZooming()
        {
            if (Input.GetKey(zoomKey))
            {
				zoomDestination = initialZoom - zoomAmount;
            }
            else
            {
				zoomDestination = initialZoom;
            }
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomDestination, zoomSpeed * Time.deltaTime);
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

