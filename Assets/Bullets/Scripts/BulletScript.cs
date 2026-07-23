using UnityEngine;
using UnityEngine.InputSystem;

public class BulletScript : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Rigidbody2D rb;

	[Header("Stats")]
	[SerializeField] float speed; 

	private void Start()
	{
		// Rotation
		Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
		Vector2 cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);
		Vector2 direction = (cursorWorldPos - MovementScript.playerPos).normalized;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, angle + 90);

		// Velocity
		rb.linearVelocity = speed * direction;

		// Lifetime
		Destroy(gameObject, 5);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Effect"))
			OnEffectEnter(collision);
		else if (collision.gameObject.layer == LayerMask.NameToLayer("Character"))
			OnCharacterEnter(collision);
		else
		{
			OnBulletDeath();
			Destroy(gameObject);
		}
	}


	protected virtual void OnEffectEnter(Collider2D collision) { }
	protected virtual void OnCharacterEnter(Collider2D collision) { }
	protected virtual void OnBulletDeath() { }
}
