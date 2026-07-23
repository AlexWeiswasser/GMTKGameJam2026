using UnityEngine;

public class OilBullet : BulletScript
{
	// Timer
    Timer oilTimer = new Timer(.05f, startFinished: false);

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
		Instantiate(oilSpill, transform.position, Quaternion.identity);
	}
}
