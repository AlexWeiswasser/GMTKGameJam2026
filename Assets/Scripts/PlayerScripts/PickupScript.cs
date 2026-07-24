using Unity.VisualScripting;
using UnityEngine;
using static BulletStorageScript;

public class PickupScript : MonoBehaviour
{
	[SerializeField] BulletData[] startingAmmo;
    void Awake()
    {
        AddStartingAmmo();
    }

	private void AddStartingAmmo()
	{
		for (int i = 0; i < BulletStorageScript.MaximumBulletCount; i++)
		{
			BulletStorageScript.AddBullet(new Bullet { bulletData = startingAmmo[i] });
		}
		BulletStorageScript.Reload();
	}
    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (bulletCount >= 6) return;

		if(collision.CompareTag("Pickup"))
		{
			if (collision.TryGetComponent<Pickup>(out var pScript))
			{
				BulletStorageScript.AddBullet(new Bullet { bulletData = pScript.GetBullet() });
				Destroy(collision.gameObject);
			}
		}
	}
}
