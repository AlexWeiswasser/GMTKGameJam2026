using UnityEngine;

public class BulletSelectController : MonoBehaviour
{
    [SerializeField] BulletSelection[] bulletSelections;
    [SerializeField] BulletData[] TEMPORARYSTARTUP;

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
        for (int i = 0; i < 3; i++)
        {
            bulletSelections[i].Init(bulletDatas[i]);
        }
    }

    public void HideSelections(OnBulletsExchangedComplete onBulletsExchangedComplete)
    {
        for (int i = 0; i < 3; i++)
        {
            bulletSelections[i].Remove();
        }
    }
}
