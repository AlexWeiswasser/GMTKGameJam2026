using UnityEngine;

public abstract class BehaviorScript : MonoBehaviour
{
	// Stats
	[HideInInspector] public bool isWet = false;
	[HideInInspector] public bool canBehavior = true;

	// References
	private Rigidbody2D rb;

	[Header("Movement")]
	[SerializeField] float maxSpeed;
	[SerializeField] float acceleration;
	[SerializeField] float damping;
	Vector2 movementVelocity;
	Vector2 externalVelocity;
	protected Transform targetPos;

	protected virtual void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	protected virtual void FixedUpdate()
	{
		Debug.Log("Fixed update");

		// Movement
		rb.linearDamping = damping;
		if (!canBehavior) return;

		float currentMaxSpeed = maxSpeed;
		if (isWet) currentMaxSpeed *= .8f;

		Vector2 direction = (targetPos.position - transform.position).normalized;
		movementVelocity = Vector2.MoveTowards(
			movementVelocity,
			direction * currentMaxSpeed,
			acceleration * Time.fixedDeltaTime);

		externalVelocity *= 0.9f;
		rb.linearVelocity = movementVelocity + externalVelocity;
	}

	public void Knockback(Vector2 direction, float strength)
	{
		externalVelocity = Vector2.MoveTowards(
			externalVelocity,
			direction * strength,
			strength * Time.fixedDeltaTime);
	}
}
