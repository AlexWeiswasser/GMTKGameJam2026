using DG.Tweening;
using TMPro;
using UnityEngine;

public class BulletSelection : MonoBehaviour
{
    [SerializeField] BulletTrackerScript bulletTracker;
    [SerializeField] int selection;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] ChamberObject chamberObject;

    private RectTransform rectTransform;
    private BulletData bulletData;
    private Canvas canvas;

    void OnEnable()
    {
        chamberObject.OnDragEnd += CheckToExchange;
        EventBus<OnBulletsExchanged>.Subscribe(ExchangeSuccess);
    }

    void OnDisable()
    {
        chamberObject.OnDragEnd -= CheckToExchange;
        EventBus<OnBulletsExchanged>.Unsubscribe(ExchangeSuccess);
    }

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void Init(BulletData data)
    {
        bulletData = data;
        UpdateText();
        UpdateChamberObject();
        MoveDown();
    }

    public void Remove()
    {
        chamberObject.SetLocked(true);
        MoveUp();
    }

    public void UpdateText()
    {
        titleText.text = bulletData.bulletName;
        descriptionText.text = bulletData.bulletDescription;
    }

    public void UpdateChamberObject()
    {
        chamberObject.SetImage(bulletData.chamberSprite);
        chamberObject.SetLocked(false);
        chamberObject.SetIndex(-1);
        
    }

    public void MoveDown()
    {
        rectTransform.DOAnchorPosY(200, 0.7f)
            .SetEase(Ease.InOutSine);
    }

    public void MoveUp()
    {
        rectTransform.DOAnchorPosY(900, 0.7f)
            .SetEase(Ease.InOutSine);
    }

    private void CheckToExchange(ChamberObject chamberObject)
	{
		foreach (ChamberObject cO in bulletTracker.chamberObjects)
		{
			if (chamberObject == cO) continue;

			if (OverlapsSlot(chamberObject.rectTransform, cO.rectTransform))
			{
				Debug.Log($"Swapping outside {chamberObject.name} and {cO.name} ({cO.GetIndex()})");
				
				RectTransform saved = chamberObject.savedPosition;
				//int firstId = chamberObject.GetIndex();
				int firstId = cO.GetIndex();

				if (BulletStorageScript.lastBulletIndex < firstId)
				{
					Debug.Log("The second bullet is out of range!");
					break;
				}

				BulletStorageScript.ExchangeBullet(firstId, bulletData, selection);
				break;
			}
		}
		
		// Always go back, even if swap was a success
		chamberObject.SetPosition(chamberObject.savedPosition);
	}

    bool OverlapsSlot(RectTransform objectRect, RectTransform slotRect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            slotRect, objectRect.position, canvas.worldCamera);
    }

    private void ExchangeSuccess(OnBulletsExchanged onBulletsExchanged)
    {
        if (onBulletsExchanged.selection != selection) return;
        
        bulletData = onBulletsExchanged.cylinderBulletData;
        UpdateText();
        UpdateChamberObject();
    }
}
