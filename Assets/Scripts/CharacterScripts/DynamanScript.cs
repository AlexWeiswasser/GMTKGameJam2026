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

		if (canBehavior && moveTween == null)
		{
			moveTween = spriteObject.DOLocalJump(Vector2.zero, hopStrength, 1, hopDuration)
				.SetEase(hopEasing)
				.OnComplete(() => { landParticles.Play(); moveTween = null; });
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
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
		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Instantiate(explosion, transform.position, Quaternion.identity); 
	}
}
