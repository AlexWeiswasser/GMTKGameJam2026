using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChamberObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public RectTransform savedPosition;
    [HideInInspector] public RectTransform rectTransform;
    Canvas canvas;
    Transform parent;

    public event Action<ChamberObject, RectTransform> OnDragEnd;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        parent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        rectTransform.rotation = Quaternion.identity;
        rectTransform.SetParent(canvas.transform);
        rectTransform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke(this, rectTransform);
    }

    public void SetPosition(RectTransform saved)
    {
        savedPosition = saved;
        rectTransform.SetParent(parent);
        //rectTransform.anchoredPosition = savedPosition.anchoredPosition;
        rectTransform.DOAnchorPos(savedPosition.anchoredPosition, 0.2f)
            .SetEase(Ease.OutQuart);
        rectTransform.rotation = savedPosition.rotation;
    }

    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
}
