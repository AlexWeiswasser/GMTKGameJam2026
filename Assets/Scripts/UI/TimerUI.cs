using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] int timeInThisLevel = 90;
    [SerializeField] TextMeshProUGUI timeText;
    private Timer timer;
    private static bool outOfTime = false;
    public static bool IsOutOfTime() { return outOfTime; }

    void OnEnable()
    {
        EventBus<OnSceneBeganLoad>.Subscribe(Pause);
    }

    void OnDisable()
    {
        EventBus<OnSceneBeganLoad>.Unsubscribe(Pause);
    }

    void Start()
    {
        timer = new Timer(timeInThisLevel);
    }

    public static void Reset()
    {
        outOfTime = false;
    }

    void Pause(OnSceneBeganLoad onSceneBeganLoad)
    {
        timer.Pause();
    }

    void Update()
    {
        if (BulletSelectController.PlayerBusy() || outOfTime) return;

        timer.Tick(Time.deltaTime);

        timeText.text = ((int)timer.Remaining + 1).ToString();

        if (timer.Finished)
        {
            outOfTime = true;
            EventBus<OnOutOfTime>.Call(new OnOutOfTime());
        }
    }
}
