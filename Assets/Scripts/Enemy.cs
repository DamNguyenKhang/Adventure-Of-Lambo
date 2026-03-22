using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float speed = 1.5f;
    public LayerMask enemyMask;

    [Header("Cài đặt hướng gốc")]
    public bool spriteFacesLeftByDefault = false;

    [Header("Cài đặt tia dò (Raycast)")]
    public float rayLength = 0.5f;
    public float rayHeightOffset = 1.0f;
    public float rayForwardOffset = 0.5f;

    Rigidbody2D myBody;
    int currentDir = 1;
    float myWidth;
    private bool isKnockedBack = false; // Trạng thái bị đẩy lùi

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        myWidth = GetComponent<Collider2D>().bounds.extents.x;
        if (spriteFacesLeftByDefault) currentDir = -1;
    }

    void FixedUpdate()
    {
        // Nếu đang bị đẩy lùi thì không tự di chuyển
        if (isKnockedBack) return;

        Vector2 startPos = new Vector2(
            transform.position.x + (currentDir * (myWidth + rayForwardOffset)),
            transform.position.y + rayHeightOffset
        );

        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.right * currentDir, rayLength, enemyMask);
        Debug.DrawRay(startPos, Vector2.right * currentDir * rayLength, Color.red);

        if (hit.collider != null)
        {
            Flip();
        }

        myBody.linearVelocity = new Vector2(currentDir * speed, myBody.linearVelocity.y);
    }

    public void GetKnockback(Vector2 direction, float force)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackRoutine(direction, force));
        }
    }

    IEnumerator KnockbackRoutine(Vector2 direction, float force)
    {
        isKnockedBack = true;
        // Đẩy quái: hướng lùi ra sau và hơi bay lên một chút (y=0.5f)
        Vector2 forceVector = new Vector2(direction.x, 0.5f) * force;
        myBody.linearVelocity = Vector2.zero; // Reset vận tốc cũ
        myBody.AddForce(forceVector, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.4f); // Thời gian khựng
        isKnockedBack = false;
    }

    void Flip()
    {
        currentDir *= -1;
        Vector3 newScale = transform.localScale;
        float faceDirection = spriteFacesLeftByDefault ? -1 : 1;
        newScale.x = Mathf.Abs(newScale.x) * currentDir * faceDirection;
        transform.localScale = newScale;
    }
}