using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BulletSelectController : MonoBehaviour
{
    [SerializeField] BulletTrackerScript trackerScript;
    [SerializeField] BulletSelection[] bulletSelections;
    [SerializeField] RectTransform confirmButton;
    [SerializeField] RectTransform titleText;
    [SerializeField] RectTransform tipText;
    [SerializeField] Image fade;
    [SerializeField] BulletData[] TEMPORARYSTARTUP;

    bool editing = false;
    bool selectionsVisible = false;

    void OnEnable()
    {
        EventBus<OnBulletsExchangedComplete>.Subscribe(HideSelections);
    }

    void OnDisable()
    {
        EventBus<OnBulletsExchangedComplete>.Unsubscribe(HideSelections);
    }

    void Start()
    {
        ShowSelections(TEMPORARYSTARTUP);
    }

    public void ShowSelections(BulletData[] bulletDatas)
    {
        trackerScript.SetMode(BulletTrackerScript.TrackerMode.EditingMode);
        fade.enabled = true;
        editing = true;
        for (int i = 0; i < 3; i++)
        {
            bulletSelections[i].Init(bulletDatas[i]);
        }
        selectionsVisible = true;

        confirmButton.DOAnchorPosY(-300, 0.7f)
            .SetEase(Ease.InOutSine);
        titleText.DOAnchorPosY(500, 0.7f)
            .SetEase(Ease.InOutSine);
        tipText.DOAnchorPosY(-300, 0.7f)
            .SetEase(Ease.InOutSine);
        fade.DOFade(0.8f, 0.7f);
    }

    public void HideSelections(OnBulletsExchangedComplete onBulletsExchangedComplete)
    {
        for (int i = 0; i < 3; i++)
        {
            bulletSelections[i].Remove();
        }
        titleText.DOAnchorPosY(1200, 0.7f)
            .SetEase(Ease.InOutSine);
        selectionsVisible = false;
    }

    public void DoneEditing()
    {
        if (!editing) return;
        if (selectionsVisible)

        {
            for (int i = 0; i < 3; i++)
            {
                bulletSelections[i].Remove();
            }
            titleText.DOAnchorPosY(1200, 0.7f)
                .SetEase(Ease.InOutSine);
            selectionsVisible = false;
        }
        trackerScript.SetMode(BulletTrackerScript.TrackerMode.ShootingMode);
        confirmButton.DOAnchorPosY(-900, 0.7f)
            .SetEase(Ease.InOutSine);
        tipText.DOAnchorPosY(-900, 0.7f)
            .SetEase(Ease.InOutSine);
        fade.DOFade(0f, 0.7f)
            .OnComplete(() => fade.enabled = false);
    }
}
