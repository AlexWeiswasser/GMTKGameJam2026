using UnityEngine;
using DG.Tweening;

public class DynamiteScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
	[SerializeField] GameObject explosion;
	[SerializeField] ParticleSystem explosionParticles;

	void Start()
    {
        ExplosionAnimation();
    }

    public void ExplosionAnimation()
    {
		Color faded = Color.Lerp(rend.color, Color.clear, .5f);

		rend.DOColor(faded, 1f/6f)
			.SetLoops(6, LoopType.Yoyo) 
			.OnComplete(() =>
			{
				Explode();
			});
	}

	public void Explode()
	{
		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
