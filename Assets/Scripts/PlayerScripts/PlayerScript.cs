using System;
using UnityEngine;

public class PlayerScript : MonoBehaviour, ICharInterface
{
	// Stats
	public static readonly int MAX_HEALTH = 50;
	int health = 50;
	int hitStreak = 0;

	// facts
	int timesFireDamaged = 0;
	[HideInInspector] public bool isMetalMarked = false;
	[HideInInspector] public bool isWet = false;

	// Events
	public static event Action<int> OnHealthChange;

	// Timers
	Timer fireTimer = null;
	Timer oilTimer = null;
	Timer stunTimer = null;

	[Header("References")]
	[SerializeField] MovementScript movementScript;
	[SerializeField] ShootScript shootScript;

	public void Damage(int amount)
	{
		health -= isMetalMarked ? Mathf.RoundToInt(amount * 1.25f) : amount;
		OnHealthChange?.Invoke(health);
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

	public void SetStunned()
	{
		movementScript.canMove = false; 
		shootScript.canShoot = false;

		stunTimer = new Timer(.5f);
	}

	public void SetMetalMarked()
	{
		isMetalMarked = true;
	}

	public void SetWet()
	{
		isWet = true;
		movementScript.isWet = true; 
		fireTimer = null;
	}

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
}
