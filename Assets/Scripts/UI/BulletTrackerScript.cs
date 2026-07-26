using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletTrackerScript : MonoBehaviour
{
	[SerializeField] private AudioClip nextBullet;
	[SerializeField] private AudioClip equipNewBullet;

	private void OnEnable()
	{
		EventBus<OnReload>.Subscribe(SetVisuals);
		EventBus<OnBulletFire>.Subscribe(SetVisuals);
		EventBus<OnBulletsSwapped>.Subscribe(SwapBullets);
		EventBus<OnBulletsExchanged>.Subscribe(ExchangeBullet);

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
		EventBus<OnBulletsExchanged>.Unsubscribe(ExchangeBullet);

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
	[SerializeField] TextMeshProUGUI ammoCount;
	public List<ChamberObject> chamberObjects;

	private Canvas canvas;
	private Tween jumpTween;
	public static TrackerMode currentMode = TrackerMode.ShootingMode;

	public void SwitchToEditMode()
	{
		SetMode(TrackerMode.EditingMode);
	}

	public void SwitchToShootingMode()
	{
		SetMode(TrackerMode.ShootingMode);
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
				if (jumpTween != null) jumpTween.Kill(false);
				cylinder.pivot = new Vector2(0.5f, 0.5f);
				cylinder.anchorMin = new Vector2(0, 1f);
				cylinder.anchorMax = new Vector2(0, 1f);

				cylinder.position = worldPos;
				cylinder.DOAnchorPos(new Vector2(220, -220), 0.2f)
					.SetEase(easing);

				LockAll(true);
				break;
			case TrackerMode.EditingMode:

				if (jumpTween != null) jumpTween.Kill(false);
				BulletStorageScript.Reload();
				cylinder.pivot = new Vector2(0.5f, 0.5f);
				cylinder.anchorMin = new Vector2(0.5f, 0f);
				cylinder.anchorMax = new Vector2(0.5f, 0f);

				cylinder.position = worldPos;

				cylinder.DOAnchorPos(new Vector2(0, 256), 0.2f)
					.SetEase(easing);

				LockAll(false);
				
				break;
		}
	}

	private Color[] textColors =
	{
		new Color(0.6901961f, 0.07058824f, 0.03921569f),
		new Color(1f, 0.4352941f, 0f),
		new Color(1f, 0.5607843f, 0f),
		new Color(0.9921569f, 0.8470588f, 0.2078431f),
		new Color(0.01960784f, 0.4352941f, 0f),
		new Color(0.03921569f, 0.5607843f, 0.03137255f),
		new Color(0.2588235f, 0.7411765f, 0.254902f),
	};

	private void UpdateText(int count)
	{
		ammoCount.text = count.ToString();
		ammoCount.color = textColors[count];
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
		UpdateText(6);
	}

	private void SetVisuals(OnReload reloadData) 
	{
		SetVisuals();
		
		if (BulletStorageScript.bulletCount > 0) RotateToBullet(BulletStorageScript.bulletCount - 1);
		


		jumpTween.Kill(true);
		if (currentMode == TrackerMode.ShootingMode)
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

		UpdateText(bulletFired);

		if (bulletFired > 0) RotateToBullet(bulletFired - 1);
	}

	private void RotateToBullet(int bullet)
	{
		cylinder.DORotate(new Vector3(0, 0, 60f * bullet), 0.2f, RotateMode.FastBeyond360)
    .SetEase(easing)
    .OnComplete(() =>
    {
        SoundManager.Instance.PlaySound(nextBullet);
    });
	}



	private void CheckToSwap(ChamberObject chamberObject)
	{
		bool swapped = false;
		foreach (ChamberObject cO in chamberObjects)
		{
			if (chamberObject == cO) continue;

			if (OverlapsSlot(chamberObject.rectTransform, cO.rectTransform))
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
			SoundManager.Instance.PlaySound(equipNewBullet);

		}
	}

	private void ExchangeBullet(OnBulletsExchanged onBulletsExchanged)
	{
		chamberObjects[onBulletsExchanged.firstBullet].SetImage(onBulletsExchanged.incomingBulletData.chamberSprite);
		SoundManager.Instance.PlaySound(equipNewBullet);

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
