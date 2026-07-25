using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
	Vector3 velocity;
	[SerializeField] float smoothTime = 0;
    public float maxDistanceFromPlayer;

	void LateUpdate()
	{
		CameraUpdate();
	}

	void CameraUpdate()
{
    Vector3 targetPos;
    switch (BulletTrackerScript.currentMode)
    {
        case BulletTrackerScript.TrackerMode.EditingMode:
            targetPos = MovementScript.playerTransform.position;
            targetPos.z = -0.5f;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            break;

        default:
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);

            Vector3 playerPos = MovementScript.playerTransform.position;
            targetPos = ((Vector3)cursorWorldPos + playerPos + playerPos) / 3f;

            // Clamp how far the target can be from the player
            Vector3 offset = targetPos - playerPos;
            offset = Vector3.ClampMagnitude(offset, maxDistanceFromPlayer);
            targetPos = playerPos + offset;

            targetPos.z = -0.5f;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            break;
    }
}
}