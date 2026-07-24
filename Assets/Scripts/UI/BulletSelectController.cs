using UnityEngine;

public class BulletSelectController : MonoBehaviour
{
    [SerializeField] BulletSelection[] bulletSelections;
    [SerializeField] BulletData[] TEMPORARYSTARTUP;

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

    public void HideSelections()
    {
        for (int i = 0; i < 3; i++)
        {
            bulletSelections[i].Remove();
        }
    }
}
