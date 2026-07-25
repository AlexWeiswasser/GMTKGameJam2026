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
        fade.DOFade(1f, 1.25f);
        deathText.DOAnchorPosY(200, 1.25f);
        returnButton.DOAnchorPosY(-200, 1.25f);
    }
}
