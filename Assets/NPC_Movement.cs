using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_Movement : MonoBehaviour
{
    public float nextStep = 160;

    public float maxSpeed = 4f;
    bool facingPlayer;
    public bool facingRight = false;

    bool grounded = false;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    
    Animator anim;
    Rigidbody2D rbody;
    GameObject player;

    float xBeforeTarget;
    float target;
    bool reachedTarget = true;
    bool lastTarget = false;

    bool inDialogue = false;
    bool changedNextStep = true;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player Prefab");
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inDialogue || lastTarget) { return; }
        if (isReadyForNextStep())
        {
            xBeforeTarget = this.transform.position.x;
            target = getNextStep();
            facingRight = xBeforeTarget < target;
            Turn();
        }
        if (!grounded || reachedTarget && !changedNextStep)
        {
            rbody.velocity = new Vector2(0, rbody.velocity.y);
        }
        else
        {
            rbody.velocity = new Vector2((facingRight ? 1 : -1) * 0.3f * maxSpeed, rbody.velocity.y);
        }

        facingPlayer = ((player.transform.position.x - transform.position.x) >= 0 && facingRight) || ((player.transform.position.x - transform.position.x) <= 0 && !facingRight);
        //anim.SetFloat("Speed", 0);
        checkReachedTarget();
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle (groundCheck.position, 0.4f, whatIsGround);
        anim.SetBool("Grounded", grounded);
        
        anim.SetFloat("Speed", Mathf.Abs(rbody.velocity.x));
        //if (!facingPlayer && (Mathf.Abs(player.transform.position.x - transform.position.x) > 7))
        //    Flip();
    }

    float getDistance(float x1, float x2)
    {
        return Mathf.Abs((x1 + Mathf.Abs(x1) + Mathf.Abs(x2)) - (x2 + Mathf.Abs(x1) + Mathf.Abs(x2)));
    }

    void Turn()
    {
        if (facingRight)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = Mathf.Abs(theScale.x);
            transform.localScale = theScale;
        }
        else
        {
            Vector3 theScale = transform.localScale;
            theScale.x = - Mathf.Abs(theScale.x);
            transform.localScale = theScale;
        }
    }

    void checkReachedTarget()
    {
        if(xBeforeTarget > target)
        {
            reachedTarget = (this.transform.position.x <= target);
        }
        else
        {
            reachedTarget = (this.transform.position.x > target);
        }
    }

    bool isReadyForNextStep()
    {
        return (reachedTarget && !inDialogue && changedNextStep);
    }

    public void setNextStep(float nextStep)
    {
        changedNextStep = true;
        this.nextStep = nextStep;
    }

    float getNextStep()
    {
        /*if (pathQueue.Count == 0)
        {
            print("path empty");
            lastTarget = true;
            rbody.constraints = RigidbodyConstraints2D.FreezePositionX;
            return this.transform.position.x;
        }*/
        changedNextStep = false;
        return nextStep;
    }

    public void startDialogue()
    {
        inDialogue = true;
    }

    public void stopDialogue()
    {
        inDialogue = false;
    }

    public void moveNPCto(Vector2 newPos)
    {
        this.transform.position = newPos;
        nextStep = newPos.x + 50;
    }
}