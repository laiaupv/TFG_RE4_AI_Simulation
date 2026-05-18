using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public bool isMakingNoise = false;

    private CharacterController _controller;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        isMakingNoise = isRunning && (moveX != 0 || moveZ != 0);

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        movement = movement.normalized * currentSpeed * Time.deltaTime;
        _controller.Move(movement);
    }
}