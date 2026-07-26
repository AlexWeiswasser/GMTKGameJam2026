using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] RectTransform image;
    [SerializeField] float animationDuration;
    [SerializeField] Ease easing;

    private readonly int imageYDistance = 1080;

    private bool animationComplete = false;
    private bool loading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    public void LoadScene(string sceneName)
    {
        if (loading) return;
        loading = true;
        EventBus<OnSceneBeganLoad>.Call(new OnSceneBeganLoad());
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        image.anchoredPosition = new Vector2(0, -imageYDistance);

        image.DOAnchorPosY(0, animationDuration)
            .SetEase(easing)
            .OnComplete(() => { animationComplete = true; });

        while (!op.isDone)
        {
            if (op.progress >= 0.9f && animationComplete)
            {
                op.allowSceneActivation = true;
                animationComplete = false;
            }
            yield return null;
        }
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (loading)
        {
            loading = false;
            
            image.DOAnchorPosY(imageYDistance, animationDuration)
                .SetEase(easing);
        }
    }
}
