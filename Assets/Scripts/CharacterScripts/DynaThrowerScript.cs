using DG.Tweening;
using System;
using UnityEngine;

public class DynaThrowerScript : BehaviorScript
{
	[Header("Dynamite")]
	[SerializeField] GameObject dynamite;

	[Header("References")]
	[SerializeField] Transform spriteObject;

	[Header("Stats")]
	[SerializeField] float timeBetweenShots;

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

		shootTimer = new Timer(timeBetweenShots);

		base.Start();
		targetPos = MovementScript.playerTransform;
	}

	private void Update()
	{
		if (BulletSelectController.PlayerBusy() || !canBehavior) return;
		if (Vector2.Distance(transform.position, MovementScript.playerTransform.position) > detectionRange) return;

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
		if (shootTimer.Finished)
		{
			Shoot();
			Recoil();
			shootTimer.Reset();
		}

		if (!animating && rb.linearVelocity.magnitude > 0.2f)
		{
			animating = true;
			spriteObject.DOLocalJump(Vector2.zero, hopStrength, 1, hopDuration)
				.SetEase(hopEasing)
				.OnComplete(() => { animating = false; landParticles.Play(); });
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

		DynamiteScript dynaScript = Instantiate(dynamite, transform.position, Quaternion.identity).GetComponent<DynamiteScript>();
		dynaScript.Movement(MovementScript.playerTransform.position);
	}

	private void Strafe()
	{
		givenDirection = UnityEngine.Random.insideUnitCircle.normalized;
	}
}
