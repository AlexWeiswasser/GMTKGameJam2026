using System.Collections;
using UnityEngine;

public class OilPatchScript : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] SpriteRenderer oilFire;
	[SerializeField] AudioClip setOnFire;

	bool onFire = false;
	Coroutine fadeAway;

    public void Awake()
    {
        int oilType = UnityEngine.Random.Range(0, 3);
		animator.Play("Oil" + oilType.ToString());
		fadeAway = StartCoroutine(FadeAway());
    }

	public void SetOnFireInASec()
	{
		Invoke("SetOnFire", .1f);
	}

    public void SetOnFire()
	{
		if (onFire) return;
		if (fadeAway != null) StopCoroutine(fadeAway);
		oilFire.enabled = true;
		animator.SetTrigger("Burn");
		Destroy(gameObject, 5f);
		onFire = true; 
		SoundManager.Instance.PlaySound(setOnFire, .9f, 1.1f, .2f);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!onFire)
		{
			if (collision.CompareTag("Fire"))
			{
				SetOnFire();
			}
			return;
		}

		if (collision.gameObject.layer == LayerMask.NameToLayer("Character"))
		{
			if (collision.TryGetComponent<ICharInterface>(out var character))
			{
				if (!character.GetOnFire())
					character.SetOnFire();
				character.SetOnOilFire();
			}
		}
		if (collision.CompareTag("Oil"))
		{
			if (collision.TryGetComponent<OilPatchScript>(out var oilPatch) && !oilPatch.onFire)
				oilPatch.SetOnFireInASec();
		}
	}

	private IEnumerator FadeAway()
	{
		yield return new WaitForSeconds(5f);
		Destroy(gameObject);
	}
}