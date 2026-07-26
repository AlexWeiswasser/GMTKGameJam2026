using UnityEngine;

public class VortexBullet : BulletScript
{
	protected override void OnBulletDeath()
	{
		float vortexRadius = 5f;

		LayerMask hurtMask = LayerMask.GetMask("Character");
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, vortexRadius, hurtMask);

		foreach (Collider2D col in nearbyColliders)
		{
			if (col.transform.TryGetComponent<ICharInterface>(out var face))
			{
				Vector2 trip = (transform.position - col.transform.position);
				float pullStrength = trip.magnitude * 10;

				face.Knockback(trip.normalized, pullStrength);
			}
		}
	}
}
