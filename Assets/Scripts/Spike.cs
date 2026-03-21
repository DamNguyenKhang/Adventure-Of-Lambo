using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spike : MonoBehaviour {

    public float damage;
	public float damageRate;
	public float pushBackForce;

	float nextDamage;

    // Use this for initialization
    Animator myAnim;

    void Start()
    {
        nextDamage = 0f;
        // 2. Lấy component Animator gắn trên Boss khi game bắt đầu
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
		
	}
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    // if(collision.CompareTag("Player"))
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //     Debug.Log("Die");
    // }
    // }
    

    // Giả sử bạn đang dùng giải pháp 2 Collider (Collider chính là vật lý, Collider phụ là Trigger gây sát thương)
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && nextDamage < Time.time)
        {
            PlayerHealth thePlayerHeath = other.gameObject.GetComponent<PlayerHealth>();
            if (thePlayerHeath != null)
            {
                thePlayerHeath.addDamage(damage);
                nextDamage = Time.time + damageRate;

                // 3. Kích hoạt Trigger "Attack" trong Animator
                if (myAnim != null)
                {
                    myAnim.SetTrigger("Attack");
                }

                pushBack(other.transform);
            }
        }
    }

    void pushBack(Transform pushObject){
		Vector2 pushDirection = new Vector2(0, (pushObject.position.y - transform.position.y)).normalized;
		pushDirection*=pushBackForce;
		Rigidbody2D pushRB = pushObject.gameObject.GetComponent<Rigidbody2D>();
		pushRB.linearVelocity = Vector2.zero;
		pushRB.AddForce(pushDirection, ForceMode2D.Impulse);
	}
}
