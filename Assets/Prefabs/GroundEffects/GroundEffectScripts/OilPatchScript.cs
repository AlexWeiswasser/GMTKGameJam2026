using System;
using UnityEngine;

public class OilPatchScript : MonoBehaviour
{
	bool onFire = false; 

	public void SetOnFire()
	{
		Destroy(gameObject, 5f);
		onFire = true; 
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!onFire) return;

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