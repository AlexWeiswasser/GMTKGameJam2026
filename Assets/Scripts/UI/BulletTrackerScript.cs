using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BulletTrackerScript : MonoBehaviour
{
	private void OnEnable()
	{
		EventBus<OnReload>.Subscribe(SetVisuals);
		EventBus<OnBulletFire>.Subscribe(SetVisuals);

		foreach (ChamberObject cO in chamberObjects)
		{
			cO.OnDragEnd += CheckToSwap;
		}
	}

	private void OnDisable()
	{
		EventBus<OnReload>.Unsubscribe(SetVisuals);
		EventBus<OnBulletFire>.Unsubscribe(SetVisuals);

		foreach (ChamberObject cO in chamberObjects)
		{
			cO.OnDragEnd -= CheckToSwap;
		}
	}

    private void Start()
    {
		canvas = GetComponentInParent<Canvas>();
        SetVisuals();
    }

	[System.Serializable]
	public enum TrackerMode
	{
		ShootingMode,
		EditingMode,
	}

	[SerializeField] Ease easing;
    [Header("References")]
	[SerializeField] RectTransform cylinder;
    [SerializeField] Image[] bulletRenderers;
	[SerializeField] ChamberObject[] chamberObjects;

	private Canvas canvas;
	private Tween jumpTween;
	private TrackerMode currentMode = TrackerMode.ShootingMode;

	public void SwitchToEditMode()
	{
		SetMode(TrackerMode.EditingMode);
	}

	public void SetMode(TrackerMode mode)
	{
		currentMode = mode;
		ModeSetup();
	}

	private void ModeSetup()
	{
		Vector3 worldPos = cylinder.position;
		switch (currentMode)
		{
			case TrackerMode.ShootingMode:
				cylinder.pivot = new Vector2(0.5f, 0.5f);
				cylinder.anchorMin = new Vector2(0, 1f);
				cylinder.anchorMax = new Vector2(0, 1f);

				cylinder.position = worldPos;
				cylinder.DOAnchorPos(new Vector2(180, -180), 0.2f)
					.SetEase(easing);
				break;
			case TrackerMode.EditingMode:

				cylinder.pivot = new Vector2(0.5f, 0.5f);
				cylinder.anchorMin = new Vector2(0.5f, 1f);
				cylinder.anchorMax = new Vector2(0.5f, 1f);

				cylinder.position = worldPos;

				cylinder.DOAnchorPos(new Vector2(0, -200), 0.2f)
					.SetEase(easing);
				break;
		}
	}

	private void SetVisuals()
	{
		for (int i = 0; i < BulletStorageScript.MaximumBulletCount; i++)
		{
			Color color = i < BulletStorageScript.bulletCount ? Color.white : Color.gray5;
			bulletRenderers[i].color = color;
		}
	}

	private void SetVisuals(OnReload reloadData) 
	{
		SetVisuals();
		
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
			Color color = i < bulletFired ? Color.white : Color.gray5;
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

	private void CheckToSwap(ChamberObject chamberObject, RectTransform rect)
	{
		bool swapped = false;
		foreach (ChamberObject cO in chamberObjects)
		{
			if (chamberObject == cO) continue;

			Debug.Log($"Checking {chamberObject.name} and {cO.name}");

			if (OverlapsSlot(rect, cO.rectTransform))
			{
				Debug.Log($"Checking {chamberObject.name} and {cO.name}");
				RectTransform saved = chamberObject.savedPosition;

				chamberObject.SetPosition(cO.savedPosition);
				cO.SetPosition(saved);
				swapped = true;
				break;
			}
		}
		
		// Swap Failed
		if (!swapped) chamberObject.SetPosition(chamberObject.savedPosition);
	}
	
    bool OverlapsSlot(RectTransform objectRect, RectTransform slotRect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            slotRect, objectRect.position, canvas.worldCamera);
    }
}
