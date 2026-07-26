using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageNumberController : MonoBehaviour
{
    [SerializeField] GameObject numberPrefab;

    void OnEnable()
    {
        EventBus<OnDamageTaken>.Subscribe(SpawnDamageNumber);
    }

    void OnDisable()
    {
        EventBus<OnDamageTaken>.Unsubscribe(SpawnDamageNumber);
    }

    void SpawnDamageNumber(OnDamageTaken onDamageTaken)
    {
        float xDifference = Random.Range(-1f, 1f);
        Vector2 position = new Vector2(onDamageTaken.position.x + xDifference, onDamageTaken.position.y);
        int number = onDamageTaken.damageValue;

        GameObject numberObj = Instantiate(numberPrefab, position, Quaternion.identity);

        TextMeshPro tmp = numberObj.GetComponent<TextMeshPro>();
        tmp.text = number.ToString();

        RectTransform rect = numberObj.GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence().SetId(this).SetAutoKill(true);
        sequence.Join(tmp.DOFade(0f, 1f));
        sequence.Join(rect.DOAnchorPosY(numberObj.transform.position.y + 2, 1f))
            .OnComplete(() => {Destroy(numberObj);});
        sequence.PlayForward();
    }
}
