using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController _controller;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ);
        movement = movement.normalized * speed * Time.deltaTime;
        _controller.Move(movement);
    }
}