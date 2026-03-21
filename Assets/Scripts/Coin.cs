using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreValue = 1; // Mỗi đồng xu đáng giá bao nhiêu điểm

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu cái chạm vào xu có Tag là "Player"
        if (other.CompareTag("Player"))
        {
            // Gọi hàm cộng điểm từ ScoreManager (chúng ta sẽ tạo ở dưới)
            ScoreManager.instance.AddScore(scoreValue);

            // Biến mất
            Destroy(gameObject);
        }
    }
}