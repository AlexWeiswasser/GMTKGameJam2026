using UnityEngine;

public class WaterBullet : BulletScript
{
	// Timer
	Timer oilTimer = new Timer(.015f, startFinished: false);

	// Prefabs
	[SerializeField] GameObject oilSpill;

	// Tracking
	bool spreadingOil = false; 

	protected override void OnCharacterEnter(Collider2D collision)
	{
		if (collision.TryGetComponent<ICharInterface>(out var character))
		{
			character.SetWet();
		}
	}

	private void Update()
	{
		if (spreadingOil)
		{
			oilTimer.Tick(Time.deltaTime);
			if (oilTimer.Finished)
			{
				oilTimer.Reset();
				Instantiate(oilSpill, transform.position, Quaternion.identity);
			}
		}
	}

	protected override void OnEffectEnter(Collider2D collision)
	{
		if (collision.CompareTag("Oil"))
		{
			spreadingOil = true; 
		}
		if (collision.CompareTag("Fire"))
		{
			Destroy(collision.gameObject);
		}
	}
}
