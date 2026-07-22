using UnityEngine;
using static BulletStorageScript;

public class PickupScript : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Pickup"))
		{
			if (collision.TryGetComponent<Pickup>(out var pScript))
			{
				BulletStorageScript.AddBullet(new Bullet { bulletObject = pScript.GetBullet() });
				Destroy(collision.gameObject);
			}
		}
	}
}
