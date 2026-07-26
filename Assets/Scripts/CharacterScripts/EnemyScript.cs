using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using DG.Tweening;
using UnityEngine;

public class EnemyScript : MonoBehaviour, ICharInterface
{
	// Stats
	[SerializeField] int health = 50;
	int hitStreak = 0;

	// facts
	int timesFireDamaged = 0;
	[HideInInspector] public bool isMetalMarked = false;
	[HideInInspector] public bool isWet = false;

	// Timers
	Timer fireTimer = null;
	Timer oilTimer = null;
	Timer stunTimer = null;
	Timer metalTimer = null;
	Timer waterTimer = null;

	[Header("References")]
	[SerializeField] BehaviorScript behaviorScript;
	[SerializeField] GameObject healthPickup;
	[SerializeField] GameObject deathEffect;

	[Header("Particles")]
	[SerializeField] ParticleSystem metalParticles;
	[SerializeField] ParticleSystem wetParticles;
	[SerializeField] ParticleSystem stunnedParticles;
	[SerializeField] ParticleSystem onFireParticles;
	[SerializeField] ParticleSystem oilFireParticles;

	// Particle Tracking
	GameObject metalParticlesObject;
	GameObject wetParticlesObject;
	GameObject stunnedParticlesObject;
	GameObject onFireParticlesObject;
	GameObject oilFireParticlesObject;

	[SerializeField] private AudioClip deathSound;



	public void Damage(int amount)
	{
		int damageTaken = isMetalMarked ? Mathf.RoundToInt(amount * 1.25f) : amount;
		health -= damageTaken;

		foreach (SpriteRenderer spr in behaviorScript.rend)
		{
			spr.color = Color.red;
			spr.DOColor(Color.white, 0.2f);
		}

		if (health <= 0)
		{
			if (PlayerScript.health <= PlayerScript.MAX_HEALTH / 2 && UnityEngine.Random.Range(0, 3) == 0)
			{
				Instantiate(healthPickup, transform.position, Quaternion.identity);
			}

			if (TryGetComponent<DynamanScript>(out var dScript)) dScript.Explode();

			SoundManager.Instance.PlaySound(deathSound);
			Instantiate(deathEffect, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}

		EventBus<OnDamageTaken>.Call(new OnDamageTaken() {position = new Vector2(transform.position.x, transform.position.y), damageValue = damageTaken});
	}

	public void Knockback(Vector2 direction, float amount)
	{
		behaviorScript.Knockback(direction, amount);
	}

	public void SetOnFire()
	{
		if (TryGetComponent<DynamanScript>(out var dScript)) dScript.Explode();

		isWet = false;
		behaviorScript.isWet = false;

		if (fireTimer == null)
		{
			onFireParticlesObject = Instantiate(onFireParticles, transform).gameObject;
			fireTimer = new Timer(.5f);
		}
		timesFireDamaged = 0;
	}

	public void SetOnOilFire()
	{
		if (TryGetComponent<DynamanScript>(out var dScript)) dScript.Explode();

		isWet = false;
		behaviorScript.isWet = false;

		if (oilTimer == null)
		{
			oilFireParticlesObject = Instantiate(oilFireParticles, transform).gameObject;
			oilTimer = new Timer(.5f);
		}
	}

	public bool GetOnFire() => fireTimer != null;

	public void SetStunned(HashSet<Collider2D> alreadyHit = null)
	{
		if (isWet || isMetalMarked) Damage(5);

		behaviorScript.canBehavior = false;
		stunTimer = new Timer(2f);
		if (stunnedParticlesObject != null) Destroy(stunnedParticlesObject);
		stunnedParticlesObject = Instantiate(stunnedParticles, transform).gameObject;

		// Sha-lightning 
		alreadyHit ??= new HashSet<Collider2D>();

		foreach (Collider2D own in GetComponentsInChildren<Collider2D>())
			alreadyHit.Add(own);

		List<ICharInterface> toZap = new List<ICharInterface>();

		LayerMask hurtMask = LayerMask.GetMask("Character");
		Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 7.5f, hurtMask);
		foreach (Collider2D col in nearby)
		{
			if (alreadyHit.Contains(col)) continue;
			if (!col.TryGetComponent<ICharInterface>(out var cFace)) continue;
			if (!cFace.GetWet() && !cFace.GetMetalMarked()) continue;

			alreadyHit.Add(col);   
			toZap.Add(cFace);
		}

		foreach (ICharInterface zapped in toZap)
			zapped.SetStunned(alreadyHit);
	}

	public void SetMetalMarked()
	{
		isMetalMarked = true;
		metalTimer = new Timer(5);

		if (metalParticlesObject != null) Destroy(metalParticlesObject);
		metalParticlesObject = Instantiate(metalParticles, transform).gameObject;
	}
	public bool GetMetalMarked() => isMetalMarked;


	public void SetWet()
	{
		isWet = true;
		behaviorScript.isWet = true;
		fireTimer = null;

		if (onFireParticlesObject != null)
		{
			Destroy(onFireParticlesObject);
			onFireParticlesObject = null;
		}

		waterTimer = new Timer(5);

		if (wetParticlesObject != null) Destroy(wetParticlesObject);
		wetParticlesObject = Instantiate(wetParticles, transform).gameObject;
	}
	public bool GetWet() => isWet;


	private void Update()
	{
		// Timers
		if (fireTimer != null)
		{
			fireTimer.Tick(Time.deltaTime);
			if (fireTimer.Finished && timesFireDamaged != 6)
			{
				fireTimer.Reset();
				Damage(1);
				timesFireDamaged++;
			}
			else if (timesFireDamaged >= 6)
			{
				if (onFireParticlesObject != null)
				{
					Destroy(onFireParticlesObject);
					onFireParticlesObject = null;
				}

				fireTimer = null;
			}
		}

		if (stunTimer != null)
		{
			stunTimer.Tick(Time.deltaTime);
			if (stunTimer.Finished)
			{
				stunTimer = null;
				behaviorScript.canBehavior = true;

				if (stunnedParticlesObject != null)
				{
					Destroy(stunnedParticlesObject);
					stunnedParticlesObject = null;
				}
			}
		}

		if (waterTimer != null)
		{
			waterTimer.Tick(Time.deltaTime);
			if (waterTimer.Finished)
			{
				waterTimer = null;
				isWet = false;
				behaviorScript.isWet = false;

				if (wetParticlesObject != null)
				{
					Destroy(wetParticlesObject);
					wetParticlesObject = null;
				}
			}
		}

		if (metalTimer != null)
		{
			metalTimer.Tick(Time.deltaTime);
			if (metalTimer.Finished)
			{
				metalTimer = null;
				isMetalMarked = false;

				if (metalParticlesObject != null)
				{
					Destroy(metalParticlesObject);
					metalParticlesObject = null;
				}
			}
		}

		if (oilTimer != null)
		{
			oilTimer.Tick(Time.deltaTime);
			if (oilTimer.Finished)
			{
				if (oilFireParticlesObject != null)
				{
					Destroy(oilFireParticlesObject);
					oilFireParticlesObject = null;
				}

				oilTimer = null;
				Damage(1);
			}
		}
	}
}
