using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float speed = 8f;              // Tốc độ di chuyển ngang
    public float jumpForce = 12f;         // Lực nhảy
    public float fireRate = 0.5f;          // Thời gian cách giữa 2 lần bắn (giây)

    [Header("Kiểm tra mặt đất")]
    public Transform feetPos;             // Vị trí đặt dưới chân để kiểm tra đất
    public float circleRadius = 0.3f;     // Bán kính vùng kiểm tra đất
    public LayerMask whatIsGround;        // Chọn Layer của các Object làm đất (Ground)

    [Header("Vũ khí & Bắn")]
    public GameObject Sword;              // Prefab thanh kiếm (đạn)
    public GameObject pos_sword;          // Vị trí spawn kiếm
    
    // Các biến thành phần (Private Components)
    private Rigidbody2D rigidBody;
    private Animator anim;
    
    // Biến trạng thái (State Variables)
    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;
    private float nextFireTime = 0f;

    private void Start()
    {
        // Khởi tạo các thành phần
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. Kiểm tra xem Player có đang chạm đất không
        isGrounded = Physics2D.OverlapCircle(feetPos.position, circleRadius, whatIsGround);
        Debug.Log("Đang chạm đất: " + isGrounded);

        // 2. Nhận input di chuyển ngang (dùng AxisRaw để phản hồi phím nhanh hơn)
        moveInput = Input.GetAxisRaw("Horizontal");

        // 3. Xử lý Nhảy (Dùng phím Mũi Tên Lên hoặc W)
        if (isGrounded && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            Jump();
        }

        // 4. Xử lý Bắn (Phím Space và có thời gian chờ giữa 2 lần bắn)
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
        }

        // 5. Kiểm tra nếu Player rớt khỏi màn hình (chết)
        CheckFallOff();

        // 6. Cập nhật Animation trạng thái (Chạy/Nghỉ)
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        // Thực hiện di chuyển vật lý trong FixedUpdate để ổn định hơn
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        // Gán vận tốc cho Rigidbody2D để di chuyển ngang
        rigidBody.linearVelocity = new Vector2(moveInput * speed, rigidBody.linearVelocity.y);

        // Xử lý Lật (Flip) hướng nhân vật dựa trên input
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Jump()
    {
        // Kích hoạt animation nhảy (Trigger)
        anim.SetTrigger("isJump");
        
        // Áp dụng lực nhảy lên trục Y
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);
    }

    private void Shoot()
    {
        // Cập nhật thời điểm được phép bắn lần tiếp theo
        nextFireTime = Time.time + fireRate;

        // Tạo đối tượng kiếm tại vị trí bắn
        if (Sword != null && pos_sword != null) {
            Instantiate(Sword, pos_sword.transform.position, pos_sword.transform.rotation);
        }

        // Hiệu ứng "đẩy ngược" nhẹ khi bắn
        float kickback = facingRight ? 5f : -5f;
        rigidBody.linearVelocity = new Vector2(kickback, rigidBody.linearVelocity.y);
    }

    private void Flip()
    {
        // Đảo trạng thái hướng mặt
        facingRight = !facingRight;

        // Lật nhân vật bằng cách đảo dấu trục Scale X
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void UpdateAnimations()
    {
        // Chạy animation "Run" nếu có nhấn nút di chuyển trái/phải và đang trên đất
        bool isRunning = Mathf.Abs(moveInput) > 0.1f && isGrounded;
        anim.SetBool("isRun", isRunning);
    }

    private void CheckFallOff()
    {
        // Kiểm tra vị trí Y trên màn hình, nếu xuống dưới đáy màn hình thì chết
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.y < 0)
        {
            Died();
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        // Va chạm với vùng chết (Batas_Mati)
        if (coll.gameObject.CompareTag("Batas_Mati"))
        {
            Died();
        }
    }

    public void Died()
    {
        // Chuyển sang Scene GameOver
        SceneManager.LoadScene("GameOver");
    }
}
