using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Transform gunTransform;
	[SerializeField] SpriteRenderer gunSprite;

	public static event Action<bool> OnGunFlip;

	private readonly float MAX_RADIUS = 0.65f;
	private static readonly float MIN_RADIUS = 0.2f;
	private static readonly float MEGA_MAX_RADIUS = 1.3f;
	private static float radius = 0.65f;

	private void OnEnable()
	{
		EventBus<OnBulletFire>.Subscribe(Recoil);
		EventBus<StartReload>.Subscribe(Reload);
	}

    private void OnDisable()
    {
        EventBus<OnBulletFire>.Unsubscribe(Recoil);
		EventBus<StartReload>.Unsubscribe(Reload);
	}

	private void Update()
	{
		if (BulletSelectController.PlayerBusy()) return;
		if (PauseUI.IsPaused()) return;

		Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
		Vector2 cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);
		Vector2 direction = (cursorWorldPos - (Vector2)transform.position).normalized;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);

		bool flip = angle < -90 || angle > 90 ? true : false;
		gunSprite.flipY = flip;
		OnGunFlip?.Invoke(flip);

		Vector2 offset = direction * radius;
		gunTransform.position = (Vector2)transform.position + offset;
	}

	private void Recoil(OnBulletFire onBulletFire)
	{
		DOVirtual.Float(MIN_RADIUS, MAX_RADIUS, 0.2f, (float value) =>
		{
			radius = value;
		});
	}

	public static void MegaRecoil()
	{
		DOVirtual.Float(MIN_RADIUS, MEGA_MAX_RADIUS, 0.2f, (float value) =>
		{
			radius = value;
		});
	}

	private void Reload(StartReload reloadData)
	{
		Invoke("ReloadDone", reloadData.reloadTime);
	}

	private void ReloadDone()
	{
		BulletStorageScript.Reload();
	}
}
