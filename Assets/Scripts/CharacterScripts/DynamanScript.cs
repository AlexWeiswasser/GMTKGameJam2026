using UnityEngine;
using UnityEngine.XR;

public class DynamanScript : BehaviorScript
{
	[SerializeField] GameObject explosion;
	[SerializeField] ParticleSystem explosionParticles;
	[SerializeField] Transform child;
	bool spawnedBomb = false; 

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

		// Parent
		Vector2 direction = MovementScript.playerTransform.position - transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, 0, angle + 180);

		// Child mesh
		child.Rotate(0, 1500 * Time.deltaTime, 0); 
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.CompareTag("Player"))
		{
			if (collision.transform.TryGetComponent<ICharInterface>(out var face))
			{
				Vector2 direction = (collision.transform.position - transform.position).normalized;
				SpawnBomb();
				Destroy(gameObject);
			}
		}
	}
	
	private void SpawnBomb()
	{
		if(spawnedBomb) return;
		spawnedBomb = true;
		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Instantiate(explosion, transform.position, Quaternion.identity); 
	}

	private void OnDestroy()
	{
		SpawnBomb();
	}
}
