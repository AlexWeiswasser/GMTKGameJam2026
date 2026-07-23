using UnityEngine;

public class FireBullet : BulletScript
{
	// Prefabs
	[SerializeField] GameObject fire;

	protected override void OnBulletDeath()
	{
		Transform oil = Instantiate(fire, transform.position, Quaternion.identity).transform;
	}

	protected override void OnEffectEnter(Collider2D collision)
	{
		if(collision.CompareTag("Oil"))
		{
			Vector2 spawnPos = collision.transform.position;
			Vector3 scale = collision.transform.localScale;
			Destroy(collision.gameObject);

			Transform newFire = Instantiate(fire, transform.position, Quaternion.identity).transform;
			newFire.localScale = scale;
		}
	}
}
