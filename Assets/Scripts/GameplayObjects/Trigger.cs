using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] UnityEvent trigger;

    public void TriggerEvent()
    {
        trigger?.Invoke();
    }
}
