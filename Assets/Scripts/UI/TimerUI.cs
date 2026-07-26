using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] int timeInThisLevel = 90;
    [SerializeField] TextMeshProUGUI timeText;
    private Timer timer;
    private static bool outOfTime = false;
    public static bool IsOutOfTime() { return outOfTime; }

    void Start()
    {
        timer = new Timer(timeInThisLevel);
    }

    public static void Reset()
    {
        outOfTime = false;
    }

    void Update()
    {
        if (BulletSelectController.PlayerBusy() || outOfTime) return;

        timer.Tick(Time.deltaTime);

        timeText.text = ((int)timer.Remaining).ToString();

        if (timer.Finished)
        {
            outOfTime = true;
            EventBus<OnOutOfTime>.Call(new OnOutOfTime());
        }
    }
}
