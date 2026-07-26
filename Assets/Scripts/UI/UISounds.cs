using UnityEngine;

public class UISounds : MonoBehaviour
{
    public AudioClip hover;
    public AudioClip press;
    public AudioClip gun;

    public void HoverSFX()
    {
        SoundManager.Instance.PlaySound(hover);
    }

    public void PressSFX()
    {
        SoundManager.Instance.PlaySound(press);

    }

    public void GunshotUI()
    {
        SoundManager.Instance.PlaySound(gun, .9f, 1.1f, 2f);

    }
}
