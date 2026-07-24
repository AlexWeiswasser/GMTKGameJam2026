using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChamberObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public RectTransform savedPosition;
    [HideInInspector] public RectTransform rectTransform;
    
    public event Action<ChamberObject, RectTransform> OnDragEnd;

    Canvas canvas;
    Transform parent;
    Image image;

    private int bulletIndex;
    private bool locked = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        parent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (locked) return;
        if (BulletStorageScript.GetBulletData(bulletIndex) == null) return;

        rectTransform.rotation = Quaternion.identity;
        rectTransform.SetParent(canvas.transform);
        rectTransform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (locked) return;
        if (BulletStorageScript.GetBulletData(bulletIndex) == null) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (locked) return;
        if (BulletStorageScript.GetBulletData(bulletIndex) == null) return;

        OnDragEnd?.Invoke(this, rectTransform);
    }

    public void SetIndex(int index) { bulletIndex = index;}
    public int GetIndex() { return bulletIndex; }

    public void SetPosition(RectTransform saved)
    {
        savedPosition = saved;
        rectTransform.SetParent(parent);
        //rectTransform.anchoredPosition = savedPosition.anchoredPosition;
        rectTransform.DOAnchorPos(savedPosition.anchoredPosition, 0.2f)
            .SetEase(Ease.OutQuart);
        rectTransform.rotation = savedPosition.rotation;
    }

    public void SetLocked(bool locked)
    {
        this.locked = locked;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
}
