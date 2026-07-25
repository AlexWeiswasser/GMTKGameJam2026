using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
	Vector3 velocity;
	[SerializeField] float smoothTime = 0;

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

				targetPos = ((Vector3)cursorWorldPos + MovementScript.playerTransform.position + MovementScript.playerTransform.position) / 3f;
				targetPos.z = -0.5f;

				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
				break;

		}
		
	}
}