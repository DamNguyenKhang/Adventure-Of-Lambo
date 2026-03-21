using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float timeToDestroy = 0.5f; // Thời gian khớp với animation nổ của bạn

    void Start()
    {
        // Tự động xóa chính nó sau 0.5 giây
        Destroy(gameObject, timeToDestroy);
    }
}