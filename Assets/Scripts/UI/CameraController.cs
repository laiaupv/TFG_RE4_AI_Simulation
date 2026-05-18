using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2.5f;
    public float rightOffset = 1.2f;
    public float mouseSensitivity = 3f;

    private float _rotationY = 0f;

    void Update()
    {
        _rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(0f, _rotationY, 0f);

        Vector3 desiredPosition = target.position
            + rotation * new Vector3(rightOffset, height, -distance);

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}