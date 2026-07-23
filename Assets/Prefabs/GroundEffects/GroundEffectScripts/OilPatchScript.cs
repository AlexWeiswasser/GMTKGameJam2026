using UnityEngine;

public class OilPatchScript : MonoBehaviour
{
	[SerializeField] private LayerMask fireLayerMask;

	private void Start()
	{
		CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
		float radius = circleCollider.radius * transform.lossyScale.x;

		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, fireLayerMask);

		foreach (Collider2D hit in hits)
		{
			if (hit.CompareTag("Fire"))
			{
				Transform newFire = Instantiate(hit.gameObject, transform.position, Quaternion.identity).transform;
				newFire.localScale = transform.localScale;
				Destroy(gameObject);
				break; 
			}
		}
	}
}