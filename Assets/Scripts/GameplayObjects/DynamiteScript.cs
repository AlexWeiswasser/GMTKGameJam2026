using UnityEngine;
using DG.Tweening;

public class DynamiteScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
	[SerializeField] GameObject explosion;
	[SerializeField] ParticleSystem explosionParticles;
	[SerializeField] Collider2D col;
	bool defused = false;
	Tween bombTween = null;

	public void Movement(Vector2 goalPosition)
	{
		Vector3 startScale = transform.localScale;
		Vector3 goal = new Vector3(goalPosition.x, goalPosition.y, transform.position.z);

		DOTween.Sequence()
			.Append(transform.DOMove(goal, 2).SetEase(Ease.InOutSine))
			.Join(transform.DOScale(startScale * 2f, 2 * 0.5f)
				.SetLoops(2, LoopType.Yoyo)
				.SetEase(Ease.InOutSine))
			.OnComplete(() =>
			{
				col.enabled = true;
				ExplosionAnimation();
			});
	}

	public void Defuse()
	{
		defused = true;
		if (bombTween != null)
		{
			bombTween.Kill();
			bombTween = null;
		}
	}

    public void ExplosionAnimation()
    {
		Color faded = Color.Lerp(rend.color, Color.clear, .5f);

		bombTween = rend.DOColor(faded, 1f/6f)
			.SetLoops(6, LoopType.Yoyo) 
			.OnComplete(() =>
			{
				Explode();
			});
	}

	public void Explode()
	{
		if (defused) return;

		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
