using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("References")]
    public GameObject Target;

    [Header("Camera Settings")]
    public float Smoothvalue = 0.2f;

    private float startingX;
    private float startingY;
    private float startingZ;

    private float velocityX = 0f;

    void Start()
    {
        // Save the camera's starting position
        startingX = transform.position.x;
        startingY = transform.position.y;
        startingZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (Target == null)
            return;

        // Get the player's X position
        float targetX = Target.transform.position.x;

        // Don't allow the camera to move left of its starting X
        targetX = Mathf.Max(targetX, startingX);

        // Smoothly follow the player horizontally
        float newX = Mathf.SmoothDamp(
            transform.position.x,
            targetX,
            ref velocityX,
            Smoothvalue
        );

        // Clamp the actual position as well
        newX = Mathf.Max(newX, startingX);

        // Only update X; keep Y and Z fixed
        transform.position = new Vector3(
            newX,
            startingY,
            startingZ
        );
    }
}