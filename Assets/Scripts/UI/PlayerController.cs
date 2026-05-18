using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public bool isMakingNoise = false;

    private CharacterController _controller;
    private Camera _cam;
    private Animator _animator;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _cam = Camera.main;
        _animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        isMakingNoise = isRunning && (moveX != 0 || moveZ != 0);

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Movimiento relativo a la cámara
        Vector3 camForward = _cam.transform.forward;
        Vector3 camRight = _cam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 movement = (camForward * moveZ + camRight * moveX);

        if (movement.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(movement);
            _controller.Move(movement.normalized * currentSpeed * Time.deltaTime);
        }

        // Actualiza las animaciones
        if (_animator != null)
        {
            float speed = movement.magnitude;
            if (isRunning && speed > 0.1f)
                _animator.SetFloat("Speed", 1f);
            else if (speed > 0.1f)
                _animator.SetFloat("Speed", 0.3f);
            else
                _animator.SetFloat("Speed", 0f);

            _animator.SetFloat("SpeedX", moveX);
        }
    }
}