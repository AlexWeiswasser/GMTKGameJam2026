using UnityEngine;
public class TumbleweedScript : BehaviorScript
{
	[SerializeField] int contactDamage; 

	protected override void Start()
	{
		canBehavior = true; 

		base.Start();
		targetPos = MovementScript.playerTransform;
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
