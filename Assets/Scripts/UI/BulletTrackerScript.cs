using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BulletTrackerScript : MonoBehaviour
{
	private void OnEnable()
	{
		EventBus<OnReload>.Subscribe(SetVisuals);
		EventBus<OnBulletFire>.Subscribe(SetVisuals);
	}

	private void OnDisable()
	{
		EventBus<OnReload>.Unsubscribe(SetVisuals);
		EventBus<OnBulletFire>.Unsubscribe(SetVisuals);
	}

    private void Start()
    {
        SetVisuals(new OnReload());
    }

	[SerializeField] Ease easing;
    [Header("References")]
	[SerializeField] RectTransform cylinder;
    [SerializeField] Image[] bulletRenderers;

	private Tween jumpTween;

	private void SetVisuals(OnReload reloadData) 
	{
		for (int i = 0; i < BulletStorageScript.MaximumBulletCount; i++)
		{
			Color color = i < BulletStorageScript.bulletCount ? Color.white : Color.black;
			bulletRenderers[i].color = color;
		}
		
		if (BulletStorageScript.bulletCount > 0) RotateToBullet(BulletStorageScript.bulletCount - 1);

		jumpTween.Kill(true);
		jumpTween = cylinder.DOJumpAnchorPos(cylinder.anchoredPosition, 20, 1, 0.2f);
	}

	private void SetVisuals(OnBulletFire fireData)
	{
		int bulletFired = fireData.bulletNumber;
		//bulletRenderers[0].enabled = false;
		//Debug.Log(bulletFired);
		for (int i = 0; i < BulletStorageScript.MaximumBulletCount; i++)
		{
			Color color = i < bulletFired ? Color.white : Color.black;
			bulletRenderers[i].color = color;
		}
		// for(int i = BulletStorageScript.lastBulletIndex; i >= 0; i--)
		// {
		// 	bulletRenderers[i].enabled = i < bulletFired;
		// }
		if (bulletFired > 0) RotateToBullet(bulletFired - 1);
	}

	private void RotateToBullet(int bullet)
	{
		cylinder.DORotate(new Vector3(0, 0, 60f * bullet), 0.2f, RotateMode.FastBeyond360)
			.SetEase(easing);
	}
}
