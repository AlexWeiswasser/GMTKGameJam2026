using System.Collections;
using UnityEngine;

public class OilPatchScript : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] SpriteRenderer oilFire;

	bool onFire = false;
	Coroutine fadeAway;

    public void Awake()
    {
        int oilType = UnityEngine.Random.Range(0, 3);
		animator.Play("Oil" + oilType.ToString());
		fadeAway = StartCoroutine(FadeAway());
    }

    public void SetOnFire()
	{
		if (fadeAway != null) StopCoroutine(fadeAway);
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

	private IEnumerator FadeAway()
	{
		yield return new WaitForSeconds(5f);
		Destroy(gameObject);
	}
}