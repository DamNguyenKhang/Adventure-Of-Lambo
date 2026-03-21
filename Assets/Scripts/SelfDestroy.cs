using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float delay = 0.5f; // Thời gian chạy hết animation nổ
    void Start()
    {
        Destroy(gameObject, delay);
    }
}
