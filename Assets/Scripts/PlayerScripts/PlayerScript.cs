using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour, ICharInterface
{
	// Stats
	public static readonly int MAX_HEALTH = 100;
	public static int health = 100;
	int hitStreak = 0;
	private static bool dead = false;
	public static bool IsDead() { return dead; }

	// facts
	int timesFireDamaged = 0;
	[HideInInspector] public bool isMetalMarked = false;
	[HideInInspector] public bool isWet = false;

	// Events
	public static event Action<int> OnHealthChange;
	public static event Action OnPlayerDeath;

	// Timers
	Timer fireTimer = null;
	Timer oilTimer = null;
	Timer stunTimer = null;
	Timer metalTimer = null;
	Timer waterTimer = null;

	//Sounds
	[SerializeField] private AudioClip[] playerHurt;
	[SerializeField] private AudioClip playerDeath;
	[SerializeField] private AudioClip playerWalk;
	[SerializeField] private AudioClip setOnFire;

	[Header("References")]
	[SerializeField] MovementScript movementScript;
	[SerializeField] ShootScript shootScript;

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

	void Start()
	{
		dead = false;
	}

	void OnEnable()
	{
		EventBus<OnOutOfTime>.Subscribe(InstaKill);
	}

	void OnDisable()
	{
		EventBus<OnOutOfTime>.Unsubscribe(InstaKill);
	}

	public void InstaKill(OnOutOfTime onOutOfTime)
	{
		DamageActions(999);
	}

	public void Damage(int amount)
	{
		if (BulletSelectController.PlayerBusy()) return;

		DamageActions(amount);
	}

	public static void Heal(int amount)
	{
		health += amount;
		if (health > MAX_HEALTH) health = MAX_HEALTH;
		OnHealthChange?.Invoke(health);
	}

	private void DamageActions(int amount)
	{
		int damageTaken = isMetalMarked ? Mathf.RoundToInt(amount * 1.25f) : amount;
		health -= damageTaken;
		//isMetalMarked = false;
		//metalTimer = null;

		foreach (SpriteRenderer spr in movementScript.playerSpriteRenderers)
		{
			spr.color = Color.red;
			spr.DOColor(Color.white, 0.2f);
		}

		int choice = -1;
		choice = UnityEngine.Random.Range(0, playerHurt.Length);
		if(choice != -1)
			SoundManager.Instance.PlaySound(playerHurt[choice]);

		EventBus<OnDamageTaken>.Call(new OnDamageTaken() {position = new Vector2(transform.position.x, transform.position.y), damageValue = damageTaken});

		if (health <= 0)
		{
			dead = true;
			health = 0;
			transform.rotation = Quaternion.Euler(0, 0, 90);
			OnPlayerDeath?.Invoke();
		}

		OnHealthChange?.Invoke(health);
	}

	public void Knockback(Vector2 direction, float amount)
	{
		movementScript.Knockback(direction, amount);
	}

	public void SetOnFire()
	{
		isWet = false;
		movementScript.isWet = false;

		if (fireTimer == null)
		{
			onFireParticlesObject = Instantiate(onFireParticles, transform).gameObject;
			fireTimer = new Timer(.5f);
			SoundManager.Instance.PlaySound(setOnFire);
		}

		timesFireDamaged = 0;
	}

	public void SetOnOilFire()
	{
		isWet = false;
		movementScript.isWet = false;

		if (oilTimer == null)
		{
			oilFireParticlesObject = Instantiate(oilFireParticles, transform).gameObject;
			oilTimer = new Timer(.5f);
			SoundManager.Instance.PlaySound(setOnFire);
		}
	}

	public bool GetOnFire() => fireTimer != null;

	public void SetStunned(HashSet<Collider2D> alreadyHit = null)
	{
		if (isWet || isMetalMarked) Damage(5);

		movementScript.canMove = false;
		shootScript.canShoot = false; 
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
		movementScript.isWet = true; 
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
				movementScript.canMove = true;
				shootScript.canShoot = true;

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
				movementScript.isWet = false;

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

	public void TogglePause(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			EventBus<OnPause>.Call(new OnPause() {pause = !PauseUI.IsPaused()});
		}
	}
}
