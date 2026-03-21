using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float speed = 1.5f;
    public LayerMask enemyMask; // Phải chọn layer "Ground" hoặc layer của Tường

    [Header("Cài đặt tia dò (Raycast)")]
    public float rayLength = 0.5f;      // Độ dài tia bắn ra ngoài thân Boss
    public float rayHeightOffset = 1.0f; // Độ cao của tia (nâng lên khỏi mặt đất)
    public float rayForwardOffset = 0.5f; // Đẩy điểm bắt đầu ra khỏi người Boss

    Rigidbody2D myBody;
    int currentDir = 1;
    float myWidth;

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        // Lấy chiều rộng từ Collider để tia luôn xuất phát từ mép ngoài
        myWidth = GetComponent<Collider2D>().bounds.extents.x;
    }

    void FixedUpdate()
    {
        // 1. Tính toán điểm xuất phát của tia:
        // Phải nâng lên (Y) và đẩy ra trước (X) để không bị chính Boss chặn lại
        Vector2 startPos = new Vector2(
            transform.position.x + (currentDir * (myWidth + rayForwardOffset)),
            transform.position.y + rayHeightOffset
        );

        // 2. Bắn tia kiểm tra tường
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.right * currentDir, rayLength, enemyMask);

        // 3. VẼ TIA ĐỂ KIỂM TRA (Rất quan trọng - Xem trong Scene tab)
        Debug.DrawRay(startPos, Vector2.right * currentDir * rayLength, Color.red);

        // 4. Nếu tia đỏ chạm vào vật cản thuộc Layer trong Enemy Mask
        if (hit.collider != null)
        {
            Debug.Log("Chạm tường: " + hit.collider.name);
            Flip();
        }

        // Luôn tiến về phía trước
        myBody.linearVelocity = new Vector2(currentDir * speed, myBody.linearVelocity.y);
    }

    void Flip()
    {
        currentDir *= -1;
        // Lật scale để toàn bộ hướng nhìn và hướng bắn tia xoay theo
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
}