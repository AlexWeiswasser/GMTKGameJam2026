using UnityEditor;
using UnityEngine;

public class MenuProxy : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #endif
    }
}
