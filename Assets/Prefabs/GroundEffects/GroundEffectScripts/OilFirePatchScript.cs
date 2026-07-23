using UnityEngine;

public class OilFirePatchScript : MonoBehaviour
{
	private void Start()
	{
		Destroy(gameObject, 5f);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Character"))
		{
			if (collision.TryGetComponent<ICharInterface>(out var character))
			{
				if (!character.GetOnFire())
					character.SetOnFire();
				character.SetOnOilFire();
			}
		}
	}


}
