using UnityEngine;

public class Sword : MonoBehaviour
{
    [Header("Cấu hình bay")]
    public float speed = 15f;
    public float damagesenjata = 50f; // Sát thương
    public float lifeTime = 5f;

    [Header("Hệ thống Nộ & Đẩy lùi")]
    public bool isBigSkill = false;
    public float knockbackForce = 15f;

    [Header("Hiệu ứng")]
    public GameObject explosionPrefab;

    private Rigidbody2D rb;
    private bool isInitialized = false;
    private float moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float direction)
    {
        moveDirection = direction;
        isInitialized = true;

        // Tự hủy sau một khoảng thời gian
        Destroy(gameObject, lifeTime);

        // --- SỬA LỖI NHỎ XÍU TẠI ĐÂY ---
        // Lấy Scale hiện tại của Prefab (ví dụ: 3 cho đạn to, 1 cho đạn nhỏ)
        Vector3 currentScale = transform.localScale;

        // Chỉ đổi dấu của trục X dựa trên hướng, giữ nguyên độ lớn (Abs)
        // Điều này đảm bảo đạn to vẫn to, đạn nhỏ vẫn nhỏ khi quay đầu
        float newX = Mathf.Abs(currentScale.x) * direction;
        transform.localScale = new Vector3(newX, currentScale.y, currentScale.z);

        // Truyền lực bay
        rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    void Update()
    {
        // Đảm bảo đạn luôn bay thẳng (tránh bị trọng lực làm rơi nếu bạn quên chỉnh Gravity Scale = 0)
        if (isInitialized && rb != null)
        {
            rb.linearVelocity = new Vector2(moveDirection * speed, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Va chạm với Kẻ địch
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.CompareTag("Enemy"))
        {
            // Gây sát thương máu
            EnemyHealth health = collision.GetComponent<EnemyHealth>();
            if (health != null) health.DiDor(damagesenjata);

            // Xử lý logic Nộ và Đẩy lùi
            Enemy movement = collision.GetComponent<Enemy>();
            if (movement != null)
            {
                if (!isBigSkill)
                {
                    // Đạn thường: Tăng nộ
                    PlayerController player = FindObjectOfType<PlayerController>();
                    if (player != null) player.AddRage();
                }
                else
                {
                    // Đạn Ultimate: Đẩy lùi
                    Vector2 pushDir = new Vector2(moveDirection, 0);
                    movement.GetKnockback(pushDir, knockbackForce);
                }
            }

            CreateExplosion();
        }

        // 2. Va chạm với Đất hoặc Vật cản
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.CompareTag("Batas"))
        {
            CreateExplosion();
        }
    }

    void CreateExplosion()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    // Tự xóa khi bay ra khỏi màn hình
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}