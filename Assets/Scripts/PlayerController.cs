using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float speed = 8f;
    public float jumpForce = 12f;
    public float jumpDelay = 0.25f;
    public float fireRate = 0.5f;

    [Header("Hệ thống Nộ (Rage)")]
    public float currentRage = 0f;
    public float maxRage = 100f;
    public float rageGainPerHit = 10f;
    public GameObject BigSwordPrefab;
    public Slider rageSlider;

    [Header("Cấu hình Delay Chưởng Thường")]
    public float castPointDelay = 0.15f;
    public float attackRecovery = 0.3f;

    [Header("Cấu hình Gồng Ultimate (R)")]
    [Tooltip("Thời gian gồng lâu hơn cho chiêu cuối")]
    public float ultimateCastDelay = 0.8f;
    [Tooltip("Thời gian khựng lại sau khi tung chiêu cuối")]
    public float ultimateRecovery = 0.5f;
    [Tooltip("Tốc độ animation khi gồng (ví dụ 0.3 là chậm lại 70%)")]
    public float ultimateAnimSpeed = 0.3f;

    [Header("Kiểm tra mặt đất")]
    public Transform feetPos;
    public float circleRadius = 0.3f;
    public LayerMask whatIsGround;

    [Header("Vũ khí & Bắn")]
    public GameObject SwordPrefab;
    public GameObject pos_sword;
    public AudioClip shootSound;
    public AudioClip bigShootSound;

    private Rigidbody2D rigidBody;
    private Animator anim;
    private AudioSource audioSource;
    private bool isAttacking = false;

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
        if (rageSlider != null) rageSlider.maxValue = maxRage;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, circleRadius, whatIsGround);
        moveInput = Input.GetAxisRaw("Horizontal");

        if (isGrounded && !isJumping && !isPreparingJump && !isAttacking && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            StartCoroutine(JumpRoutine());
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && Time.time >= nextFireTime)
        {
            StartCoroutine(AttackRoutine(false));
        }

        if (Input.GetKeyDown(KeyCode.R) && !isAttacking && currentRage >= maxRage)
        {
            StartCoroutine(AttackRoutine(true));
        }

        CheckFallOff();
        UpdateAnimations();
        UpdateUI();
    }

    // --- HỆ THỐNG GỒNG VÀ CHƯỞNG ---
    IEnumerator AttackRoutine(bool isBigSkill)
    {
        isAttacking = true;
        float originalAnimSpeed = anim.speed; // Lưu lại tốc độ gốc của animation

        // 1. Kích hoạt Animation
        if (anim != null) anim.SetTrigger("isShoot");

        // 2. Thiết lập thông số dựa trên loại đạn
        float delayToSpawn = castPointDelay;
        float recoveryTime = attackRecovery;

        if (isBigSkill)
        {
            // Làm chậm animation để khớp với thời gian gồng lâu
            anim.speed = ultimateAnimSpeed;
            delayToSpawn = ultimateCastDelay;
            recoveryTime = ultimateRecovery;
        }

        // 3. Chờ cho đến khi vung tay xong (Cast Point)
        yield return new WaitForSeconds(delayToSpawn);

        // 4. Thực hiện bắn
        if (isBigSkill) PerformBigSkill();
        else PerformNormalShoot();

        // Trả lại tốc độ animation bình thường sau khi đạn đã bay ra
        anim.speed = originalAnimSpeed;

        // 5. Chờ phần khựng sau khi bắn (Recovery)
        yield return new WaitForSeconds(recoveryTime);

        isAttacking = false;
    }

    void PerformNormalShoot()
    {
        nextFireTime = Time.time + fireRate;
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);
        SpawnProjectile(SwordPrefab, false);
    }

    void PerformBigSkill()
    {
        currentRage = 0;
        if (audioSource != null && bigShootSound != null)
            audioSource.PlayOneShot(bigShootSound);
        SpawnProjectile(BigSwordPrefab, true);
    }

    void SpawnProjectile(GameObject prefab, bool bigSkillFlag)
    {
        if (prefab != null && pos_sword != null)
        {
            GameObject projectile = Instantiate(prefab, pos_sword.transform.position, Quaternion.identity);
            float direction = facingRight ? 1f : -1f;
            Sword swordScript = projectile.GetComponent<Sword>();
            if (swordScript != null)
            {
                swordScript.Initialize(direction);
                if (bigSkillFlag) swordScript.isBigSkill = true;
            }
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        if (isPreparingJump || isAttacking)
        {
            rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
        }
        else
        {
            rigidBody.linearVelocity = new Vector2(moveInput * speed, rigidBody.linearVelocity.y);
        }

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

    void UpdateUI() { if (rageSlider != null) rageSlider.value = currentRage; }
    public void AddRage() { currentRage = Mathf.Clamp(currentRage + rageGainPerHit, 0, maxRage); }
    void Flip() { facingRight = !facingRight; Vector3 scaler = transform.localScale; scaler.x *= -1; transform.localScale = scaler; }
    void UpdateAnimations()
    {
        bool isRunning = Mathf.Abs(moveInput) > 0.1f && isGrounded && !isPreparingJump && !isAttacking;
        anim.SetBool("isRun", isRunning);
        bool isFalling = !isGrounded && rigidBody.linearVelocity.y < -0.1f;
        anim.SetBool("isFall", isFalling);
    }
    void CheckFallOff() { if (transform.position.y < -15f) Died(); }
    void OnCollisionEnter2D(Collision2D coll) { if (coll.gameObject.CompareTag("Batas_Mati")) Died(); }
    public void Died() { SceneManager.LoadScene("GameOver"); }
    void OnDrawGizmosSelected() { if (feetPos != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(feetPos.position, circleRadius); } }
}