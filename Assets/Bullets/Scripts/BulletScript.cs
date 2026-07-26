using UnityEngine;
using UnityEngine.InputSystem;

public class BulletScript : MonoBehaviour
{

	public enum BulletType
	{
		Default,
		Oil,
		Fire,
		FullMetalJacket,
		Lightning,
		Water
	}

	[Header("References")]
	[HideInInspector] public bool playerCreated;
	Rigidbody2D rb;

	[Header("Stats")]
	[SerializeField] float speed = 35f;
	public BulletType type;
	public int damage = 10; 

	public void Init(bool createdByPlayer)
	{
		playerCreated = createdByPlayer;
		if (!createdByPlayer)
			speed /= 2;

		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		Vector2 mouseScreenPos;
		Vector2 cursorWorldPos;
		Vector2 direction;

		// Rotation
		if (playerCreated)
		{
			mouseScreenPos = Mouse.current.position.ReadValue();
			cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);
			direction = (cursorWorldPos - (Vector2)MovementScript.playerTransform.position).normalized;
		}
		else
		{
			direction = ((Vector2)MovementScript.playerTransform.position - (Vector2)transform.position).normalized;
		}

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
		{
			if (collision.CompareTag("Player") != playerCreated)
			{
				if (collision.TryGetComponent<ICharInterface>(out var face))
				{
					Vector2 direction = (collision.transform.position - transform.position).normalized;
					face.Knockback(direction, damage);
					face.Damage(damage);
				}
				OnCharacterEnter(collision);
			}
		}
		else 
		{
			OnBulletDeath();
			Destroy(gameObject);
		}
	}


	protected virtual void OnEffectEnter(Collider2D collision) { }
	protected virtual void OnCharacterEnter(Collider2D collision) { Destroy(gameObject); }
	protected virtual void OnBulletDeath() { }
}
