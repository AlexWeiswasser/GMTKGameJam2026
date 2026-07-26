using DG.Tweening;
using UnityEngine;

public class DynamanScript : BehaviorScript
{
	[SerializeField] GameObject explosion;
	[SerializeField] ParticleSystem explosionParticles;

	[SerializeField] Transform spriteObject;
	[SerializeField] float hopDuration;
	[SerializeField] float hopStrength;
	[SerializeField] Ease hopEasing;
	[SerializeField] ParticleSystem landParticles;

	[SerializeField] private AudioClip getAngry;
	[SerializeField] private AudioClip explodes;
	private bool isAngry = false;

	Tween moveTween = null;

	protected override void Start()
	{
		canBehavior = true;
		canMove = true;

		base.Start();
		targetPos = MovementScript.playerTransform;
	}

	private void Update()
	{
		if (Vector2.Distance(transform.position, MovementScript.playerTransform.position) > detectionRange) return;
		if (BulletSelectController.PlayerBusy()) return;
		if(isAngry == false)
		{
			SoundManager.Instance.PlaySound(getAngry);
			isAngry = true;
		}

		if (canBehavior && moveTween == null)
		{
			moveTween = spriteObject.DOLocalJump(Vector2.zero, hopStrength, 1, hopDuration)
				.SetEase(hopEasing)
				.OnComplete(() => { landParticles.Play(); moveTween = null; });
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (TryGetComponent<EnemyScript>(out var eScript)) if (eScript.isWet) return;

		if (collision.transform.CompareTag("Player"))
		{
			if (collision.transform.TryGetComponent<ICharInterface>(out var face))
			{
				Vector2 direction = (collision.transform.position - transform.position).normalized;
				Explode();
				Destroy(gameObject);
			}
		}
	}
	
	public void Explode()
	{
		if (TryGetComponent<EnemyScript>(out var eScript)) if (eScript.isWet) return;
		SoundManager.Instance.PlaySound(explodes);
		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Instantiate(explosion, transform.position, Quaternion.identity); 
	}
}
