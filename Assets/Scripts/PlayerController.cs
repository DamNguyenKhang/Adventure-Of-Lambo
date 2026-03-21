using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float speed = 8f;
    public float jumpForce = 12f;
    public float jumpDelay = 0.25f;
    public float fireRate = 0.5f;

    [Header("Kiểm tra mặt đất")]
    public Transform feetPos;
    public float circleRadius = 0.3f;
    public LayerMask whatIsGround;

    [Header("Vũ khí & Bắn")]
    public GameObject SwordPrefab; // Đổi tên để tránh nhầm với class
    public GameObject pos_sword;
    public AudioClip shootSound;

    private Rigidbody2D rigidBody;
    private Animator anim;
    private AudioSource audioSource;

    private float moveInput;
    private bool isGrounded;
    private bool isJumping = false;
    private bool facingRight = true;
    private float nextFireTime = 0f;
    private bool isPreparingJump = false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, circleRadius, whatIsGround);
        moveInput = Input.GetAxisRaw("Horizontal");

        // Nhảy với thời gian nhún (Jump Anticipation)
        if (isGrounded && !isJumping && !isPreparingJump && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            StartCoroutine(JumpRoutine());
        }

        // Bắn
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
        }

        CheckFallOff();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        // Khóa di chuyển khi đang lấy đà nhảy
        if (isPreparingJump)
        {
            rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        }
        else
        {
            rigidBody.linearVelocity = new Vector2(moveInput * speed, rigidBody.linearVelocity.y);
        }

        // Lật mặt
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();
    }

    IEnumerator JumpRoutine()
    {
        isJumping = true;
        isPreparingJump = true;

        if (anim != null) anim.SetTrigger("isJump");

        yield return new WaitForSeconds(jumpDelay);

        isPreparingJump = false;
        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);

        yield return new WaitForSeconds(0.2f);
        isJumping = false;
    }

    void Shoot()
    {
        nextFireTime = Time.time + fireRate;
        if (anim != null) anim.SetTrigger("isShoot");

        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        if (SwordPrefab != null && pos_sword != null)
        {
            // 1. Tạo quả cầu
            GameObject projectile = Instantiate(SwordPrefab, pos_sword.transform.position, Quaternion.identity);

            // 2. Lấy hướng hiện tại (1 hoặc -1)
            float direction = facingRight ? 1f : -1f;

            // 3. Truyền hướng vào script Sword
            Sword swordScript = projectile.GetComponent<Sword>();
            if (swordScript != null)
            {
                swordScript.Initialize(direction);
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void UpdateAnimations()
    {
        // Chỉ chạy khi có nhấn phím, chạm đất và không đang gồng nhảy
        bool isRunning = Mathf.Abs(moveInput) > 0.1f && isGrounded && !isPreparingJump;
        anim.SetBool("isRun", isRunning);

        // Hiệu ứng rơi
        bool isFalling = !isGrounded && rigidBody.linearVelocity.y < -0.1f;
        anim.SetBool("isFall", isFalling);
    }

    void CheckFallOff()
    {
        if (transform.position.y < -15f) Died();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Batas_Mati")) Died();
    }

    public void Died()
    {
        SceneManager.LoadScene("GameOver");
    }

    void OnDrawGizmosSelected()
    {
        if (feetPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(feetPos.position, circleRadius);
        }
    }
}