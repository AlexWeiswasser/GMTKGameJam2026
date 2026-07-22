using UnityEngine;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{
	private void Update()
	{
		Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
		Vector2 cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);
		Vector2 direction = (cursorWorldPos - (Vector2)transform.parent.position).normalized;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, angle);

		float radius = Vector2.Distance(transform.position, transform.parent.position);
		Vector2 offset = direction * radius;
		transform.position = (Vector2)transform.parent.position + offset;
	}
}
