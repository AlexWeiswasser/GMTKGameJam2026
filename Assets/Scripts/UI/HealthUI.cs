using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] RectTransform healthFill;
    [SerializeField] TextMeshProUGUI healthText;

    readonly int MinAnchorSize = 25;
    readonly int MaxAnchorSize = 720;
    int Difference => MaxAnchorSize - MinAnchorSize;

    void Start()
    {
        DisplayHealth(PlayerScript.MAX_HEALTH);
    }

    void OnEnable()
    {
        PlayerScript.OnHealthChange += DisplayHealth;
    }

    void OnDisable()
    {
        PlayerScript.OnHealthChange -= DisplayHealth;
    }

    void DisplayHealth(int health)
    {
        healthText.text = $"{health}/{PlayerScript.MAX_HEALTH}";
        float ratio = Mathf.Clamp01((float)health / PlayerScript.MAX_HEALTH);
        float width = MinAnchorSize + Difference * ratio;

        Vector2 size = healthFill.sizeDelta;
        size.x = width;
        healthFill.sizeDelta = size;
    }
}
