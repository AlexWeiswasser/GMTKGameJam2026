using System;
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
        // optional: bring to front, play a sound, etc.
        rectTransform.rotation = Quaternion.identity;
        transform.parent = canvas.transform;
        rectTransform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // eventData.delta is the mouse movement since last frame, in screen pixels
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // optional: snap to slot, save position, etc.
        OnDragEnd?.Invoke(this, rectTransform);
    }

    public void SetPosition(RectTransform saved)
    {
        savedPosition = saved;
        transform.parent = parent;
        rectTransform.anchoredPosition = savedPosition.anchoredPosition;
        rectTransform.rotation = savedPosition.rotation;
    }

    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
}
