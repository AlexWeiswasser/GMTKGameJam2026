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
		Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
		Vector2 cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);

		Vector3 targetPos = ((Vector3)cursorWorldPos + MovementScript.playerTransform.position + MovementScript.playerTransform.position) / 3f;
		targetPos.z = -0.5f;

		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
	}
}