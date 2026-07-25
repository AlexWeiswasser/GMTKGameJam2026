using UnityEngine;
using UnityEngine.InputSystem;
using static BulletStorageScript;

public class ShootScript : MonoBehaviour
{
	[Header("References")] 
	[SerializeField] Transform bulletSpawnPos;
	[SerializeField] GameObject bullet;
	[HideInInspector] public bool canShoot = true;

	public void Shoot(InputAction.CallbackContext context)
	{
		if(context.performed)
		{
			if (!canShoot) return;
			if (BulletSelectController.PlayerBusy()) return;

			Bullet bullet = FireBullet();
			if (bullet != null)
			{
				Vector2 direction = (bulletSpawnPos.position - transform.position).normalized;
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

				BulletScript bulletScript = Instantiate(bullet.bulletData.bulletPrefab, bulletSpawnPos.position, Quaternion.identity).GetComponent<BulletScript>();
				bulletScript.Init(createdByPlayer: true);
			}
		}
	}
}
