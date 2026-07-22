using UnityEngine;
using UnityEngine.InputSystem;
using static BulletStorageScript;

public class ShootScript : MonoBehaviour
{
	[Header("References")] 
	[SerializeField] Transform bulletSpawnPos;
	[SerializeField] GameObject bullet;

	public void Shoot(InputAction.CallbackContext context)
	{
		if(context.performed)
		{
			Bullet bullet = GetBullet();
			if (bullet != null)
			{
				Vector2 direction = (bulletSpawnPos.position - transform.position).normalized;
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

				BulletScript bulletScript = Instantiate(bullet.bulletObject, bulletSpawnPos.position, Quaternion.identity).GetComponent<BulletScript>();
				bulletScript.Init(bullet.speedIncrease);
			}
		}
	}
}
