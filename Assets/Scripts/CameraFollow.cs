using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player's transform
    public Vector3 offset; // The offset from the target
    private Vector3 initialPosition; // Initial position of the camera relative to the player
    private Quaternion initialRotation; // Initial rotation of the camera

    void Start()
    {
        // Store the initial position of the camera relative to the player
        initialPosition = target.position + offset;
        // Store the initial rotation of the camera
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // Calculate the desired position based on the initial position
        Vector3 desiredPosition = target.position + offset;

        // Update the camera's position to the desired position
        transform.position = desiredPosition;

        // Reapply the initial rotation
        transform.rotation = initialRotation;
    }
}
