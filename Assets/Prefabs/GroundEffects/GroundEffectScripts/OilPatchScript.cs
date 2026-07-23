using UnityEngine;

public class OilPatchScript : MonoBehaviour
{
	[SerializeField] private LayerMask fireLayerMask;

	private void Start()
	{
		CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
		Collider2D hit = Physics2D.OverlapCircle(transform.position, circleCollider.radius, fireLayerMask);

		if (hit != null && hit.CompareTag("Fire"))
		{
			Transform newFire = Instantiate(hit.gameObject, transform.position, Quaternion.identity).transform;
			newFire.localScale = transform.localScale;
			Destroy(gameObject);
		}
	}
}
