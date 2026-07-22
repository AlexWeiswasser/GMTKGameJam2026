using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{
	[Header("References")] 
	[SerializeField] Transform bulletSpawnPos;
	[SerializeField] GameObject bullet;

	public void Shoot(InputAction.CallbackContext context)
	{
		if(context.performed)
		{
			Vector2 direction = (bulletSpawnPos.position - transform.position).normalized;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

			Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
		}
	}
}
