using UnityEditor;
using UnityEngine;

public abstract class BehaviorScript : MonoBehaviour
{
	// Stats
	[HideInInspector] public bool isWet = false;
	[HideInInspector] public bool canBehavior = true;
	protected bool canMove = true;

	// References
	protected Rigidbody2D rb;
	public SpriteRenderer[] rend;

	[Header("Movement")]
	[SerializeField] float maxSpeed;
	[SerializeField] float acceleration;
	[SerializeField] float damping;
	public float detectionRange;
	Vector2 movementVelocity;
	public Vector2 externalVelocity;
	protected Transform targetPos;
	protected Vector2 givenDirection = Vector2.zero;

	// Lunge movement
	private float lungeTimer;
	private bool lunging = false;

	protected virtual void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	protected virtual void FixedUpdate()
	{
		// Looking at the player
		if (MovementScript.playerTransform.position.x < transform.position.x)
		{
			foreach (SpriteRenderer spr in rend)
			{
				spr.flipX = true;
			}
		}		
		else
		{
			foreach (SpriteRenderer spr in rend)
			{
				spr.flipX = false;
			}
		}

		// Don't do anything if outside detection range
		if (Vector2.Distance(transform.position, MovementScript.playerTransform.position) > detectionRange) return;
		if (BulletSelectController.PlayerBusy()) return;

		// Lunge movement
		if (lunging)
		{
			lungeTimer -= Time.fixedDeltaTime;

			if (lungeTimer <= 0f)
			{
				lunging = false;
				movementVelocity = Vector2.zero;
			}
		}
		else
		{
			// Base movement
			rb.linearDamping = damping;
			if (canBehavior && canMove)
			{
				float currentMaxSpeed = maxSpeed;
				if (isWet) currentMaxSpeed *= .8f;

				if (givenDirection != Vector2.zero)
				{
					movementVelocity = Vector2.MoveTowards(
						movementVelocity,
						givenDirection * currentMaxSpeed,
						acceleration * Time.fixedDeltaTime);
				}
				else
				{
					Vector2 direction = (targetPos.position - transform.position).normalized;
					movementVelocity = Vector2.MoveTowards(
						movementVelocity,
						direction * currentMaxSpeed,
						acceleration * Time.fixedDeltaTime);
				}
			}
		}

		externalVelocity *= 0.9f;
		rb.linearVelocity = movementVelocity + externalVelocity;
	}

	public void Knockback(Vector2 direction, float strength)
	{
		strength *= 50;

		externalVelocity = Vector2.MoveTowards(
			externalVelocity,
			direction * strength,
			strength * Time.fixedDeltaTime);
	}

	public float Lunge(Vector2 targetPos, float speed)
	{
		if (!canBehavior) return -1;

		Vector2 toTarget = targetPos - rb.position;

		float distance = toTarget.magnitude;
		float duration = distance / speed;

		movementVelocity = toTarget.normalized * speed;

		lungeTimer = duration;
		lunging = true;
		return duration;
	}

	protected void DamageCheck(CircleCollider2D collider, int damage)
	{
		float radius = collider.radius * Mathf.Max(collider.transform.lossyScale.x, collider.transform.lossyScale.y, collider.transform.lossyScale.z) * 1.1f;
		LayerMask hurtMask = LayerMask.GetMask("Character");
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, radius, hurtMask);

		foreach (Collider2D col in nearbyColliders)
		{
			if (col.transform.CompareTag("Player"))
			{
				if (col.transform.TryGetComponent<ICharInterface>(out var face))
				{
					Vector2 direction = (col.transform.position - transform.position).normalized;
					face.Knockback(direction, damage);
					face.Damage(damage);
				}
			}
		}
	}
}
