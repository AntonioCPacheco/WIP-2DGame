using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float maxSpeed = 4f;
    public bool facingRight = false;

    Vector3 vecZero = Vector3.zero;
    Rigidbody2D rbody;

    bool playerOnTop = false;

    float lastX = 0f;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rbody.velocity = new Vector2((facingRight ? 1 : -1) * 0.3f * maxSpeed, 0);
        if (playerOnTop && lastX != transform.position.x)
        {
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().somethingsVelocity = (rbody.velocity);
        }
        lastX = transform.position.x;
    }

    public void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player") && coll.collider.GetType() == typeof(CircleCollider2D))
        {
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().onTopOfSomething = true;
            playerOnTop = true;
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("Floor"))
            Flip();
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && col.collider.GetType() == typeof(CircleCollider2D))
        {
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().onTopOfSomething = false;
            playerOnTop = false;
        }
    }
}
