using UnityEngine;

public class SnakeScript : BehaviorScript
{
	[Header("Sprites")]
	[SerializeField] Sprite baseSprite;
	[SerializeField] Sprite madSprite;
	[SerializeField] Sprite[] rattleSprites;

	[Header("References")]
	[SerializeField] CircleCollider2D damageCircle;

	[Header("Stats")]
	[SerializeField] int contactDamage;

	[SerializeField] private AudioSource rattleAudio;
	[SerializeField] private AudioClip getAngry;
	[SerializeField] private AudioClip bite;

	// Tracking stuff
	Timer rattleTimer = new Timer(1f);
	Timer attackTimer = null;
	Timer lungeTimer = null;
	bool playerIsCloseEnough = false;
	bool attacking = false; 

	protected override void Start()
	{
		base.Start();

		canBehavior = true;
		canMove = false; 
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (attacking) return;

		if (collision.CompareTag("Player"))
		{
			playerIsCloseEnough = true;
			if (!rattleAudio.isPlaying)
                rattleAudio.Play();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (attacking) return;

		if (collision.CompareTag("Player"))
		{
			playerIsCloseEnough = false;
			rattleAudio.Stop();

		}
	}

	private void Update()
	{
		if (BulletSelectController.PlayerBusy() || !canBehavior) return;

		if (rattleTimer != null)
		{
			if (playerIsCloseEnough)
			{
				int firstDigit = (int)(rattleTimer.Progress * 10) % 10;
				if (firstDigit % 2 == 0)
					rend[0].sprite = rattleSprites[0];
				else
					rend[0].sprite = rattleSprites[1];

				rattleTimer.Tick(Time.deltaTime);
				if (rattleTimer.Finished)
				{
					attackTimer = new Timer(.5f, startFinished: true);
					attacking = true;
					rend[0].sprite = madSprite;
					rattleTimer = null;
				}
			}
			else
			{
				rend[0].sprite = baseSprite;
				rattleTimer.Reset();
			}
		}

		if(attacking)
		{
			attackTimer.Tick(Time.deltaTime);
			if(attackTimer.Finished)
			{
				attackTimer.Reset();
				SoundManager.Instance.PlaySound(getAngry);
				float time = base.Lunge(MovementScript.playerTransform.position, 20f);
				lungeTimer = new Timer(time);
			}
		}

		if(lungeTimer != null)
		{
			lungeTimer.Tick(Time.deltaTime);
			if(lungeTimer.Finished)
			{
				SoundManager.Instance.PlaySound(bite);
				base.DamageCheck(damageCircle, contactDamage);
				lungeTimer = null;
			}
		}
	}
}
