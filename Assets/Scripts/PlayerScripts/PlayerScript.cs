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

	[Header("References")]
	[SerializeField] MovementScript movementScript;
	[SerializeField] ShootScript shootScript;

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

	private void DamageActions(int amount)
	{
		int damageTaken =isMetalMarked ? Mathf.RoundToInt(amount * 1.25f) : amount;
		health -= damageTaken;
		//isMetalMarked = false;
		//metalTimer = null;

		foreach (SpriteRenderer spr in movementScript.playerSpriteRenderers)
		{
			spr.color = Color.red;
			spr.DOColor(Color.white, 0.2f);
		}

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
			fireTimer = new Timer(.5f);
		timesFireDamaged = 0;
	}

	public void SetOnOilFire()
	{
		isWet = false;
		movementScript.isWet = false;

		if (oilTimer == null)
			oilTimer = new Timer(.5f);
	}

	public bool GetOnFire() => fireTimer != null;

	public void SetStunned(List<Collider2D> thoseAlreadyHit = null)
	{
		movementScript.canMove = false; 
		shootScript.canShoot = false;

		stunTimer = new Timer(.5f);

		// Sha-lightning
		List<ICharInterface> thoseHit = new List<ICharInterface>();
		List<Collider2D> newThoseAlreadyHit = new List<Collider2D>();
		if(thoseAlreadyHit != null)
			newThoseAlreadyHit.AddRange(thoseAlreadyHit);

		LayerMask hurtMask = LayerMask.GetMask("Character");
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 5f, hurtMask);
		foreach (Collider2D col in nearbyColliders)
		{
			if (thoseAlreadyHit != null && thoseAlreadyHit.Contains(col)) continue;
			if (col.transform.TryGetComponent<ICharInterface>(out var cFace) && cFace.GetWet() || cFace.GetMetalMarked())
			{
				thoseHit.Add(cFace);
				newThoseAlreadyHit.Add(col);
			}
		}
		foreach (ICharInterface zapped in thoseHit)
		{
			zapped.SetStunned(newThoseAlreadyHit);
		}
	}

	public void SetMetalMarked()
	{
		isMetalMarked = true;
		metalTimer = new Timer(7);
	}
	public bool GetMetalMarked() => isMetalMarked;

	public void SetWet()
	{
		isWet = true;
		movementScript.isWet = true; 
		fireTimer = null;

		waterTimer = new Timer(5);
	}

	public bool GetWet() => isWet;

	private void Update()
	{
		if (fireTimer != null)
		{
			fireTimer.Tick(Time.deltaTime);
			if(fireTimer.Finished && timesFireDamaged != 6)
			{
				fireTimer.Reset();
				Damage(1);
				timesFireDamaged++;
			}
			else if (timesFireDamaged >= 6)
			{
				fireTimer = null;
			}
		}

		if (stunTimer != null)
		{
			stunTimer.Tick(Time.deltaTime);
			if (stunTimer.Finished)
			{
				fireTimer = null;
				movementScript.canMove = true;
				shootScript.canShoot = true;
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
			}
		}

		if (metalTimer != null)
		{
			metalTimer.Tick(Time.deltaTime);
			if (metalTimer.Finished)
			{
				metalTimer = null;
				isMetalMarked = false; 
			}
		}

		if (oilTimer != null)
		{
			oilTimer.Tick(Time.deltaTime);
			if(oilTimer.Finished)
			{
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
