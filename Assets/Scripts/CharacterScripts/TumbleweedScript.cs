using UnityEngine;
public class TumbleweedScript : BehaviorScript
{
	[SerializeField] int contactDamage; 

	protected override void Start()
	{
		canBehavior = true;
		canMove = true; 

		base.Start();
		targetPos = MovementScript.playerTransform;
	}

	private void Update()
	{
		if (Vector2.Distance(transform.position, MovementScript.playerTransform.position) > detectionRange) return;
		if (BulletSelectController.PlayerBusy()) return;
		transform.Rotate(0, 0, 1000 * Time.deltaTime);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.CompareTag("Player"))
		{
			if (collision.transform.TryGetComponent<PlayerScript>(out var pScript))
			{
				pScript.Damage(contactDamage);
				Destroy(gameObject);
			}
		}
	}
}
