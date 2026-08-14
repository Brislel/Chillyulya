using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _rotateSpeed = 75;
        [SerializeField] private float _runSpeed = 8;
        [SerializeField] private float _walkSpeed = 5;
        [SerializeField] private float _jumpForce = 5;
        [SerializeField] private float _gravity = -9.81f;
        [SerializeField] private float _backwardSpeedMultiplier = 0.5f;

        [Header("Camera")]
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private Animator _cameraAnimator;

        public static bool IsInputBlocked = false;

        private CharacterController _characterController;
        private Vector2 _rotation;
        private Vector3 _velocity;
        private Vector2 _direction;
        private bool _jumpRequested;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();

            if (_cameraPivot == null)
                _cameraPivot = transform.Find("CameraHolder");

            if (_cameraAnimator == null)
            {
                Camera cam = GetComponentInChildren<Camera>();
                if (cam != null) _cameraAnimator = cam.GetComponent<Animator>();
            }

            Cursor.lockState = CursorLockMode.Locked;

            if (_cameraPivot != null)
            {
                Vector3 euler = _cameraPivot.localEulerAngles;
                float pitch = euler.x;
                if (pitch > 180f) pitch -= 360f;
                pitch = Mathf.Clamp(pitch, -90f, 90f);
                float yaw = euler.y;
                _rotation = new Vector2(yaw, pitch);
            }
        }

        private void Update()
        {
            if (!IsInputBlocked)
            {
                _direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                mouseDelta *= _rotateSpeed * Time.deltaTime;
                _rotation.y += mouseDelta.x;
                _rotation.x = Mathf.Clamp(_rotation.x - mouseDelta.y, -90f, 90f);
                if (_cameraPivot != null)
                    _cameraPivot.localEulerAngles = _rotation;

                if (Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
                {
                    _jumpRequested = true;
                }
            }
            else
            {
                _direction = Vector2.zero;
            }

            if (!_characterController.isGrounded)
            {
                _velocity.y += _gravity * Time.deltaTime;
            }
            else
            {
                if (!_jumpRequested)
                    _velocity.y = -0.1f;
            }

            if (_cameraAnimator != null)
            {
                if (_cameraAnimator != null)
                {
                    Vector3 horizontalVelocity = new Vector3(_velocity.x, 0, _velocity.z);
                    float speed = IsInputBlocked ? 0f : horizontalVelocity.magnitude;

                    bool isMoving = !IsInputBlocked && _direction.sqrMagnitude > 0.001f;

                    _cameraAnimator.SetFloat("Speed", speed);
                    _cameraAnimator.SetBool("IsRunning", !IsInputBlocked && Input.GetKey(KeyCode.LeftShift));
                    _cameraAnimator.SetBool("IsMoving", isMoving);  
                }
            }
        }

        private void FixedUpdate()
        {
            if (_jumpRequested && _characterController.isGrounded)
            {
                _velocity.y = _jumpForce;
                _jumpRequested = false;
            }

            if (!IsInputBlocked)
            {
                bool isMovingBackward = _direction.y < 0f;
                float currentSpeed;
                if (isMovingBackward)
                    currentSpeed = _walkSpeed * _backwardSpeedMultiplier;
                else
                    currentSpeed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed;

                Vector2 finalDirection = _direction * currentSpeed;

                float yaw = _cameraPivot != null ? _cameraPivot.eulerAngles.y : 0f;
                Vector3 move = Quaternion.Euler(0, yaw, 0) * new Vector3(finalDirection.x, 0, finalDirection.y);

                _velocity = new Vector3(move.x, _velocity.y, move.z);
            }
            else
            {
                _velocity = new Vector3(0, _velocity.y, 0);
            }

            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}