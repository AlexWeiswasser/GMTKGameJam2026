using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] GameObject[] pauseMenuObjects;

    private static bool paused = false;
    public static bool IsPaused() {return paused;}

    void Start()
    {
        paused = false;
        TogglePause(new OnPause() {pause = false});
    }

    void OnEnable()
    {
        EventBus<OnPause>.Subscribe(TogglePause);
    }

    void OnDisable()
    {
        EventBus<OnPause>.Unsubscribe(TogglePause);
    }

    void TogglePause(OnPause onPause)
    {
        paused = onPause.pause;
        Time.timeScale = paused ? 0f : 1f;
        foreach (GameObject obj in pauseMenuObjects)
        {
            obj.SetActive(paused);
        }
    }

    public void UnPause()
    {
        EventBus<OnPause>.Call(new OnPause() { pause = false});
    }

    public void ReturnToTitle()
    {
        TogglePause(new OnPause() {pause = false});
        SceneLoader.Instance.LoadScene("Title");
    }
}
