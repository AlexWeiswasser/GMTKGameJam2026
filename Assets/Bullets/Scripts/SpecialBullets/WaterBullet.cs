using UnityEngine;

public class WaterBullet : BulletScript
{
	// Timer
	Timer oilTimer = new Timer(.015f, startFinished: false);

	// Prefabs
	[SerializeField] GameObject oilSpill;
	[SerializeField] private ParticleSystem waterParticles;

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
		if (collision.CompareTag("Dynamite"))
		{
			if(collision.TryGetComponent<DynamiteScript>(out var dScript))
			{
				dScript.Defuse();
			}
		}
	}

	protected override void OnBulletDeath()
	{
		Instantiate(waterParticles, transform.position, Quaternion.identity);
		float waterRadius = 5f;

		LayerMask charMask = LayerMask.GetMask("Character");
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, waterRadius, charMask);

		foreach (Collider2D col in nearbyColliders)
		{
			if (col.transform.TryGetComponent<ICharInterface>(out var face))
			{
				face.SetWet();
			}
		}

		LayerMask effectMask = LayerMask.GetMask("Effect");
		Collider2D[] nearbyEffectColliders = Physics2D.OverlapCircleAll(transform.position, waterRadius, effectMask);

		foreach (Collider2D col in nearbyEffectColliders)
		{
			if (col.transform.TryGetComponent<FirePatchScript>(out var fire))
			{
				Destroy(fire.gameObject);
			}
			else if (col.transform.TryGetComponent<DynamiteScript>(out var dScript))
			{
				dScript.Defuse();
			}
		}

	}
}
