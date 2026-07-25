using DG.Tweening;
using System;
using UnityEngine;
using static BulletStorageScript;

public class ShooterScript : BehaviorScript
{
	[Header("Bullets")]
	[SerializeField] GameObject bullet;

	[Header("References")]
	[SerializeField] Transform spriteObject;
	[SerializeField] Transform gunTransform;
	[SerializeField] SpriteRenderer gunSprite;
	[SerializeField] Transform bulletSpawnPos;

	[Header("Tracking")]
	Timer shootTimer = new Timer(3);
	Timer walkTimer = new Timer(1, startFinished: true);
	bool walking = false;
	bool animating = false;

	[Header("Hop Animation")]
	[SerializeField] float hopDuration;
	[SerializeField] float hopStrength;
	//[SerializeField] float hopMagnitudeThreshold;
	[SerializeField] Ease hopEasing;
	[SerializeField] ParticleSystem landParticles;

	public static event Action<bool> OnGunFlip;

	private readonly float MAX_RADIUS = 1f;
	private readonly float MIN_RADIUS = 0.2f;
	private float radius = 1f;

	protected override void Start()
	{
		canBehavior = true;
		canMove = false;

		base.Start();
		targetPos = MovementScript.playerTransform;
	}

	private void Update()
	{
		if (BulletSelectController.PlayerBusy()) return;
		// Moving
		walkTimer.Tick(Time.deltaTime);
		if (walkTimer.Finished)
		{
			if (!walking)
			{
				Strafe();
				canMove = true;
				walking = true;
			}
			else
			{
				canMove = false;
				walking = false;
			}
			walkTimer.Reset();   
		}

		// Shooting
		shootTimer.Tick(Time.deltaTime);
		if(shootTimer.Finished)
		{
			Shoot();
			Recoil();
			shootTimer.Reset();
		}

		// Aiming
		Vector2 direction = (MovementScript.playerTransform.position - transform.position).normalized;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);

		bool flip = angle < -90 || angle > 90 ? true : false;
		gunSprite.flipY = flip;
		OnGunFlip?.Invoke(flip);

		Vector2 offset = direction * radius;
		gunTransform.position = (Vector2)transform.position + offset;

		if (!animating && rb.linearVelocity.magnitude > 0.2f)
		{
			animating = true;
			spriteObject.DOLocalJump(Vector2.zero, hopStrength, 1, hopDuration)
				.SetEase(hopEasing)
				.OnComplete(() => { animating = false; landParticles.Play();});
		}
	}

	private void Recoil()
	{
		DOVirtual.Float(MIN_RADIUS, MAX_RADIUS, 0.2f, (float value) =>
		{
			radius = value;
		});
	}

	private void Shoot()
	{
		if (BulletSelectController.PlayerBusy()) return;
		Vector2 direction = (bulletSpawnPos.position - transform.position).normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		BulletScript bulletScript = Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity).GetComponent<BulletScript>();
		bulletScript.Init(createdByPlayer: false);
	}

	private void Strafe()
	{
		givenDirection = UnityEngine.Random.insideUnitCircle.normalized;
	}
}
