using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Thông số sức khỏe")]
    public float fullHealth;
    float currentHealth;

    [Header("Âm thanh & Hiệu ứng")]
    public AudioClip playerHurt;
    AudioSource playerAS;

    [Header("Giao diện (UI)")]
    public Slider heartBar;
    public Image damageScreen;
    public Color damagedColour = new Color(1f, 0f, 0f, 0.5f); // Màu đỏ nhạt khi dính đòn
    public float smoothColour = 5f;

    [Header("Cấu hình bất tử tạm thời")]
    public float invincibilityDuration = 1f; // 1 giây bất tử sau khi trúng đòn
    bool isInvincible = false;
    bool damaged = false;

    // Các thành phần tham chiếu
    PlayerController playerControl;
    Animator myAnim;
    SpriteRenderer mySR;

    void Start()
    {
        currentHealth = fullHealth;
        playerControl = GetComponent<PlayerController>();
        myAnim = GetComponent<Animator>();
        mySR = GetComponent<SpriteRenderer>();
        playerAS = GetComponent<AudioSource>();

        // Khởi tạo thanh máu
        if (heartBar != null)
        {
            heartBar.maxValue = fullHealth;
            heartBar.value = fullHealth;
        }

        damaged = false;
    }

    void Update()
    {
        // Hiệu ứng nhấp nháy màn hình đỏ khi dính đòn
        if (damaged)
        {
            damageScreen.color = damagedColour;
        }
        else if (damageScreen != null)
        {
            damageScreen.color = Color.Lerp(damageScreen.color, Color.clear, smoothColour * Time.deltaTime);
        }
        damaged = false;
    }

    public void addDamage(float damage)
    {
        // Nếu đang trong thời gian bất tử thì không nhận thêm sát thương
        if (isInvincible || damage <= 0) return;

        currentHealth -= damage;

        // Cập nhật UI và Âm thanh
        if (heartBar != null) heartBar.value = currentHealth;
        if (playerHurt != null && playerAS != null) playerAS.PlayOneShot(playerHurt);

        damaged = true;

        // Kích hoạt Animation "Hurt"
        if (myAnim != null)
        {
            myAnim.SetTrigger("Hurt");
        }

        // Bắt đầu thời gian bất tử
        StartCoroutine(HandleInvincibility());

        if (currentHealth <= 0)
        {
            makeDead();
        }
    }

    // Coroutine xử lý thời gian bất tử và hiệu ứng nhấp nháy Sprite
    IEnumerator HandleInvincibility()
    {
        isInvincible = true;

        // Hiệu ứng nhấp nháy nhân vật (mờ dần rồi hiện lại)
        float elapsed = 0;
        while (elapsed < invincibilityDuration)
        {
            if (mySR != null) mySR.enabled = !mySR.enabled; // Tắt/Bật sprite liên tục
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        if (mySR != null) mySR.enabled = true; // Đảm bảo hiện lại sprite sau khi hết bất tử
        isInvincible = false;
    }

    public void addHealth(float health)
    {
        currentHealth += health;
        if (currentHealth > fullHealth) currentHealth = fullHealth;
        if (heartBar != null) heartBar.value = currentHealth;
    }

    public void makeDead()
    {
        // Chuyển sang scene GameOver
        SceneManager.LoadScene("GameOver");
    }
}