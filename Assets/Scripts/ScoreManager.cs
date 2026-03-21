using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("Coin Settings")]
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("Kill Settings")]
    public TextMeshProUGUI killText; // Kéo Text đếm quái vào đây
    private int killCount = 0;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    // Hàm cộng xu (đã có)
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    // HÀM MỚI: Cộng điểm diệt quái
    public void AddKill()
    {
        killCount++;
        killText.text = killCount.ToString();
    }
}