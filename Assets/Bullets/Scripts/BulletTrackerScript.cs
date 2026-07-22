using UnityEngine;

public class BulletTrackerScript : MonoBehaviour
{
	private void OnEnable()
	{
		EventBus<OnBulletGain>.Subscribe(SetVisuals);
		EventBus<OnBulletFire>.Subscribe(SetVisuals);
	}

	private void OnDisable()
	{
		EventBus<OnBulletGain>.Unsubscribe(SetVisuals);
		EventBus<OnBulletFire>.Unsubscribe(SetVisuals);
	}

	[Header("References")]
    [SerializeField] SpriteRenderer[] bulletRenderers;

	private void SetVisuals(OnBulletGain gainData) { bulletRenderers[gainData.bulletNumber].enabled = true; }
	private void SetVisuals(OnBulletFire fireData)
	{
		bulletRenderers[fireData.bulletNumber].enabled = false; 

		for(int i = fireData.bulletNumber + 1; i < bulletRenderers.Length; i++)
		{
			if (bulletRenderers[i].enabled)
			{
				bulletRenderers[i].enabled = false;
				bulletRenderers[i - 1].enabled = true;
			}
		}
	}
}
