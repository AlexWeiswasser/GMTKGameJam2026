using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
	private void Start()
	{
		float explosionRadius = 5f;
		float innerExplosionRadius = 3f;
		float explosionStrength = 10f;

		LayerMask hurtMask = LayerMask.GetMask("Character");
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, hurtMask);
		Collider2D[] innerColliders = Physics2D.OverlapCircleAll(transform.position, innerExplosionRadius, hurtMask);

		foreach (Collider2D col in nearbyColliders)
		{
			if (col.transform.TryGetComponent<ICharInterface>(out var face))
			{
				Vector2 trip = (col.transform.position - transform.position).normalized;
				face.Knockback(trip, explosionStrength);
				if (!col.gameObject.CompareTag("Player"))
					face.Damage(15);
			}
		}

		foreach (Collider2D col in innerColliders)
		{
			if (col.transform.TryGetComponent<ICharInterface>(out var face))
			{
				if (col.gameObject.CompareTag("Player"))
					face.Damage(15);
			}
		}

		Destroy(gameObject);
	}
}
