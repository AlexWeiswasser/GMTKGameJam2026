using System.Collections.Generic;
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

	public void Damage(int amount)
	{
		int damageTaken = isMetalMarked ? Mathf.RoundToInt(amount * 1.25f) : amount;
		health -= damageTaken;
		//isMetalMarked = false;
		//metalTimer = null;

		EventBus<OnDamageTaken>.Call(new OnDamageTaken() {position = new Vector2(transform.position.x, transform.position.y), damageValue = damageTaken});
	}

	public void Knockback(Vector2 direction, float amount)
	{
		behaviorScript.Knockback(direction, amount);
	}

	public void SetOnFire()
	{
		isWet = false;
		behaviorScript.isWet = false;

		if (fireTimer == null)
			fireTimer = new Timer(.5f);
		timesFireDamaged = 0;
	}

	public void SetOnOilFire()
	{
		isWet = false;
		behaviorScript.isWet = false;

		if (oilTimer == null)
			oilTimer = new Timer(.5f);
	}

	public bool GetOnFire() => fireTimer != null;

	public void SetStunned(List<Collider2D> thoseAlreadyHit = null)
	{
		behaviorScript.canBehavior = false;
		stunTimer = new Timer(.5f);

		// Sha-lightning
		List<ICharInterface> thoseHit = new List<ICharInterface>();
		List<Collider2D> newThoseAlreadyHit = new List<Collider2D>();
		if (thoseAlreadyHit != null)
			newThoseAlreadyHit.AddRange(thoseAlreadyHit);

		LayerMask hurtMask = LayerMask.GetMask("Character");
		Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 10f, hurtMask);
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
		behaviorScript.isWet = true;
		fireTimer = null;

		waterTimer = new Timer(5);
	}
	public bool GetWet() => isWet;


	private void Update()
	{
		// Health
		if (health <= 0) { Destroy(gameObject); }

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
				fireTimer = null;
			}
		}

		if (stunTimer != null)
		{
			stunTimer.Tick(Time.deltaTime);
			if (stunTimer.Finished)
			{
				fireTimer = null;
				behaviorScript.canBehavior = false;
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
			if (oilTimer.Finished)
			{
				oilTimer = null;
				Damage(1);
			}
		}
	}
}
