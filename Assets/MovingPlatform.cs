using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float maxSpeed = 4f;

    Vector3 vecZero = Vector3.zero;
    Rigidbody2D rbody;

    bool playerOnTop = false;

    float lastX = 0f;

    public bool enabled = true;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            rbody.velocity = new Vector2(0.3f * maxSpeed, 0);
        }
        else
        {
            rbody.velocity = new Vector2(0, 0);
        }
        if (playerOnTop && lastX != transform.position.x)
        {
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().somethingsVelocity = (rbody.velocity);
        }
        lastX = transform.position.x;
    }

    public void Flip()
    {
        maxSpeed *= -1;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player") && coll.collider.GetType() == typeof(CircleCollider2D))
        {
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().onTopOfSomething = true;
            playerOnTop = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && col.collider.GetType() == typeof(CircleCollider2D))
        {
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().onTopOfSomething = false;
            playerOnTop = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
            Flip();
    }

    public void enable()
    {
        enabled = true;
    }

    public void disable()
    {
        enabled = false;
    }
}
