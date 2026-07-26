using UnityEditor;
using UnityEngine;

public class MenuProxy : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneLoader.Instance.LoadScene(sceneName);
        PlayerScript.health = 100;
    }

    public void Reset()
    {
        BulletStorageScript.Reset();
        TimerUI.Reset();
    }

    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #endif
    }
}
