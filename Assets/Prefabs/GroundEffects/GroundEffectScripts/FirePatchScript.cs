using UnityEngine;

public class FirePatchScript : MonoBehaviour
{
	private void Start()
	{
		Destroy(gameObject, 3f);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Character"))
		{
			if(collision.TryGetComponent<ICharInterface>(out var character))
			{
				if(!character.GetOnFire())
					character.SetOnFire();
			}
		}
		else if (collision.CompareTag("Oil"))
		{
			if(collision.TryGetComponent<OilPatchScript>(out var oilScript))
			{
				oilScript.SetOnFire();
			}
		}
	}
}
