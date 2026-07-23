using UnityEngine;

public class OilBullet : BulletScript
{
	// Timer
    Timer oilTimer = new Timer(.1f);

	// Prefabs
	[SerializeField] GameObject oilSpill;

	private void Update()
	{
		oilTimer.Tick(Time.deltaTime);
		if(oilTimer.Finished)
		{
			oilTimer.Reset();
			Instantiate(oilSpill, transform.position, Quaternion.identity);
		}
	}

	protected override void OnBulletDeath()
	{
		Transform oil = Instantiate(oilSpill, transform.position, Quaternion.identity).transform;
		oil.localScale *= 3;
	}
}
