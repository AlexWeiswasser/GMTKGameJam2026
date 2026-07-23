using UnityEngine;

public class FirePatchScript : MonoBehaviour
{
	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.CompareTag("Oil"))
		{
			Vector2 spawnPos = collision.transform.position;
			Vector3 scale = collision.transform.localScale;
			Destroy(collision.gameObject);

			Transform oil = Instantiate(gameObject, transform.position, Quaternion.identity).transform;
			oil.localScale = scale;
		}
	}
}
