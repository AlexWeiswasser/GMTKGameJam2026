using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
	private void Start()
	{
		float explosionRadius = 5f;
		float explosionStrength = 10f;

		LayerMask hurtMask = LayerMask.GetMask("Character");
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, hurtMask);

		foreach (Collider2D col in nearbyColliders)
		{
			if (col.transform.TryGetComponent<ICharInterface>(out var face))
			{
				Vector2 trip = (col.transform.position - transform.position).normalized;
				face.Knockback(trip, explosionStrength);
				face.Damage(15);
			}
		}

		Destroy(gameObject);
	}
}
