using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;            // Reference to the player's transform
    public float smoothSpeed = 0.125f;  // Smoothness of camera movement (lower is smoother)
    public Vector3 offset;             // Offset to maintain distance from the player

    private void Start()
    {
        // Ensure that the player is assigned
        if (player == null)
        {
            // Debug.LogError("Player reference not assigned in CameraFollow script!");
            return;
        }

        // If you haven't set the offset, calculate a default one based on the player's starting position
        if (offset == Vector3.zero)
        {
            offset = transform.position - player.position;
        }

        // Log the initial offset for debugging purposes
        // Debug.Log("Initial Camera Offset: " + offset);
    }

    private void LateUpdate()
    {
        if (player == null) return; // Exit if player is missing

        // Calculate the target position based on the player's position and the offset
        Vector3 targetPosition = player.position + offset;

        // Ensure the camera stays at a fixed Z position (e.g., -10) for 2D
        targetPosition.z = -10f;

        // Smoothly move the camera towards the target position
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        // Set the camera's position (only adjusting X, Y, and keeping Z constant)
        transform.position = new Vector3(smoothPosition.x, smoothPosition.y, -10f);

        // Optional: Debugging to check the camera position
        // Debug.Log("Camera Position: " + transform.position);
    }
}