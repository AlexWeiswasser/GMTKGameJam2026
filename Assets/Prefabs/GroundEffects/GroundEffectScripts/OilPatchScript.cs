using System;
using UnityEngine;

public class OilPatchScript : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] SpriteRenderer oilFire;

	bool onFire = false;

    public void Awake()
    {
        int oilType = UnityEngine.Random.Range(0, 3);
		animator.Play("Oil" + oilType.ToString());
    }

    public void SetOnFire()
	{
		oilFire.enabled = true;
		animator.SetTrigger("Burn");
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