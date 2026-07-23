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
			if(collision.TryGetComponent<OilPatchScript>(out var oilScript))
			{
				oilScript.SetOnFire();
			}
		}
	}
}
