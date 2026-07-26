using UnityEngine;

public class ExplosiveBullet : BulletScript
{
	// Prefabs
	[SerializeField] GameObject explosion;

	protected override void OnBulletDeath()
	{
		Instantiate(explosion, transform.position, Quaternion.identity);
	}
}
