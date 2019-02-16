using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob_Movement : MonoBehaviour {

    [SerializeField]
    private bool facingRight = false;
    [SerializeField]
    private bool needsLight = true;
    [SerializeField]
    private float maxSpeed = 2f;
    [SerializeField]
    private float delay = 2f;
    [SerializeField]
    private float minDistanceToPlayer = 200;
    [SerializeField]
    private LayerMask FloorLayerMask;
    [SerializeField]
    private LayerMask BouncyLayerMask;

    private Rigidbody2D rbody;
    private LightSensor sensor;
    private Transform idleColliders;
    private Transform movingColliders;
    private Animator anim;

    private bool grounded = false;
    private float distance_to_player = 0f;
    private float timeInLight = 0f;
    private bool triggered = false;
    private bool flippedThisFrame = false;

    // Use this for initialization
    void Start()
    {
        distance_to_player = (this.transform.position - GameObject.Find("Player").transform.position).magnitude;
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sensor = GetComponent<LightSensor>();

        idleColliders = transform.Find("IdleColliders");
        movingColliders = transform.Find("MovingColliders");
    }

    // Update is called once per frame
    void Update()
    {
        distance_to_player = (this.transform.position - GameObject.Find("Player").transform.position).magnitude;
        if (!triggered)
        {
            if (sensor.IsInLight() || !needsLight)
            {
                if ((timeInLight >= delay || !needsLight) && distance_to_player <= minDistanceToPlayer)
                {
                    triggered = true;
                    rbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    rbody.velocity = new Vector2((facingRight ? 1 : -1) * maxSpeed, rbody.velocity.y);
                    anim.SetBool("Moving", true);
                    switchColliders();
                }
                else if(sensor.IsInLight() && distance_to_player <= minDistanceToPlayer)
                {
                    anim.SetBool("WakingUp", true);
                    timeInLight += Time.deltaTime;
                }
            }
            else
            {
                anim.SetBool("WakingUp", false);
                timeInLight = 0;
                rbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
        }
        else
        {
            anim.SetBool("Moving", true);
            rbody.velocity = new Vector2((facingRight ? 1 : -1) * maxSpeed, rbody.velocity.y);
        }
    }

    void FixedUpdate()
    {
        grounded = Mathf.Round(rbody.velocity.y) == 0f;
    }

    private void LateUpdate()
    {
        flippedThisFrame = false;
    }

    void switchColliders()
    {
        idleColliders.gameObject.SetActive(!idleColliders.gameObject.activeSelf);
        movingColliders.gameObject.SetActive(!movingColliders.gameObject.activeSelf);
    }

    private void Flip()
    {
        if (flippedThisFrame || !flippedThisFrame)
        {
            flippedThisFrame = true;
            facingRight = !facingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        LayerMask l = other.gameObject.layer;
        if (FloorLayerMask == (FloorLayerMask | (1 << l)))
        {
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LayerMask l = collision.gameObject.layer;
        if (BouncyLayerMask == (BouncyLayerMask | (1 << l)))
        {
            Flip();
        }
    }
}