using UnityEngine;

public class FireBullet : BulletScript
{
	// Timer
	Timer oilTimer = new Timer(.1f, startFinished: true);

	// Prefabs
	[SerializeField] GameObject fire;

	private void Update()
	{
		oilTimer.Tick(Time.deltaTime);
		if (oilTimer.Finished)
		{
			oilTimer.Reset();
			Instantiate(fire, transform.position, Quaternion.identity);
		}
	}

	protected override void OnBulletDeath()
	{
		Transform oil = Instantiate(fire, transform.position, Quaternion.identity).transform;
		oil.localScale *= 3;
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
