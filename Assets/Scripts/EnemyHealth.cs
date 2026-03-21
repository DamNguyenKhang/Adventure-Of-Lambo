using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Chỉ số máu")]
    public float enemyhealth;
    public Slider enemyHealthBar;
    float currhealth;

    [Header("Vật phẩm rơi ra")]
    public bool drops;
    public GameObject thedrop;
    public AudioClip deathKnell;

    // Biến tham chiếu đến Animator
    Animator myAnim;

    // Biến để kiểm tra quái đã chết chưa (tránh đếm điểm nhiều lần)
    bool isDead = false;

    void Start()
    {
        currhealth = enemyhealth;

        // Khởi tạo thanh máu UI
        if (enemyHealthBar != null)
        {
            enemyHealthBar.maxValue = currhealth;
            enemyHealthBar.value = currhealth;
        }

        // Lấy component Animator trên Boss
        myAnim = GetComponent<Animator>();
    }

    // Hàm nhận sát thương
    public void DiDor(float damage)
    {
        // Nếu đã chết thì không nhận thêm sát thương nữa
        if (isDead) return;

        currhealth = currhealth - damage;

        // Cập nhật thanh máu
        if (enemyHealthBar != null)
        {
            enemyHealthBar.value = currhealth;
        }

        // KÍCH HOẠT ANIMATION HURT
        if (myAnim != null)
        {
            myAnim.SetTrigger("Hurt");
        }

        if (currhealth <= 0)
        {
            makeDead();
        }
    }

    public void makeDead()
    {
        // Nếu đã chạy hàm này rồi thì không chạy lại nữa
        if (isDead) return;
        isDead = true;

        // --- CỘNG ĐIỂM DIỆT QUÁI ---
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddKill();
        }

        // 1. Phát âm thanh kết thúc
        if (deathKnell != null)
        {
            AudioSource.PlayClipAtPoint(deathKnell, transform.position);
        }

        // 2. Dừng mọi hoạt động vật lý và di chuyển
        GetComponent<Collider2D>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }

        // 3. Kích hoạt Animation Chết
        if (myAnim != null)
        {
            myAnim.SetTrigger("Dead");
        }

        // 4. Rơi vật phẩm (Nếu có)
        if (drops && thedrop != null)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            Instantiate(thedrop, spawnPos, transform.rotation);
        }

        // 5. Xóa Boss sau một khoảng thời gian
        Destroy(gameObject, 1.5f);
    }
}