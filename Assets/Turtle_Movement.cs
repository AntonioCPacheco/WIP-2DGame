using UnityEngine;
using System.Collections;

public class Turtle_Movement : MonoBehaviour {

    public float maxSpeed = 4f;
    public bool facingRight = false;
    Vector3 vecZero = Vector3.zero;
    Rigidbody2D rbody;

    bool grounded = false;
    public Transform groundCheck;
    public LayerMask whatIsGround;

    bool playerOnTop = false;

    bool v_isInLight = false;

    // Use this for initialization
    void Start () {
        rbody = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        if (v_isInLight)
        {
            rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rbody.velocity = new Vector2((facingRight ? 1 : -1) * 0.3f * maxSpeed, rbody.velocity.y);
            if (playerOnTop)
            {
                GameObject.Find("Player Prefab").GetComponent<Player_Movement>().somethingsVelocity += (rbody.velocity/1.22f);
            }
        }
        else
        {
            rbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void FixedUpdate()
    {
        if (v_isInLight)
        {
            grounded = Mathf.Round(rbody.velocity.y) == 0f;
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void InLight(bool l)
    {
        v_isInLight = l;
    }

    public bool IsInLight()
    {
        return v_isInLight;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Floor"))
            Flip();
    }

    void OnCollisionEnter2D(Collision2D coll)
        {
        Debug.Log("1");
        if (coll.gameObject.CompareTag("Player") && coll.collider.GetType() == typeof(BoxCollider2D))
        {
            Debug.Log("2");
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().onTopOfSomething = true;
            playerOnTop = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && col.collider.GetType() == typeof(BoxCollider2D))
        {
            GameObject.Find("Player Prefab").GetComponent<Player_Movement>().onTopOfSomething = false;
            playerOnTop = false;
        }
    }
}
