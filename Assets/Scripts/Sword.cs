using UnityEngine;

public class Sword : MonoBehaviour
{
    [Header("Cấu hình bay")]
    public float speed = 15f;
    public float damagesenjata = 50f;
    public float lifeTime = 5f;

    [Header("Hiệu ứng tan biến")]
    public GameObject explosionPrefab;

    private Rigidbody2D rb;
    private bool isInitialized = false;
    private float moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Hàm này sẽ được Player gọi ngay sau khi Instantiate
    public void Initialize(float direction)
    {
        moveDirection = direction;
        isInitialized = true;

        // Tự hủy sau lifeTime
        Destroy(gameObject, lifeTime);

        // Xoay hình ảnh theo hướng bay bằng Scale
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * direction;
        transform.localScale = localScale;
    }

    void FixedUpdate()
    {
        if (isInitialized)
        {
            // Duy trì tốc độ bay thẳng
            rb.linearVelocity = new Vector2(moveDirection * speed, 0);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        // 1. Va chạm với Đất hoặc Vật cản (Batas)
        if (coll.CompareTag("Batas") || coll.CompareTag("Ground"))
        {
            CreateExplosion();
        }

        // 2. Va chạm với Kẻ địch (Kiểm tra qua Layer)
        if (coll.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth hurtenemy = coll.gameObject.GetComponent<EnemyHealth>();
            if (hurtenemy != null)
            {
                hurtenemy.DiDor(damagesenjata);
            }
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
}