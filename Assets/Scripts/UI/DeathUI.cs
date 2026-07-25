using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeathUI : MonoBehaviour
{
    [SerializeField] Image fade;
    [SerializeField] RectTransform deathText;
    [SerializeField] RectTransform returnButton;
    void OnEnable()
    {
        PlayerScript.OnPlayerDeath += ShowDeathOverlay;
    }

    void OnDisable()
    {
        PlayerScript.OnPlayerDeath -= ShowDeathOverlay;
    }

    void ShowDeathOverlay()
    {
        fade.enabled = true;
        fade.DOFade(1f, 0.75f);
        deathText.DOAnchorPosY(200, 0.75f);
        returnButton.DOAnchorPosY(-200, 0.75f);
    }
}
