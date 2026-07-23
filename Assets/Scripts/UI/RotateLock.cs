using UnityEngine;

public class RotateLock : MonoBehaviour
{
    void LateUpdate()
    {
        if (transform.rotation != Quaternion.identity)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
