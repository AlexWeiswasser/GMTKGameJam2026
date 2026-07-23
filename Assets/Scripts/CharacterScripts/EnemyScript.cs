using UnityEngine;

public class EnemyScript : ICharInterface
{
	public enum EnemyType
	{
		Rusher,
		Shooter,
		Guardian
	}

	// Stats

	[SerializeField] int health = 50;
	[SerializeField] EnemyType type;
	int hitStreak = 0;

	// facts
	int timesFireDamaged = 0;
	[HideInInspector] public bool isMetalMarked = false;
	[HideInInspector] public bool isWet = false;

	// Timers
	Timer fireTimer = null;
	Timer oilTimer = null;
	Timer stunTimer = null;

	[Header("References")]
	[SerializeField] BehaviorScript behaviorScript;

	public void Damage(int amount) => health -= isMetalMarked ? Mathf.RoundToInt(amount * 1.25f) : amount;

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

	public void SetStunned()
	{
		behaviorScript.canBehavior = false;

		stunTimer = new Timer(.5f);
	}

	public void SetMetalMarked()
	{
		isMetalMarked = true;
	}

	public void SetWet()
	{
		isWet = true;
		behaviorScript.isWet = true;
		fireTimer = null;
	}

	private void Update()
	{
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
