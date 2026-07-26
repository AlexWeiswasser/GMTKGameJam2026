using UnityEngine;

public class ExplosiveBullet : BulletScript
{
	// Prefabs
	[SerializeField] GameObject explosion;
	[SerializeField] ParticleSystem explosionParticles; 
	[SerializeField] AudioClip explode; 

	protected override void OnBulletDeath()
	{
		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Instantiate(explosion, transform.position, Quaternion.identity);
		SoundManager.Instance.PlaySound(explode, .9f, 1.1f, .5f);
	}
}
