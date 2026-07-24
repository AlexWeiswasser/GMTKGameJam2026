using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BulletTrackerScript : MonoBehaviour
{
	private void OnEnable()
	{
		EventBus<OnReload>.Subscribe(SetVisuals);
		EventBus<OnBulletFire>.Subscribe(SetVisuals);
		EventBus<OnBulletsSwapped>.Subscribe(SwapBullets);

		foreach (ChamberObject cO in chamberObjects)
		{
			cO.OnDragEnd += CheckToSwap;
		}
	}

	private void OnDisable()
	{
		EventBus<OnReload>.Unsubscribe(SetVisuals);
		EventBus<OnBulletFire>.Unsubscribe(SetVisuals);
		EventBus<OnBulletsSwapped>.Unsubscribe(SwapBullets);

		foreach (ChamberObject cO in chamberObjects)
		{
			cO.OnDragEnd -= CheckToSwap;
		}
	}

    private void Start()
    {
		canvas = GetComponentInParent<Canvas>();

		for (int i = 0; i < chamberObjects.Count; i++)
		{
			chamberObjects[i].SetIndex(i);
		}

        SetVisuals();

		if (BulletStorageScript.bulletCount > 0) RotateToBullet(BulletStorageScript.bulletCount - 1);
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
	[SerializeField] Sprite emptySprite;
	[SerializeField] List<ChamberObject> chamberObjects;

	private Canvas canvas;
	private Tween jumpTween;
	public static TrackerMode currentMode = TrackerMode.ShootingMode;

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

				LockAll(true);
				break;
			case TrackerMode.EditingMode:
				BulletStorageScript.Reload();
				cylinder.pivot = new Vector2(0.5f, 0.5f);
				cylinder.anchorMin = new Vector2(0.5f, 1f);
				cylinder.anchorMax = new Vector2(0.5f, 1f);

				cylinder.position = worldPos;

				cylinder.DOAnchorPos(new Vector2(0, -200), 0.2f)
					.SetEase(easing);

				LockAll(false);
				
				break;
		}
	}

	private void SetVisuals()
	{
		for (int i = 0; i < BulletStorageScript.MaximumBulletCount; i++)
		{
			Color color = i < BulletStorageScript.bulletCount ? Color.white : Color.gray5;
			chamberObjects[i].SetColor(color);
			
			BulletData data = BulletStorageScript.GetBulletData(i);
			if (data == null)
			{
				chamberObjects[i].SetImage(emptySprite);
			}
			else
			{
				chamberObjects[i].SetImage(data.chamberSprite);
			}
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

		for (int i = 0; i < BulletStorageScript.MaximumBulletCount; i++)
		{
			Color color = i < bulletFired ? Color.white : Color.gray5;
			chamberObjects[i].SetColor(color);
		}

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

			if (OverlapsSlot(rect, cO.rectTransform))
			{
				Debug.Log($"Swapping {chamberObject.name} ({chamberObject.GetIndex()}) and {cO.name} ({cO.GetIndex()})");
				
				RectTransform saved = chamberObject.savedPosition;
				int firstId = chamberObject.GetIndex();
				int secondId = cO.GetIndex();

				if (BulletStorageScript.lastBulletIndex < secondId)
				{
					Debug.Log("The second bullet is out of range!");
					break;
				}

				BulletStorageScript.SwapBullets(firstId, secondId);
				swapped = true;
				break;
			}
		}
		
		// Swap Failed
		if (!swapped) chamberObject.SetPosition(chamberObject.savedPosition);
	}

	private void SwapBullets(OnBulletsSwapped onBulletsSwapped)
	{
		ChamberObject bulletOne = chamberObjects.FirstOrDefault(e => e.GetIndex() == onBulletsSwapped.firstBullet);
		ChamberObject bulletTwo = chamberObjects.FirstOrDefault(e => e.GetIndex() == onBulletsSwapped.secondBullet);

		if (bulletOne != null && bulletTwo != null)
		{
			RectTransform saved = bulletOne.savedPosition;
			
			bulletOne.SetPosition(bulletTwo.savedPosition);
			bulletOne.SetIndex(onBulletsSwapped.secondBullet);

			bulletTwo.SetPosition(saved);
			bulletTwo.SetIndex(onBulletsSwapped.firstBullet);

			(chamberObjects[onBulletsSwapped.firstBullet], chamberObjects[onBulletsSwapped.secondBullet]) = (chamberObjects[onBulletsSwapped.secondBullet], chamberObjects[onBulletsSwapped.firstBullet]);
		}
	}

	private void LockAll(bool locked)
	{
		foreach (ChamberObject chamberObject in chamberObjects)
		{
			chamberObject.SetLocked(locked);
		}
	}
	
    bool OverlapsSlot(RectTransform objectRect, RectTransform slotRect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            slotRect, objectRect.position, canvas.worldCamera);
    }
}
