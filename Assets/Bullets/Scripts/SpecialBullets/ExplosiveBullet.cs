using UnityEngine;

public class ExplosiveBullet : BulletScript
{
	// Prefabs
	[SerializeField] GameObject explosion;
	[SerializeField] ParticleSystem explosionParticles; 

	protected override void OnBulletDeath()
	{
		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Instantiate(explosion, transform.position, Quaternion.identity);
	}
}
