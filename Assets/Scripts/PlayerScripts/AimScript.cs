using UnityEngine;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Transform gunTransform;
	[SerializeField] SpriteRenderer gunSprite;

	private void Update()
	{
		Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
		Vector2 cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);
		Vector2 direction = (cursorWorldPos - (Vector2)transform.position).normalized;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);

		bool flip = angle < -90 || angle > 90 ? true : false;
		gunSprite.flipY = flip;

		float radius = Vector2.Distance(transform.position, gunTransform.position);
		Vector2 offset = direction * radius;
		gunTransform.position = (Vector2)transform.position + offset;
	}
}
