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
        rend.DOColor(Color.white, 1f).OnComplete(() =>
        {
            Explode();
        });
    }
	public void Explode()
	{
		Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Instantiate(explosion, transform.position, Quaternion.identity);
	}
}
